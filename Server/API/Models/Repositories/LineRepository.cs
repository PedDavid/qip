﻿using API.Models.IRepositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Repositories {
    public class LineRepository : ILineRepository {
        private readonly IConfiguration _configuration;
        private readonly SqlServerTemplate _queryTemplate;

        public LineRepository(SqlServerTemplate queryTemplate, IConfiguration configuration) {
            _configuration = configuration;
            _queryTemplate = queryTemplate;
        }

        public long Add(Line line) {
            long lineId = line.Id;

            PointsTable points = new PointsTable(line.Points);

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = lineId;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = line.BoardId;

            parameters//TODO rever, seria melhor receber o styleId, até porque isso é uma coisa que a app deveria saber; se quiser uma nova cor, ??insere quando seleciona??
                .Add("@color", SqlDbType.VarChar)
                .Value = line.Style.Color;

            parameters
                .Add("@points", SqlDbType.Structured)
                .Value = points;

            _queryTemplate.StoredProcedure(INSERT_LINE, parameters);

            return lineId;
        }

        public Line Find(long id, long boardId) {
            using(SqlConnection con = new SqlConnection()) {
                con.ConnectionString = _configuration.GetConnectionString("QIPContext");
                con.Open();
                SqlTransaction tran = con.BeginTransaction(IsolationLevel.Serializable);//TODO Rever niveis transacionais
                try {
                    Line line = null;
                    using(SqlCommand cmd = con.CreateCommand()) {
                        cmd.Transaction = tran;
                        cmd.CommandText = SELECT_LINES;

                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;
                        parameters.Add("@id", SqlDbType.BigInt).Value = id;
                        parameters.ForEach(prm => cmd.Parameters.Add(prm));

                        using(SqlDataReader dr = cmd.ExecuteReader()) {
                            if(dr.Read()) {
                                line = GetLine(dr);
                            }
                        }
                    }

                    if(line != null) {
                        using(SqlCommand cmd = con.CreateCommand()) {
                            cmd.Transaction = tran;
                            cmd.CommandText = SELECT_LINES_POINTS;

                            List<SqlParameter> parameters = new List<SqlParameter>();
                            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;
                            parameters.Add("@id", SqlDbType.BigInt).Value = id;
                            parameters.ForEach(prm => cmd.Parameters.Add(prm));

                            List<LinePoint> points = new List<LinePoint>();
                            using(SqlDataReader dr = cmd.ExecuteReader()) {
                                while(dr.Read()) {
                                    points.Add(GetPointWithStyle(dr));
                                }
                            }
                            line.Points = points;
                        }
                    }

                    tran.Commit();

                    return line;
                }
                catch(Exception ex) {
                    tran.Rollback();
                    Console.WriteLine("E R R O : " + ex.Message);
                    throw;//TODO alterar
                }
            }
        }

        public IEnumerable<Line> GetAll(long boardId) {
            using(SqlConnection con = new SqlConnection()) {
                con.ConnectionString = _configuration.GetConnectionString("QIPContext");
                con.Open();
                SqlTransaction tran = con.BeginTransaction(IsolationLevel.Serializable);//TODO Rever niveis transacionais
                try {
                    List<Line> lines = new List<Line>();
                    using(SqlCommand cmd = con.CreateCommand()) {
                        cmd.Transaction = tran;
                        cmd.CommandText = SELECT_ALL_LINES;

                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;
                        parameters.ForEach(prm => cmd.Parameters.Add(prm));

                        using(SqlDataReader dr = cmd.ExecuteReader()) {
                            while(dr.Read()) {
                                lines.Add(GetLine(dr));
                            }
                        }
                    }

                    using(SqlCommand cmd = con.CreateCommand()) {
                        cmd.Transaction = tran;
                        cmd.CommandText = SELECT_ALL_LINES_POINTS;

                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;
                        parameters.ForEach(prm => cmd.Parameters.Add(prm));

                        using(SqlDataReader dr = cmd.ExecuteReader()) {
                            lines = GetLinePoints(lines, dr);
                        }
                    }

                    tran.Commit();

                    return lines;
                }
                catch(Exception ex) {
                    tran.Rollback();
                    Console.WriteLine("E R R O : " + ex.Message);
                    throw;//TODO alterar
                }
            }
        }

        public void Remove(long id, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = id;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = boardId;

            _queryTemplate.StoredProcedure(REMOVE_LINE, parameters);
        }

        public void Update(Line line) {
            long lineId = line.Id;

            PointsTable points = new PointsTable(line.Points);

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = line.Id;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = line.BoardId;

            parameters
                .Add("@color", SqlDbType.VarChar)
                .Value = line.Style.Color;

            parameters
                .Add("@points", SqlDbType.Structured)
                .Value = points;

            _queryTemplate.StoredProcedure(UPDATE_LINE, parameters);
        }

        public void PartialUpdate(Line figure) {
            throw new NotImplementedException();
        }

        //SQL Functions
        private static readonly string SELECT_ALL_LINES = "SELECT id, boardId, isClosedForm, lineStyleId, lineColor FROM dbo.GetLinesInfo(@boardId) ORDER BY id";
        private static readonly string SELECT_ALL_LINES_POINTS = "SELECT id, boardId, linePointX, linePointY, linePointIdx, pointStyleId, pointWidth FROM dbo.GetLinesPoints(@boardId) ORDER BY id";

        private static readonly string SELECT_LINES = "SELECT id, boardId, isClosedForm, lineStyleId, lineColor FROM dbo.GetLinesInfo(@boardId) WHERE id=@id";
        private static readonly string SELECT_LINES_POINTS = "SELECT id, boardId, linePointX, linePointY, linePointIdx, pointStyleId, pointWidth FROM dbo.GetLinesPoints(@boardId) WHERE id=@id";

        //SQL Stored Procedures
        private static readonly string INSERT_LINE = "dbo.InsertNewLine";
        private static readonly string REMOVE_LINE = "dbo.RemoveFigure";
        private static readonly string UPDATE_LINE = "dbo.UpdateLine";

        //Extract Data From Data Reader
        private static Line GetLine(SqlDataReader dr) {
            return new Line(dr.GetInt64(1), dr.GetInt64(0)) {
                Closed = dr.GetBoolean(2),
                Style = new LineStyle(dr.GetInt64(3)) {
                    Color = dr.GetString(4)
                }
            };
        }

        private static List<Line> GetLinePoints(List<Line> orderedLines, SqlDataReader dr) {
            foreach(Line line in orderedLines) {
                if(!dr.Read())
                    break;

                long lineId = line.Id;

                List<LinePoint> points = new List<LinePoint>();
                do {
                    if(dr.GetInt64(0) != lineId)
                        break;

                    LinePoint curr = GetPointWithStyle(dr);

                    points.Add(curr);
                } while(dr.Read());

                line.Points = points;
            }

            return orderedLines;
        }

        private static LinePoint GetPointWithStyle(SqlDataReader dr) {
            return new LinePoint() {
                X = dr.GetInt32(2),
                Y = dr.GetInt32(3),
                Idx = dr.GetInt32(4),
                Style = new PointStyle(dr.GetInt64(5)) {
                    Width = dr.GetInt32(6)
                }
            };
        }
    }
}
