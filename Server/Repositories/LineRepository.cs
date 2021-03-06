﻿using QIP.Domain;
using QIP.Public.IRepositories;
using QIP.Repositories.Model;
using QIP.Repositories.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using QIP.Repositories.Extensions;

namespace QIP.Repositories {
    public class LineRepository : ILineRepository {
        private readonly SqlServerTemplate _queryTemplate;
        private readonly DatabaseOptions _options;

        public LineRepository(SqlServerTemplate queryTemplate, IOptionsSnapshot<DatabaseOptions> options) {
            _queryTemplate = queryTemplate;
            _options = options.Value;
        }

        public Task<bool> ExistsAsync(long id, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            return _queryTemplate.QueryForScalarAsync<bool>(LINE_EXISTS, parameters);
        }

        public async Task AddAsync(Line line) {
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

            parameters
                .Add("@isClosedForm", SqlDbType.Bit)
                .Value = line.Closed;

            await _queryTemplate.StoredProcedureAsync(INSERT_LINE, parameters);
        }

        public async Task<Line> FindAsync(long id, long boardId) {
            using(SqlConnection con = new SqlConnection(_options.Context)) {
                await con.OpenAsync();
                SqlTransaction tran = con.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    Line line = null;
                    using(SqlCommand cmd = con.CreateCommand()) {
                        cmd.Transaction = tran;
                        cmd.CommandText = SELECT_LINE;

                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;
                        parameters.Add("@id", SqlDbType.BigInt).Value = id;
                        parameters.ForEach(prm => cmd.Parameters.Add(prm));

                        using(SqlDataReader dr = await cmd.ExecuteReaderAsync()) {
                            if(await dr.ReadAsync()) {
                                line = GetLine(dr);
                            }
                        }
                    }

                    if(line != null) {
                        using(SqlCommand cmd = con.CreateCommand()) {
                            cmd.Transaction = tran;
                            cmd.CommandText = SELECT_LINE_POINTS;

                            List<SqlParameter> parameters = new List<SqlParameter>();
                            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;
                            parameters.Add("@id", SqlDbType.BigInt).Value = id;
                            parameters.ForEach(prm => cmd.Parameters.Add(prm));

                            List<LinePoint> points = new List<LinePoint>();
                            using(SqlDataReader dr = await cmd.ExecuteReaderAsync()) {
                                while(await dr.ReadAsync()) {
                                    points.Add(GetPointWithStyle(dr));
                                }
                            }
                            line.Points = points;
                        }
                    }

                    tran.Commit();

                    return line;
                }
                catch(Exception) {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Line>> GetAllAsync(long boardId) {
            using(SqlConnection con = new SqlConnection(_options.Context)) {
                await con.OpenAsync();
                SqlTransaction tran = con.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    List<Line> lines = new List<Line>();
                    using(SqlCommand cmd = con.CreateCommand()) {
                        cmd.Transaction = tran;
                        cmd.CommandText = SELECT_ALL_LINES;

                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;
                        parameters.ForEach(prm => cmd.Parameters.Add(prm));

                        using(SqlDataReader dr = await cmd.ExecuteReaderAsync()) {
                            while(await dr.ReadAsync()) {
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

                        using(SqlDataReader dr = await cmd.ExecuteReaderAsync()) {
                            lines = GetLinePoints(lines, dr);
                        }
                    }

                    tran.Commit();

                    return lines;
                }
                catch(Exception) {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public Task RemoveAsync(long id, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = id;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = boardId;

            return _queryTemplate.StoredProcedureAsync(REMOVE_LINE, parameters);
        }

        public Task UpdateAsync(Line line) {
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

            parameters
                .Add("@isClosedForm", SqlDbType.Bit)
                .Value = line.Closed;

            return _queryTemplate.StoredProcedureAsync(UPDATE_LINE, parameters);
        }

        //SQL Functions
        private static readonly string LINE_EXISTS = "SELECT CAST(count(figureId) as BIT) FROM dbo.Line WHERE figureId = @id and boardId = @boardId";
        private static readonly string SELECT_ALL_LINES = "SELECT id, boardId, isClosedForm, lineStyleId, lineColor FROM dbo.GetLinesInfo(@boardId) ORDER BY id";
        private static readonly string SELECT_ALL_LINES_POINTS = "SELECT id, boardId, linePointX, linePointY, linePointIdx, pointStyleWidth FROM dbo.GetLinesPoints(@boardId) ORDER BY id, linePointIdx";

        private static readonly string SELECT_LINE = "SELECT id, boardId, isClosedForm, lineStyleId, lineColor FROM dbo.GetLinesInfo(@boardId) WHERE id=@id";
        private static readonly string SELECT_LINE_POINTS = "SELECT id, boardId, linePointX, linePointY, linePointIdx, pointStyleWidth FROM dbo.GetLinesPoints(@boardId) WHERE id=@id ORDER BY linePointIdx";

        //SQL Stored Procedures
        private static readonly string INSERT_LINE = "dbo.InsertNewLine";
        private static readonly string REMOVE_LINE = "dbo.RemoveFigure";
        private static readonly string UPDATE_LINE = "dbo.UpdateLine";

        //Extract Data From Data Reader
        private static Line GetLine(SqlDataReader dr) {
            return new Line() {
                Id = dr.GetInt64(0),
                BoardId = dr.GetInt64(1),
                Closed = dr.GetBoolean(2),
                Style = new LineStyle() {
                    Id = dr.GetInt64(3),
                    Color = dr.GetString(4)
                }
            };
        }

        private static List<Line> GetLinePoints(List<Line> orderedLines, SqlDataReader dr) {
            if(!dr.Read())
                return orderedLines;

            bool areMoreRows = true;
            foreach(Line line in orderedLines) {
                if(!areMoreRows) break;

                long lineId = line.Id;

                List<LinePoint> points = new List<LinePoint>();
                long currId;
                do {
                    currId = dr.GetInt64(0);

                    if(currId != lineId) {
                        while(currId < lineId && dr.Read()) {
                            currId = dr.GetInt64(0);
                        }
                        break;
                    }

                    LinePoint curr = GetPointWithStyle(dr);

                    points.Add(curr);
                } while(areMoreRows = dr.Read());

                line.Points = points;
            }

            return orderedLines;
        }

        private static LinePoint GetPointWithStyle(SqlDataReader dr) {
            return new LinePoint() {
                X = dr.GetInt32(2),
                Y = dr.GetInt32(3),
                Idx = dr.GetInt32(4),
                Style = new PointStyle() {
                    Width = dr.GetSqlInt32(5).ToNullableInt()
                }
            };
        }
    }
}
