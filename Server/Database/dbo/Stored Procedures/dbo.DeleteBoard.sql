CREATE PROCEDURE [dbo].[DeleteBoard]
	@id BIGINT
AS
begin try
	begin TRAN
		DELETE FROM dbo.User_Board WHERE boardId = @id

		DELETE FROM dbo.[Image] WHERE boardId = @id

		DELETE FROM dbo.Line_Point WHERE boardId = @id

		DELETE FROM dbo.Line WHERE boardId = @id

		DELETE FROM dbo.Figure WHERE boardId = @id

		DELETE FROM dbo.Board WHERE id = @id
	COMMIT
	RETURN 0
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch
