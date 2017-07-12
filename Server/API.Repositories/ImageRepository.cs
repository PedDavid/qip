﻿using API.Domain;
using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories {
    public class ImageRepository : IImageRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public ImageRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public async Task<long> Add(Image image) {
            long imageId = image.Id;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = imageId;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = image.BoardId;

            parameters
                .Add("@x", SqlDbType.Int)
                .Value = image.Origin.X;

            parameters
                .Add("@y", SqlDbType.Int)
                .Value = image.Origin.Y;

            parameters
                .Add("@width", SqlDbType.Int)
                .Value = image.Width;

            parameters
                .Add("@height", SqlDbType.Int)
                .Value = image.Height;

            parameters
                .Add("@src", SqlDbType.NVarChar)
                .Value = image.Src;

            await _queryTemplate.StoredProcedureAsync(INSERT_IMAGE, parameters);

            return imageId;
        }

        public Task<Image> Find(long id, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@id", SqlDbType.BigInt).Value = id;

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            return _queryTemplate.QueryForObjectAsync(SELECT_IMAGE, parameters, GetImage);
        }

        public Task<IEnumerable<Image>> GetAll(long boardId) {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                return _queryTemplate.QueryAsync(SELECT_ALL, parameters, GetImage);
        }

        public Task Remove(long id, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = id;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = boardId;

            return _queryTemplate.StoredProcedureAsync(REMOVE_IMAGE, parameters);
        }

        public Task Update(Image image) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = image.Id;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = image.BoardId;

            parameters
                .Add("@x", SqlDbType.Int)
                .Value = image.Origin.X;

            parameters
                .Add("@y", SqlDbType.Int)
                .Value = image.Origin.Y;

            parameters
                .Add("@width", SqlDbType.Int)
                .Value = image.Width;

            parameters
                .Add("@height", SqlDbType.Int)
                .Value = image.Height;

            parameters
                .Add("@src", SqlDbType.NVarChar)
                .Value = image.Src;

            return _queryTemplate.StoredProcedureAsync(UPDATE_IMAGE, parameters);
        }

        public Task PartialUpdate(Image image) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@figureId", SqlDbType.BigInt)
                    .Value = image.Id;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = image.BoardId;

            parameters
                .Add("@x", SqlDbType.Int)
                .Value = image.Origin.X ?? SqlInt32.Null;

            parameters
                .Add("@y", SqlDbType.Int)
                .Value = image.Origin.Y ?? SqlInt32.Null;

            parameters
                .Add("@width", SqlDbType.Int)
                .Value = image.Width ?? SqlInt32.Null;

            parameters
                .Add("@height", SqlDbType.Int)
                .Value = image.Height ?? SqlInt32.Null;

            parameters
                .Add("@src", SqlDbType.NVarChar)
                .Value = image.Src ?? SqlString.Null;

            return _queryTemplate.StoredProcedureAsync(UPDATE_IMAGE, parameters);
        }

        //SQL Functions
        private static readonly string SELECT_ALL = "SELECT id, boardId, pointX, pointY, src, imageWidth, imageHeight FROM dbo.GetImages(@boardId)";
        private static readonly string SELECT_IMAGE = "SELECT id, boardId, pointX, pointY, src, imageWidth, imageHeight FROM dbo.GetImages(@boardId) WHERE id=@id";

        //SQL Stored Procedures
        private static readonly string INSERT_IMAGE = "dbo.InsertNewImage ";
        private static readonly string REMOVE_IMAGE = "dbo.RemoveFigure";
        private static readonly string UPDATE_IMAGE = "dbo.UpdateImage";

        //Extract Data From Data Reader
        private static Image GetImage(SqlDataReader dr) {
            return new Image(dr.GetInt64(1), dr.GetInt64(0)) {
                Origin = new Point() {
                    X = dr.GetInt32(2),
                    Y = dr.GetInt32(3)
                },
                Src = dr.GetString(4),
                Width = dr.GetInt32(5),
                Height = dr.GetInt32(6)
            };
        }
    }
}
