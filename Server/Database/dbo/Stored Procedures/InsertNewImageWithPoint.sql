create proc dbo.InsertNewImageWithPoint @figureId bigint, @boardId bigint, @pointId bigint, @src nvarchar(max), @width int, @height int 
AS
SET NOCOUNT ON;
begin try
	begin tran

		insert into dbo.Figure (id, figureType, boardId) values(@figureId, 'image', @boardId)

		insert into dbo.[Image] (figureId, boardId, initPointId, src, width, height) values (@figureId, @boardId, @pointId, @src, @width, @height)

	commit
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch