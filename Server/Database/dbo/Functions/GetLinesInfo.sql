CREATE FUNCTION dbo.GetLinesInfo( @boardId bigint)
RETURNS TABLE 
AS 
	RETURN(
		select fig.boardId, fig.id, lin.isClosedForm, fStyle.lineStyleId, fStyle.color as lineColor
			from dbo.Figure as fig 
				inner join dbo.Line as lin
					on(lin.boardId=fig.boardId and lin.figureId = fig.id)
				inner join dbo.LineStyle as fStyle
					on (fStyle.lineStyleId = lin.lineStyleId)
			where (fig.boardId = @boardId)
	)

-- drop function GetLinesInfo