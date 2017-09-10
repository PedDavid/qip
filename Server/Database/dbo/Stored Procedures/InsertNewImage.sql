create proc dbo.InsertNewImage @figureId bigint, @boardId bigint, @x int, @y int, @src nvarchar(1024), @width int, @height int
as
	begin try
		set transaction isolation level REPEATABLE READ
		begin tran
			declare @pointId bigint
			select @pointId = id from dbo.Point where x=@x and y=@y
			--verificar se o ponto já existe
			--caso exista chamar proc com o id 
			if(@pointId is null) 
			BEGIN
				insert into dbo.Point (x, y) values(@x, @y)
				SET @pointId = SCOPE_IDENTITY()
			end
			--caso não exista, criar e chamar proc com o id 
			
			exec dbo.InsertNewImageWithPoint @figureId, @boardId, @pointId, @src, @width, @height
		commit
	end try
	begin catch
		if @@TRANCOUNT <> 0
			rollback;
		throw
	end catch