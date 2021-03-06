﻿CREATE PROCEDURE [dbo].[InsertBoard]
	@name VARCHAR(100),
	@maxDistPoints TINYINT,
	@basePermission TINYINT = 0,
	@userId VARCHAR(128),
	@boardId BIGINT OUT
AS
SET NOCOUNT ON;
begin try
	begin TRAN
		INSERT INTO dbo.Board([name], maxDistPoints, basePermission) VALUES(@name, @maxDistPoints, @basePermission)

		SET @boardId = SCOPE_IDENTITY()

		IF(@userId IS NOT NULL)
			INSERT INTO dbo.User_Board(userId, boardId, permission) VALUES(@userId, @boardId, 3)
	COMMIT
	RETURN 0
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch