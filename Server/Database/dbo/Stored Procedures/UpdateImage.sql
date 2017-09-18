/*
*
*
* Update Image Procedure
*
*/

create proc dbo.UpdateImage @boardId bigint, @figureId bigint, @x int = null, @y int = null, @width int = null, @height int = null, @src nvarchar(max) = null
AS
SET NOCOUNT ON;
begin try
	set transaction isolation level serializable
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