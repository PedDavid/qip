/*
*
*
* Update Line Procedure
*
*/
create proc dbo.UpdateLine @boardId bigint, @figureId bigint, @color varchar(30) = null, @points dbo.Points readonly, @isClosedForm bit = 0
as
	begin try
		set transaction isolation level serializable
		begin tran

			if exists(select * from @points)begin

				-- verificar se o estilo dos pontos é um json válido
				declare @pointStyle varchar(max)
				select @pointStyle=pointStyle from @points
				if(ISJSON(@pointStyle) < 1)
					throw 51000, 'Point Style is not a valid JSON', 1;  


				--filtrar os pontos que já existem e adicionar os restantes à tabela de Point
				insert into dbo.Point(x, y) select newPoints.x, newPoints.y 
												from @points as newPoints
												except 
												select pointTable.x, pointTable.y
													from dbo.Point as pointTable
				
				--apagar todos os tuplos de Figure_Point que pertencem à linha pois é provável que todos os pontos estejam mudados
				delete from dbo.Line_Point
					where figureId=@figureId

				--inserir na tabela Figure_Point os pontos associados à linha inserida, assim como o estilo de cada ponto
				insert into dbo.Line_Point (figureId, boardId, pointId, pointIdx, pointStyle)
										select @figureId, @boardId, points.id, figPoints.idx, figPoints.pointStyle
											from dbo.Point as points
											inner join @points as figPoints
											on(points.x=figPoints.x and points.y = figPoints.y)

			end			
		

			update dbo.LineStyle 
				set color = isnull(@color, color)
				where lineStyleId = (select lineStyleId from dbo.Line where @boardId=boardId and @figureId = figureId)

			update dbo.Line set isClosedForm = @isClosedForm

		commit
	end try
	begin catch
		if @@TRANCOUNT <> 0
			rollback;
		throw
	end catch

--	drop proc UpdateLine