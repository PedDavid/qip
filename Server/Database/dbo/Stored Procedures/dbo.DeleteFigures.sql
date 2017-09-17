CREATE PROCEDURE [dbo].[DeleteFigures]
	@boardId BIGINT,
	@lastFigureToDelete BIGINT
AS
SET NOCOUNT ON;
BEGIN TRY
	BEGIN TRANSACTION
		DELETE FROM dbo.Line WHERE boardId = @boardId AND figureId <= @lastFigureToDelete

		DELETE FROM dbo.[Image] WHERE boardId = @boardId AND figureId <= @lastFigureToDelete

		DELETE FROM dbo.Figure WHERE boardId = @boardId AND id <= @lastFigureToDelete
	COMMIT
END TRY
BEGIN CATCH
	IF(@@TRANCOUNT <> 0)
		ROLLBACK;
	THROW
END CATCH
