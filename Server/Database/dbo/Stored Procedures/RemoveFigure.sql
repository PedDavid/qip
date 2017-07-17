create proc dbo.RemoveFigure @boardId bigint, @figureId bigint
as
	begin try
		set transaction isolation level serializable
		begin tran
			declare @figType varchar(5)
			select @figType=figureType from dbo.Figure where boardId=@boardId and id=@figureId
			
			declare @figStyleId bigint
			--apagar a figura específica			
			if(@figType='line') begin
				--apagar primeiro o FigureStyle associado à Line
				select @figStyleId = lineStyleId from dbo.Line where figureId = @figureId
				delete from dbo.Line where boardId=@boardId and figureId = @figureId
				delete from dbo.LineStyle where lineStyleId = @figStyleId
			end
			else 
				delete from dbo.[Image] where boardId=@boardId and figureId = @figureId

			--apagar a associação entre pontos e a figura a eliminar - caso seja uma imagem isto não faz nada
			delete from dbo.Line_Point where boardId=@boardId and figureId = @figureId

			--apagar a figura geral
			delete from dbo.Figure where boardId=@boardId and id=@figureId

		commit
	end try
	begin catch
		if @@TRANCOUNT <> 0
			rollback;
		throw
	end catch