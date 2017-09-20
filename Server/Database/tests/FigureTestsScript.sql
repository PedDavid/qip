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


--test remove figure

GO

begin tran
	declare @figureId int = 1

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, pointStyle varchar(max))
	
	insert into dbo.Point values (10, 10)
	insert into dbo.Point values (12, 10)

	insert into dbo.Board values('test_board', 0)
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, N'{"width": 5}')
	insert into @pointsTable values(12, 10, 1, N'{"width": 5}')
	insert into @pointsTable values(16, 10, 3, N'{"width": 5}')
	insert into @pointsTable values(18, 10, 4, N'{"width": 5}')
	insert into @pointsTable values(20, 10, 5, N'{"width": 5}')
	insert into @pointsTable values(14, 10, 2, N'{"width": 5}')

	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable

	exec dbo.RemoveFigure @boardIdx, @figureId

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if not exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if not exists (select * from dbo.Line where figureId = @figureId)
		set @testResult = @testResult+1

	if not  exists (select * from dbo.LineStyle as style inner join dbo.Line as line on style.lineStyleId = line.lineStyleId  and line.figureId = @figureId)
		set @testResult = @testResult+1

	if (select count(figureId) from dbo.Line_Point where figureId = @figureId) = 0
		set @testResult = @testResult+1

	if @testResult = 5
		print 'Test Passed'
	else begin
		rollback;
		throw 51000, 'Test Failed', 1;	
		return;
	end

rollback

--simple removal test
GO

begin tran
	declare @figureId int = 1
	
	insert into dbo.Board values('test_board', 0)
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	exec dbo.InsertNewImage @figureId, @boardIdx, 10, 10, 'srcExample', 200, 200

	exec dbo.RemoveFigure @boardIdx, @figureId

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if not exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if not exists (select * from dbo.[Image] where figureId = @figureId)
		set @testResult = @testResult+1
	
	if @testResult = 3
		print 'Test Passed'
	else begin
		rollback;
		throw 51000, 'Test Failed', 1;	
		return;
	end

rollback

--test
--check if removes only one figure when more than one figure have de sourcePoint in common

GO

begin tran
	declare @figureId int = 1
	declare @figureId2 int = 2
	
	insert into dbo.Board values('test_board', 0)
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.Point values(10, 10)
	declare @pointId int = IDENT_CURRENT('dbo.Point')

	exec dbo.InsertNewImageWithPoint @figureId, @boardIdx, @pointId, 'srcExample', 200, 200
	exec dbo.InsertNewImageWithPoint @figureId2, @boardIdx, @pointId, 'srcExample', 200, 200

	exec dbo.RemoveFigure @boardIdx, @figureId

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if not exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1
	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId2)
		set @testResult = @testResult+1

	if not exists (select * from dbo.[Image] where figureId = @figureId)
		set @testResult = @testResult+1
	if exists (select * from dbo.[Image] where figureId = @figureId2)
		set @testResult = @testResult+1


	if @testResult = 5
		print 'Test Passed'
	else begin
		rollback;
		throw 51000, 'Test Failed', 1;	
		return;
	end

rollback


--get figures extended test
GO

begin tran
	declare @figureId bigint = 1
	declare @figureId2 bigint = 2

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, pointStyleId int)

	insert into dbo.Board values('test_board', 0)
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, N'{"width": 5}')
	insert into @pointsTable values(12, 10, 1, N'{"width": 5}')
	insert into @pointsTable values(16, 10, 3, N'{"width": 5}')
	insert into @pointsTable values(18, 10, 4, N'{"width": 5}')
	insert into @pointsTable values(20, 10, 5, N'{"width": 5}')
	insert into @pointsTable values(14, 10, 2, N'{"width": 5}')
	
	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable
	exec dbo.InsertNewImage @figureId2, @boardIdx, 10, 10, 'srcExample', 200, 200
	
	select * from dbo.GetFiguresExtended(@boardIdx)

	declare @testResult int = 0

	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=1)
		set @testResult = @testResult+1

	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=1 and linePointX=10 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=1 and linePointX=12 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=1 and linePointX=14 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=1 and linePointX=16 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=1 and linePointX=18 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=1 and linePointX=20 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1

	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=2)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetFiguresExtended(@boardIdx) where id=2 and imageWidth=200 and imageHeight=200)
		set @testResult = @testResult+1



	if @testResult = 9
		print 'Test Passed'
	else begin
		rollback;
		throw 51000, 'Test Failed', 1;	
		return;
	end
rollback

