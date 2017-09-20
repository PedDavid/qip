create proc dbo.InsertNewLine @figureId bigint, @boardId bigint, @color varchar(20), @points dbo.Points readonly, @isClosedForm bit = 0
AS
SET NOCOUNT ON;
begin try
	set transaction isolation level REPEATABLE READ
	begin tran
		INSERT INTO dbo.LineStyle(color) SELECT @color WHERE NOT EXISTS(SELECT * FROM dbo.LineStyle WHERE color = @color)

        DECLARE @styleId BIGINT
		IF(@@ROWCOUNT <> 0)
		BEGIN
			SET @styleId = SCOPE_IDENTITY()
        END
		ELSE
        BEGIN
			SELECT @styleId = lineStyleId FROM dbo.LineStyle WHERE color = @color
		END

		insert into dbo.Figure (id, figureType, boardId) values(@figureId, 'line', @boardId)

		insert into dbo.Line (figureId, boardId, isClosedForm, lineStyleId) values (@figureId, @boardId, @isClosedForm, @styleId)
	
		--filtrar os pontos que já existem e adicionar os restantes à tabela de Point
		insert into dbo.Point(x, y) select figPoints.x, figPoints.y 
										from @points as figPoints
										except 
										select points.x, points.y 
										from dbo.Point points

		INSERT INTO dbo.PointStyle(width) 
		SELECT figPoints.pointStyleWidth AS width
		FROM @points AS figPoints
		EXCEPT
		select styles.width
		FROM dbo.PointStyle styles

		--inserir na tabela Figure_Point os pontos associados à figura inserida, assim como o estilo de cada ponto
		insert into dbo.Line_Point (figureId, boardId, pointId, pointIdx, pointStyleId)
									select @figureId, @boardId, points.id, figPoints.idx, ps.id
										from dbo.Point as points
										inner join @points as figPoints
										on(points.x=figPoints.x and points.y = figPoints.y)
										INNER JOIN dbo.PointStyle AS ps
										ON(figPoints.pointStyleWidth = ps.width)


	commit
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch

	-- drop proc dbo.InsertNewLine