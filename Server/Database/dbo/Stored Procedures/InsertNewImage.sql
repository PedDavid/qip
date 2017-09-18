create proc dbo.InsertNewImage @figureId bigint, @boardId bigint, @x int, @y int, @src nvarchar(1024), @width int, @height int
AS
SET NOCOUNT ON;
begin try
	set transaction isolation level REPEATABLE READ
	begin tran
		INSERT INTO dbo.Point(x, y) SELECT @x, @y WHERE NOT EXISTS(SELECT * FROM dbo.Point WHERE x=@x and y=@y)

		DECLARE @pointId BIGINT
		IF(@@ROWCOUNT <> 0)
        BEGIN
			SET @pointId = SCOPE_IDENTITY()
		END
		ELSE
        BEGIN
			SELECT @pointId = id FROM dbo.Point WHERE x=@x and y=@y
		END
			
		exec dbo.InsertNewImageWithPoint @figureId, @boardId, @pointId, @src, @width, @height
	commit
end try
begin catch
	if @@TRANCOUNT <> 0
		rollback;
	throw
end catch