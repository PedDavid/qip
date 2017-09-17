namespace QIP.Public {
    public class LoggingEvents {
        #region Events BoardsController
        public const int ListBoards = 0;
        public const int GetBoard = 1;
        public const int InsertBoard = 2;
        public const int UpdateBoard = 3;
        public const int DeleteBoard = 4;

        public const int GetBoardNotAuthorized = 5;
        public const int UpdateBoardNotAuthorized = 6;
        public const int DeleteBoardNotAuthorized = 7;

        public const int InsertBoardWithoutBody = 8;
        public const int UpdateBoardWithoutBody = 9;
        public const int UpdateBoardWrongId = 10;

        public const int GetBoardNotFound = 11;
        public const int UpdateBoardNotFound = 12;
        public const int DeleteBoardNotFound = 13;
        #endregion

        #region Events UsersController
        public const int ListUsers = 14;
        public const int ListUserBoards = 15;
        public const int GetUserBoard = 16;

        public const int ListUserBoardsNotAuthorized = 17;
        public const int GetUserBoardNotAuthorized = 18;

        public const int GetUserBoardNotFound = 19;
        #endregion

        #region Events PreferencesController
        public const int GetPreferences = 20;
        public const int InsertPreferences = 21;
        public const int UpdatePreferences = 22;
        public const int DeletePreferences = 23;

        public const int GetPreferencesNotAuthorized = 24;
        public const int InsertUpdatePreferencesNotAuthorized = 25;
        public const int DeletePreferencesNotAuthorized = 26;

        public const int GetPreferencesNotFound = 27;
        public const int DeletePreferencesNotFound = 28;

        public const int InsertUpdatePreferencesWithoutBody = 29;
        public const int InsertUpdatePreferencesWrongId = 30;
        #endregion

        #region Events ImagesController
        public const int ListImages = 31;
        public const int GetImage = 32;
        public const int InsertImage = 33;
        public const int UpdateImage = 34;
        public const int DeleteImage = 35;

        public const int ListImagesNotAuthorized = 36;
        public const int GetImageNotAuthorized = 37;
        public const int InsertImageNotAuthorized = 38;
        public const int UpdateImageNotAuthorized = 39;
        public const int DeleteImageNotAuthorized = 40;

        public const int InsertImageWithoutBody = 41;
        public const int UpdateImageWithoutBody = 42;

        public const int InsertImageWrongId = 43;
        public const int UpdateImageWrongId = 44;
        public const int InsertImageWrongBoardId = 45;
        public const int UpdateImageWrongBoardId = 46;

        public const int GetImageNotFound = 47;
        public const int UpdateImageNotFound = 48;
        public const int DeleteImageNotFound = 49;
        #endregion

        #region Events LinesController
        public const int ListLines = 50;
        public const int GetLine = 51;
        public const int InsertLine = 52;
        public const int UpdateLine = 53;
        public const int DeleteLine = 54;

        public const int ListLinesNotAuthorized = 55;
        public const int GetLineNotAuthorized = 56;
        public const int InsertLineNotAuthorized = 57;
        public const int UpdateLineNotAuthorized = 58;
        public const int DeleteLineNotAuthorized = 59;

        public const int InsertLineWithoutBody = 60;
        public const int UpdateLineWithoutBody = 61;

        public const int InsertLineWrongId = 62;
        public const int UpdateLineWrongId = 63;
        public const int InsertLineWrongBoardId = 64;
        public const int UpdateLineWrongBoardId = 65;

        public const int GetLineNotFound = 66;
        public const int UpdateLineNotFound = 67;
        public const int DeleteLineNotFound = 68;
        #endregion

        #region Events LineStyleController
        public const int ListLineStyles = 69;
        public const int GetLineStyle = 70;
        public const int InsertLineStyle = 71;
        public const int UpdateLineStyle = 72;
        public const int DeleteLineStyle = 73;

        public const int InsertLineStyleWithoutBody = 74;
        public const int UpdateLineStyleWithoutBody = 75;
        public const int UpdateLineStyleWrongId = 76;

        public const int GetLineStyleNotFound = 77;
        public const int UpdateLineStyleNotFound = 78;
        public const int DeleteLineStyleNotFound = 79;
        #endregion

        #region Events UsersBoardsController
        public const int ListBoardUsers = 80;
        public const int GetBoardUser = 81;
        public const int InsertUserBoard = 82;
        public const int UpdateUserBoard = 83;
        public const int DeleteUserBoard = 84;

        public const int ListBoardUsersNotAuthorized = 85;
        public const int GetBoardUserNotAuthorized = 86;
        public const int InsertUserBoardNotAuthorized = 87;
        public const int UpdateUserBoardNotAuthorized = 88;
        public const int DeleteUserBoardNotAuthorized = 89;

        public const int InsertUserBoardWithoutBody = 90;
        public const int UpdateUserBoardWithoutBody = 91;
        public const int InsertUserBoardWrongBoardId = 92;
        public const int UpdateUserBoardWrongBoardId = 93;
        public const int UpdateUserBoardWrongUserId = 94;

        public const int GetBoardUserNotFound = 95;
        public const int UpdateUserBoardNotFound = 96;
        public const int DeleteUserBoardNotFound = 97;
        #endregion

        #region Events WebSocketsController
        public const int ConnectWebSocket = 80;
        public const int ConnectWebSocketNotAuthorized = 82;
        public const int ConnectWebSocketNotFound = 81;
        public const int ConnectWebSocketWrongProtocol = 83;
        #endregion

        #region Events ImageOperations
        public const int InsertWSImage = 84;
        public const int UpdateWSImage = 85;
        public const int DeleteWSImage = 86;

        public const int InsertWSImageNotAuthorized = 87;
        public const int UpdateWSImageNotAuthorized = 88;
        public const int DeleteWSImageNotAuthorized = 89;

        public const int InsertWSImageWrongBoardId = 90;
        public const int UpdateWSImageWrongBoardId = 91;

        public const int InsertWSImageInvalidModel = 92;
        public const int UpdateWSImageInvalidModel = 93;
        public const int DeleteWSImageInvalidModel = 94;

        public const int UpdateWSImageNotFound = 95;
        public const int DeleteWSImageNotFound = 96;

        public const int InsertWSImageUnexpectedServiceError = 97;
        public const int UpdateWSImageUnexpectedServiceError = 98;
        public const int DeleteWSImageUnexpectedServiceError = 99;

        public const int InsertWSImageUnexpectedError = 100;
        public const int UpdateWSImageUnexpectedError = 101;
        public const int DeleteWSImageUnexpectedError = 102;
        #endregion

        #region Events LineOperations
        public const int InsertWSLine = 103;
        public const int UpdateWSLine = 104;
        public const int DeleteWSLine = 105;

        public const int InsertWSLineNotAuthorized = 106;
        public const int UpdateWSLineNotAuthorized = 107;
        public const int DeleteWSLineNotAuthorized = 108;

        public const int InsertWSLineWrongBoardId = 109;
        public const int UpdateWSLineWrongBoardId = 110;

        public const int InsertWSLineInvalidModel = 111;
        public const int UpdateWSLineInvalidModel = 112;
        public const int DeleteWSLineInvalidModel = 113;

        public const int UpdateWSLineNotFound = 114;
        public const int DeleteWSLineNotFound = 115;

        public const int InsertWSLineUnexpectedServiceError = 116;
        public const int UpdateWSLineUnexpectedServiceError = 117;
        public const int DeleteWSLineUnexpectedServiceError = 118;

        public const int InsertWSLineUnexpectedError = 119;
        public const int UpdateWSLineUnexpectedError = 120;
        public const int DeleteWSLineUnexpectedError = 121;
        #endregion

        #region Events PointerOperations
        public const int PointToNotAuthorized = 122;
        public const int PointTo = 123;
        public const int PointToInvalidModel = 124;
        #endregion

        #region Events FiguresOperations
        public const int DeleteWSFigures = 125;
        public const int DeleteWSFiguresNotAuthorized = 126;
        public const int DeleteWSFiguresInvalidModel = 127;
        public const int DeleteWSFiguresWrongBoardId = 128;
        public const int DeleteWSFiguresUnexpectedServiceError = 129;
        public const int DeleteWSFiguresUnexpectedError = 130;
        #endregion

        #region Events FiguresController
        public const int DeleteFigures = 131;
        public const int DeleteFiguresNotAuthorized = 132;
        #endregion
    }
}
