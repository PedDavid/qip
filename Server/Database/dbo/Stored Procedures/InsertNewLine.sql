create proc dbo.InsertNewLine @figureId bigint, @boardId bigint, @color varchar(20), @points dbo.Points readonly, @isClosedForm bit = 0
AS
SET NOCOUNT ON;
begin try
	set transaction isolation level REPEATABLE READ
	begin tran
			
		-- verificar se o estilo dos pontos é um json válido
		declare @invalidJson int
		SELECT @invalidJson = COUNT(*) FROM @points WHERE ISJSON(pointStyle) <> 1
		if(@invalidJson <> 0)
			throw 51000, 'Point Style is not a valid JSON', 1; 


		INSERT INTO dbo.LineStyle(color) SELECT @color WHERE NOT EXISTS(SELECT * FROM dbo.LineStyle WHERE color = @color)

        DECLARE @styleId BIGINT = SCOPE_IDENTITY()
		IF(@@ROWCOUNT = 0)
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

		--inserir na tabela Figure_Point os pontos associados à figura inserida, assim como o estilo de cada ponto
		insert into dbo.Line_Point (figureId, boardId, pointId, pointIdx, pointStyle)
									select @figureId, @boardId, points.id, figPoints.idx, figPoints.pointStyle
										from dbo.Point as points
										inner join @points as figPoints
										on(points.x=figPoints.x and points.y = figPoints.y)


	commit
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch

	-- drop proc dbo.InsertNewLine