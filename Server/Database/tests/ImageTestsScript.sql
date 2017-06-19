/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


--teste com ponto criado
GO
begin tran
	declare @figureId int = 1
	
	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.Point values(10, 10)
	declare @pointId int = IDENT_CURRENT('dbo.Point')
	
	exec dbo.InsertNewImageWithPoint @figureId=@figureId, @boardId=@boardIdx, @pointId=@pointId, @src='srcExample', @width=200, @height=200

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.[Image] where figureId = @figureId and boardId = @boardIdx)
		set @testResult = @testResult+1

	if exists (select * from dbo.Point where id=@pointId)
		set @testResult = @testResult+1

	if @testResult = 4
		print 'Test Passed'
	else
		print 'Test Failed'

rollback


--teste com ponto não criado
GO
begin tran
	declare @figureId int = 1
	
	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	exec dbo.InsertNewImage @figureId, @boardIdx, 10, 10, 'srcExample', 200, 200

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.[Image] where figureId = @figureId and boardId = @boardIdx)
		set @testResult = @testResult+1

	if exists (select * from dbo.Point where x=10 and y=10)
		set @testResult = @testResult+1

	declare @pointId int
	select @pointId=id from dbo.Point where x=10 and y=10


	if @testResult = 4
		print 'Test Passed'
	else
		print 'Test Failed'

rollback

--check if removes only one figure when more than one figure have de sourcePoint in common
GO
begin tran
	declare @figureId int = 1
	
	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.Point values(10, 10)
	declare @pointId int = IDENT_CURRENT('dbo.Point')

	exec dbo.InsertNewImageWithPoint @figureId, @boardIdx, @pointId, 'srcExample', 200, 200

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.[Image] where figureId = @figureId and width=200 and height=200)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.Point where id=@pointId and x=10 and y=10)
		set @testResult = @testResult+1

	exec dbo.UpdateImage @boardId=@boardIdx, @figureId=@figureId, @width=400, @height=400

	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.[Image] where figureId = @figureId and width=400 and height=400)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.Point where id=@pointId and x=10 and y=10)
		set @testResult = @testResult+1


	if @testResult = 8
		print 'Test Passed'
	else
		print 'Test Failed'

rollback


--test when image change position

GO

begin tran
	declare @figureId int = 1
	
	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.Point values(10, 10)
	declare @pointId int = IDENT_CURRENT('dbo.Point')

	exec dbo.InsertNewImageWithPoint @figureId, @boardIdx, @pointId, 'srcExample', 200, 200

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.[Image] where figureId = @figureId and width=200 and height=200)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.Point where id=@pointId and x=10 and y=10)
		set @testResult = @testResult+1

	insert into dbo.Point values(20, 20)
	declare @updatedPointId int = IDENT_CURRENT('dbo.Point')

	exec dbo.UpdateImage @boardIdx, @figureId, @updatedPointId

	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.[Image] where figureId = @figureId and width=200 and height=200)
		set @testResult = @testResult+1
	--

	if exists (select * from dbo.Point where id=@updatedPointId and x=20 and y=20)
		set @testResult = @testResult+1

	if @testResult = 8
		print 'Test Passed'
	else
		print 'Test Failed.'

rollback

GO


--test when image change position and other parameters

GO

begin tran
	declare @figureId int = 1
	
	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.Point values(10, 10)
	declare @pointId int = IDENT_CURRENT('dbo.Point')

	exec dbo.InsertNewImageWithPoint @figureId, @boardIdx, @pointId, 'srcExample', 200, 200

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1
	
	if exists (select * from dbo.[Image] where figureId = @figureId and width=200 and height=200 and src='srcExample')
		set @testResult = @testResult+1
	
	if exists (select * from dbo.Point where id=@pointId and x=10 and y=10)
		set @testResult = @testResult+1

	insert into dbo.Point values(20, 20)
	declare @updatedPointId int = IDENT_CURRENT('dbo.Point')

	exec dbo.UpdateImage @boardIdx, @figureId, @updatedPointId, @width=400, @height=400, @src='srcExampleUpdated'

	if exists (select * from dbo.[Image] where @boardIdx=boardId and @figureId=figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.Point where id=@updatedPointId and x=20 and y=20)
		set @testResult = @testResult+1

	--other parameters
	if exists (select * from dbo.[Image] where figureId = @figureId and width=400 and height=400 and src='srcExampleUpdated')
		set @testResult = @testResult+1

	if @testResult = 8
		print 'Test Passed'
	else
		print 'Test Failed.'

rollback


--get images test
GO

begin tran
	declare @figureId bigint = 1

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	exec dbo.InsertNewImage @figureId=@figureId, @boardId=@boardIdx, @x=10, @y=10, @src='src', @width=10, @height=10 
	
	declare @testResult int = 0

	if exists (select * from dbo.GetImages(@boardIdx) where boardId = @boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if exists (select id, x, y from dbo.Point where id = (select initPointId from dbo.GetImages(@boardIdx)) and x = 10 and y = 10)
		set @testResult = @testResult+1

	if ((select src from dbo.GetImages(@boardIdx)) = 'src')
		set @testResult = @testResult+1
	
	if exists (select * from dbo.GetImages(@boardIdx) where imageWidth = 10 and imageHeight = 10)
		set @testResult = @testResult+1

	select * from dbo.GetImages(@boardIdx)

	if @testResult = 4
		print 'Test Passed'
	else
		print 'Test Failed'

rollback