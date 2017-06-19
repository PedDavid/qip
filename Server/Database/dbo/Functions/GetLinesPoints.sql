CREATE FUNCTION dbo.GetLinesPoints
(
	@boardId bigint
)
RETURNS TABLE 
AS 
	RETURN(
			select fig.boardId, fig.id, point.x as linePointX, point.y as linePointY, figP.pointIdx as linePointIdx, pStyle.pointStyleId, pStyle.width as pointWidth
			from dbo.Figure as fig 
				inner join dbo.Line as lin
					on(lin.figureId = fig.id and lin.boardId = @boardId)
				inner join dbo.Line_Point as figP
					on (figP.figureId = lin.figureId and figP.boardId = @boardId)
				inner join dbo.Point as point
						on(figP.pointId = point.id)
				inner join dbo.PointStyle as pStyle
						on(figP.pointStyleId = pStyle.pointStyleId)						
		)