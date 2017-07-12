﻿using API.Domain;
using API.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories {
    public class UsersBoardsRepository : IUsersBoardsRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public UsersBoardsRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task Add(UserBoard userBoard) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@userId", SqlDbType.BigInt)
                    .Value = userBoard.UserId.Value;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = userBoard.BoardId.Value;

            parameters
                .Add("@permission", SqlDbType.TinyInt)
                .Value = (byte)userBoard.Permission;

            return _queryTemplate.QueryAsync(INSERT_USER_BOARD, parameters);
        }

        public Task<UserBoard> Find(long boardId, long userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

            return _queryTemplate.QueryForObjectAsync(SELECT_USER_BOARD, parameters, GetUserBoard);
        }

        public Task<UserBoard_Board> FindBoard(long userId, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

            return _queryTemplate.QueryForObjectAsync(SELECT_BOARD, parameters, GetBoard);
        }

        public Task<UserBoard_User> FindUser(long boardId, long userId) {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

                return _queryTemplate.QueryForObjectAsync(SELECT_USER, parameters, GetUser);
        }

        public Task<IEnumerable<UserBoard_Board>> GetAllBoards(long userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

            return _queryTemplate.QueryAsync(SELECT_ALL_BOARDS, parameters, GetBoard);
        }

        public Task<IEnumerable<UserBoard_User>> GetAllUsers(long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            return _queryTemplate.QueryAsync(SELECT_ALL_USERS, parameters, GetUser);
        }

        public Task Remove(long boardId, long userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            return _queryTemplate.QueryAsync(DELETE_USER_BOARD, parameters);
        }

        public Task Update(UserBoard userBoard) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@userId", SqlDbType.BigInt)
                    .Value = userBoard.UserId.Value;

            parameters
                    .Add("@boardId", SqlDbType.BigInt)
                .Value = userBoard.BoardId.Value;

            parameters
                    .Add("@permission", SqlDbType.TinyInt)
                .Value = (byte)userBoard.Permission;

            return _queryTemplate.QueryAsync(UPDATE_USER_BOARD, parameters);
        }

        //SQL Commands
        private static readonly string SELECT_ALL_USERS = "SELECT permission, userId, username, [name] FROM dbo.Full_User_Board WHERE boardId=@boardId";
        private static readonly string SELECT_ALL_BOARDS = "SELECT permission, boardId, boardName, maxDistPoints FROM dbo.Full_User_Board WHERE userId=@userId";
        private static readonly string SELECT_USER_BOARD = "SELECT permission, boardId, userId FROM dbo.User_Board WHERE boardId=@boardId and userId=@userId";
        private static readonly string SELECT_USER = "SELECT permission, userId, username, [name] FROM dbo.Full_User_Board WHERE boardId=@boardId and userId=@userId";
        private static readonly string SELECT_BOARD = "SELECT permission, boardId, boardName, maxDistPoints FROM dbo.Full_User_Board WHERE userId=@userId and boardId=@boardId";
        private static readonly string INSERT_USER_BOARD = "INSERT INTO dbo.User_Board (userId, boardId, permission) VALUES (@userId, @boardId, @permission)";
        private static readonly string DELETE_USER_BOARD = "DELETE FROM dbo.User_Board WHERE userId = @userId and boardId = @boardId";
        private static readonly string UPDATE_USER_BOARD = "UPDATE dbo.User_Board " +
                                                           "SET permission= @permission " +
                                                           "WHERE userId = @userId and boardId = @boardId";

        //Extract Data From Data Reader
        private static UserBoard_User GetUser(SqlDataReader dr) {
            return new UserBoard_User() {
                Permission = (BoardPermission)dr.GetByte(0),
                User = new User(dr.GetInt64(1)) {
                    UserName = dr.GetString(2),
                    Name = dr.GetString(3)
                }
            };
        }

        private static UserBoard_Board GetBoard(SqlDataReader dr) {
            return new UserBoard_Board() {
                Permission = (BoardPermission)dr.GetByte(0),
                Board = new Board(dr.GetInt64(1)) {
                    Name = dr.GetString(2),
                    MaxDistPoints = dr.GetByte(3)
                }
            };
        }

        private static UserBoard GetUserBoard(SqlDataReader dr) {
            return new UserBoard() {
                Permission = (BoardPermission)dr.GetByte(0),
                BoardId = dr.GetInt64(1),
                UserId = dr.GetInt64(2)
            };
        }

    }
}
