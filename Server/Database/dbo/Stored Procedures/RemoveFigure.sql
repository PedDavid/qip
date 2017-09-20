create proc dbo.RemoveFigure @boardId bigint, @figureId bigint
AS
SET NOCOUNT ON;
begin try
	set transaction isolation level READ COMMITTED
	begin TRAN
		--apagar a associação entre pontos e a figura a eliminar - caso seja uma imagem isto não faz nada
		delete from dbo.Line_Point where boardId=@boardId and figureId = @figureId

		declare @figType varchar(5)
		select @figType=figureType from dbo.Figure where boardId=@boardId and id=@figureId
			
		declare @figStyleId bigint
		--apagar a figura específica			
		if(@figType='line') begin
			--apagar primeiro o FigureStyle associado à Line
			delete from dbo.Line where boardId=@boardId and figureId = @figureId
		end
		else 
			delete from dbo.[Image] where boardId=@boardId and figureId = @figureId

		--apagar a figura geral
		delete from dbo.Figure where boardId=@boardId and id=@figureId

	commit
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch

-- drop proc dbo.RemoveFigure