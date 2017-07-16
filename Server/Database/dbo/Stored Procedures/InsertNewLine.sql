create proc dbo.InsertNewLine @figureId bigint, @boardId bigint, @color varchar(30), @points dbo.Points readonly
as
	begin try
		set transaction isolation level serializable
		begin tran
			
			-- verificar se o estilo dos pontos é um json válido
			declare @pointStyle varchar(max)
			select @pointStyle=pointStyle from @points
			if(ISJSON(@pointStyle) < 1)
				throw 51000, 'Point Style is not a valid JSON', 1;  

			--verificar se já existe um objeto lineStyle com width dada. se já existir, é possível reutilizar
			declare @styleId bigint
			select @styleId=lineStyleId from dbo.LineStyle where color = @color
			if @styleId is null
			begin
				insert into dbo.LineStyle values(@color)
				set @styleId = SCOPE_IDENTITY() --último id introduzido no scope atual
			end

			insert into dbo.Figure (id, figureType, boardId) values(@figureId, 'line', @boardId)

			insert into dbo.Line (figureId, boardId, lineStyleId) values (@figureId, @boardId, @styleId)
	
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