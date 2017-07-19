create function dbo.GetFigures(@boardId bigint) returns table
as
	return(
		select fig.boardId, fig.id, fig.figureType, lineX.isClosedForm, lineX.lineStyleId,
			img.initPointId, img.src, img.width, img.height 
			from dbo.Figure as fig 
				full outer join dbo.[Image] as img
				on(img.figureId = fig.id and fig.boardId = @boardId)
				full outer join 
				(select lin.figureId, lin.boardId, lin.isClosedForm, lin.lineStyleId from dbo.Line as lin) as lineX
				on(lineX.figureId = fig.id and fig.boardId = @boardId)
			where(fig.boardId = @boardId)
	)

-- drop function dbo.GetFigures