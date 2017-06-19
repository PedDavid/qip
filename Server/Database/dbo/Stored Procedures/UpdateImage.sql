/*
*
*
* Update Image Procedure
*
*/

create proc dbo.UpdateImage @boardId bigint, @figureId bigint, @x int = null, @y int = null, @width int = null, @height int = null, @src nvarchar(max) = null
as
	begin try
		set transaction isolation level serializable
		begin tran
			
			declare @pointId bigint = null
			if @x is not null or @y is not null
			begin
				select TOP(1) @pointId = id from dbo.Point where x = @x and y = @y
				if @pointId is null
				begin
					insert into dbo.Point(x, y) values(@x, @y)
					set @pointId = SCOPE_IDENTITY()
				end
			end

			update dbo.[Image] 
				set width= isnull(@width, width), height=isnull(@height, height), src=isnull(@src, src), initPointId=isnull(@pointId, initPointId)
				where boardId=@boardId and figureId=@figureId

			commit
	end try
	begin catch
		if @@TRANCOUNT <> 0
			rollback;
		throw
	end catch

	--drop proc dbo.UpdateImage