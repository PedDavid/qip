create function dbo.GetImages(@boardId bigint) returns table
as
	return(
		select fig.boardId, fig.id, img.initPointId, point.x as pointX, point.y as pointY, img.src, img.width as imageWidth, img.height as imageHeight
			from dbo.Figure as fig 
			inner join dbo.[Image] as img
				on(img.figureId = fig.id and img.boardId = @boardId)
			inner join dbo.Point as point
				on point.id = img.initPointId
	)