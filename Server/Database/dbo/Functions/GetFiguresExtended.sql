﻿create function dbo.GetFiguresExtended(@boardId bigint) returns table
as
	return(
		select fig.boardId, fig.id, fig.figureType, lineX.isClosedForm, lineX.color as lineColor, lineX.linePointX, lineX.linePointY, lineX.pointWidth,
			img.initPointId, img.src, img.width as imageWidth, img.height as imageHeight
			from dbo.Figure as fig 
				full outer join dbo.[Image] as img
					on(img.figureId = fig.id and img.boardId = @boardId)
				full outer join 
				(select lin.figureId, lin.boardId, lin.isClosedForm, fStyle.color, point.x as linePointX, point.y as linePointY, pStyle.width as pointWidth 
					from dbo.Line as lin
					inner join dbo.LineStyle as fStyle
						on (fStyle.lineStyleId = lin.lineStyleId)
					inner join dbo.Line_Point as figP
						on (figP.figureId = lin.figureId and figP.boardId = @boardId)
							inner join dbo.Point as point
								on(figP.pointId = point.id)
							inner join dbo.PointStyle as pStyle
								on(figP.pointStyleId = pStyle.pointStyleId)
						
				) as lineX
				on(lineX.figureId = fig.id and lineX.boardId = @boardId)
	)