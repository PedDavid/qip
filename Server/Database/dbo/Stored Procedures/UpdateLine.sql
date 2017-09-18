/*
*
*
* Update Line Procedure
*
*/
create proc dbo.UpdateLine @boardId bigint, @figureId bigint, @color varchar(20) = null, @points dbo.Points readonly, @isClosedForm bit = 0
AS
SET NOCOUNT ON;
begin try
	set transaction isolation level REPEATABLE READ
	begin tran

		if exists(select * from @points)begin

			-- verificar se o estilo dos pontos é um json válido
			declare @invalidJson int
			SELECT @invalidJson = COUNT(*) FROM @points WHERE ISJSON(pointStyle) <> 1
			if(@invalidJson <> 0)
				throw 51000, 'Point Style is not a valid JSON', 1;  


			--filtrar os pontos que já existem e adicionar os restantes à tabela de Point
			insert into dbo.Point(x, y) select newPoints.x, newPoints.y 
											from @points as newPoints
											except 
											select pointTable.x, pointTable.y
												from dbo.Point as pointTable
				
			--apagar todos os tuplos de Figure_Point que pertencem à linha pois é provável que todos os pontos estejam mudados
			delete from dbo.Line_Point WHERE figureId = @figureId AND boardId = @boardId

			--inserir na tabela Figure_Point os pontos associados à linha inserida, assim como o estilo de cada ponto
			insert into dbo.Line_Point (figureId, boardId, pointId, pointIdx, pointStyle)
									select @figureId, @boardId, points.id, figPoints.idx, figPoints.pointStyle
										from dbo.Point as points
										inner join @points as figPoints
										on(points.x=figPoints.x and points.y = figPoints.y)

		end			
		
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

		update dbo.Line set isClosedForm = @isClosedForm, lineStyleId = @styleId WHERE figureId = @figureId AND boardId = @boardId

	commit
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch

--	drop proc UpdateLine