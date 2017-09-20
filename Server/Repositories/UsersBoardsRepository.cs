using QIP.Domain;
using QIP.Public.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace QIP.Repositories {
    public class UsersBoardsRepository : IUsersBoardsRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public UsersBoardsRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public Task<bool> ExistsAsync(long boardId, string userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            parameters.Add("@userId", SqlDbType.VarChar).Value = userId;

            return _queryTemplate.QueryForScalarAsync<bool>(USER_BOARD_EXISTS, parameters);
        }

        public Task AddAsync(UserBoard userBoard) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@userId", SqlDbType.VarChar)
                    .Value = userBoard.UserId;

            parameters
                .Add("@boardId", SqlDbType.BigInt)
                .Value = userBoard.BoardId;

            parameters
                .Add("@permission", SqlDbType.TinyInt)
                .Value = (byte)userBoard.Permission;

            return _queryTemplate.CommandAsync(INSERT_USER_BOARD, parameters);
        }

        public Task<UserBoard> FindAsync(long boardId, string userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            parameters.Add("@userId", SqlDbType.VarChar).Value = userId;

            return _queryTemplate.QueryForObjectAsync(SELECT_USER_BOARD, parameters, GetUserBoard);
        }

        public Task<UserBoard_Board> FindBoardAsync(string userId, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            parameters.Add("@userId", SqlDbType.VarChar).Value = userId;

            return _queryTemplate.QueryForObjectAsync(SELECT_BOARD, parameters, GetBoard);
        }

        public Task<UserBoard_User> FindUserAsync(long boardId, string userId) {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                parameters.Add("@userId", SqlDbType.VarChar).Value = userId;

                return _queryTemplate.QueryForObjectAsync(SELECT_USER, parameters, GetUser);
        }

        public async Task<BoardPermission> FindPermissionAsync(string userId, long boardId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@userId", SqlDbType.VarChar).Value = userId??SqlString.Null;

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            return (BoardPermission) await _queryTemplate.QueryForScalarAsync<byte>(SELECT_PERMISSION, parameters);
        }

        public Task<IEnumerable<UserBoard_Board>> GetAllBoardsAsync(string userId, long index, long size) {
            return GetAllBoardsAsync(userId, index, size, null);
        }

        public Task<IEnumerable<UserBoard_Board>> GetAllBoardsAsync(string userId, long index, long size, string search) {
            string selectBoards = SELECT_ALL_BOARDS;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@userId", SqlDbType.VarChar).Value = userId;

            parameters.Add("@take", SqlDbType.BigInt).Value = size;

            parameters.Add("@skip", SqlDbType.BigInt).Value = index * size;

            if(search != null) {
                parameters.Add("@search", SqlDbType.NVarChar).Value = search;
                selectBoards = SELECT_SEARCH_BOARDS;
            }

            return _queryTemplate.QueryAsync(selectBoards, parameters, GetBoard);
        }

        public Task<IEnumerable<UserBoard_User>> GetAllUsersAsync(long boardId, long index, long size) {
            return GetAllUsersAsync(boardId, index, size, null);
        }

        public Task<IEnumerable<UserBoard_User>> GetAllUsersAsync(long boardId, long index, long size, string search) {
            string selectUsers = SELECT_ALL_USERS;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            parameters.Add("@take", SqlDbType.BigInt).Value = size;

            parameters.Add("@skip", SqlDbType.BigInt).Value = index * size;

            if(search != null) {
                parameters.Add("@search", SqlDbType.NVarChar).Value = search;
                selectUsers = SELECT_SEARCH_USERS;
            }

            return _queryTemplate.QueryAsync(selectUsers, parameters, GetUser);
        }

        public Task RemoveAsync(long boardId, string userId) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add("@userId", SqlDbType.VarChar).Value = userId;

            parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

            return _queryTemplate.CommandAsync(DELETE_USER_BOARD, parameters);
        }

        public Task UpdateAsync(UserBoard userBoard) {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters
                    .Add("@userId", SqlDbType.VarChar)
                    .Value = userBoard.UserId;

            parameters
                    .Add("@boardId", SqlDbType.BigInt)
                .Value = userBoard.BoardId;

            parameters
                    .Add("@permission", SqlDbType.TinyInt)
                .Value = (byte)userBoard.Permission;

            return _queryTemplate.CommandAsync(UPDATE_USER_BOARD, parameters);
        }

        //SQL Commands
        private static readonly string USER_BOARD_EXISTS = "SELECT CAST(count(*) as BIT) FROM dbo.User_Board WHERE userId = @userId and boardId = @boardId";
        private static readonly string SELECT_ALL_USERS = "SELECT permission, userId " +
                                                          "FROM dbo.Full_User_Board WHERE boardId=@boardId " +
                                                          "ORDER BY userId " +
                                                          "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        private static readonly string SELECT_SEARCH_USERS = "SELECT permission, userId " +
                                                             "FROM dbo.Full_User_Board WHERE boardId=@boardId " +
                                                             "WHERE CONTAINS((username, [name]), @search) " +
                                                             "ORDER BY userId " +
                                                             "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        private static readonly string SELECT_ALL_BOARDS = "SELECT permission, boardId, boardName, maxDistPoints, basePermission " +
                                                           "FROM dbo.Full_User_Board WHERE userId=@userId " +
                                                           "ORDER BY boardId " +
                                                           "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        private static readonly string SELECT_SEARCH_BOARDS = "SELECT permission, boardId, boardName, maxDistPoints, basePermission " +
                                                              "FROM dbo.Full_User_Board WHERE userId=@userId " +
                                                              "WHERE CONTAINS(boardName, @search) " +
                                                              "ORDER BY boardId " +
                                                              "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        private static readonly string SELECT_USER_BOARD = "SELECT permission, boardId, userId FROM dbo.User_Board WHERE boardId=@boardId and userId=@userId";
        private static readonly string SELECT_USER = "SELECT permission, userId FROM dbo.Full_User_Board WHERE boardId=@boardId and userId=@userId";
        private static readonly string SELECT_BOARD = "SELECT permission, boardId, boardName, maxDistPoints, basePermission FROM dbo.Full_User_Board WHERE userId=@userId and boardId=@boardId";
        private static readonly string SELECT_PERMISSION = "SELECT [dbo].[GetPermission](@userId, @boardId)";
        private static readonly string INSERT_USER_BOARD = "INSERT INTO dbo.User_Board (userId, boardId, permission) VALUES (@userId, @boardId, @permission)";
        private static readonly string DELETE_USER_BOARD = "DELETE FROM dbo.User_Board WHERE userId = @userId and boardId = @boardId";
        private static readonly string UPDATE_USER_BOARD = "UPDATE dbo.User_Board " +
                                                           "SET permission= @permission " +
                                                           "WHERE userId = @userId and boardId = @boardId";

        //Extract Data From Data Reader
        private static UserBoard_User GetUser(SqlDataReader dr) {
            return new UserBoard_User() {
                Permission = (BoardPermission)dr.GetByte(0),
                User = new User() {
                    User_id = dr.GetString(1)
                }
            };
        }

        private static UserBoard_Board GetBoard(SqlDataReader dr) {
            return new UserBoard_Board() {
                Permission = (BoardPermission)dr.GetByte(0),
                Board = new Board {
                    Id = dr.GetInt64(1),
                    Name = dr.GetString(2),
                    MaxDistPoints = dr.GetByte(3),
                    BasePermission = (BoardPermission)dr.GetByte(4) 
                }
            };
        }

        private static UserBoard GetUserBoard(SqlDataReader dr) {
            return new UserBoard() {
                Permission = (BoardPermission)dr.GetByte(0),
                BoardId = dr.GetInt64(1),
                UserId = dr.GetString(2)
            };
        }
    }
}
