using API.Models.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Repositories {
    public class UsersBoardsRepository : IUsersBoardsRepository {
        private readonly SqlServerTemplate _queryTemplate;

        public UsersBoardsRepository(SqlServerTemplate queryTemplate) {
            _queryTemplate = queryTemplate;
        }

        public void Add(UserBoard userBoard) {
            try {
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

                _queryTemplate.Query(INSERT_USER_BOARD, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public UserBoard Find(long boardId, long userId) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

                return _queryTemplate.QueryForObject(SELECT_USER_BOARD, parameters, GetUserBoard);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public UserBoard_Board FindBoard(long userId, long boardId) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

                return _queryTemplate.QueryForObject(SELECT_BOARD, parameters, GetBoard);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public UserBoard_User FindUser(long boardId, long userId) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

                return _queryTemplate.QueryForObject(SELECT_USER, parameters, GetUser);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public IEnumerable<UserBoard_Board> GetAllBoards(long userId) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

                return _queryTemplate.Query(SELECT_ALL_BOARDS, parameters, GetBoard);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public IEnumerable<UserBoard_User> GetAllUsers(long boardId) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                return _queryTemplate.Query(SELECT_ALL_USERS, parameters, GetUser);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void Remove(long boardId, long userId) {
            try {
                List<SqlParameter> parameters = new List<SqlParameter>();

                parameters.Add("@userId", SqlDbType.BigInt).Value = userId;

                parameters.Add("@boardId", SqlDbType.BigInt).Value = boardId;

                _queryTemplate.Query(DELETE_USER_BOARD, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
        }

        public void Update(UserBoard userBoard) {
            try {
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

                _queryTemplate.Query(UPDATE_USER_BOARD, parameters);
            }
            catch(Exception ex) {
                Console.WriteLine("E R R O : " + ex.Message);
                throw;//TODO alterar
            }
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
