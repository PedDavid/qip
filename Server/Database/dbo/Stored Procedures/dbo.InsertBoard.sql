CREATE PROCEDURE [dbo].[InsertBoard]
	@name VARCHAR(100),
	@maxDistPoints TINYINT,
	@basePermission TINYINT = 0,
	@userId VARCHAR(128),
	@boardId BIGINT OUT
AS
begin try
	begin TRAN
		INSERT INTO dbo.Board([name], maxDistPoints) VALUES(@name, @maxDistPoints)

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