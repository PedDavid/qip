﻿CREATE FUNCTION dbo.GetLinesPoints
(
	@boardId bigint
)
RETURNS TABLE 
AS 
	RETURN(
			select fig.boardId, fig.id, point.x as linePointX, point.y as linePointY, figP.pointIdx as linePointIdx, figP.pointStyle as pointStyle
			from dbo.Figure as fig 
				inner join dbo.Line as lin
					on(lin.figureId = fig.id and lin.boardId = fig.boardId)
				inner join dbo.Line_Point as figP
					on (figP.figureId = lin.figureId and figP.boardId = lin.boardId)
				inner join dbo.Point as point
						on(figP.pointId = point.id)	
						WHERE fig.boardId = @boardId
		)