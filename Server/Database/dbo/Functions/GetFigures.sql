create function dbo.GetFigures(@boardId bigint) returns table
as
	return(
		select fig.boardId, fig.id, fig.figureType, lineX.isClosedForm, lineX.lineStyleId, lineX.pointId, lineX.pointStyle,
			img.initPointId, img.src, img.width, img.height 
			from dbo.Figure as fig 
				full outer join dbo.[Image] as img
				on(img.figureId = fig.id and fig.boardId = 1)
				full outer join 
				(select lin.figureId, lin.boardId, lin.isClosedForm, lin.lineStyleId, figP.pointId, figP.pointStyle from dbo.Line as lin
					inner join dbo.Line_Point as figP
					on (figP.figureId = lin.figureId and figP.boardId = @boardId) ) as lineX
				on(lineX.figureId = fig.id and fig.boardId = @boardId)
	)