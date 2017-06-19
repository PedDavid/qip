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
--teste básico

GO

begin tran
	declare @figureId int = 1

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, idx int, pointStyleId int)
	
	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)

	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.Line where figureId = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.LineStyle as style inner join dbo.Line as line on style.figureStyleId = line.figureStyleId  and line.figureId = @figureId)
		set @testResult = @testResult+1

	if (select count(figureId) from dbo.Line_Point where figureId = @figureId) = 6
		set @testResult = @testResult+1
	
	if (select count(id) from dbo.Point where x=10 and y=10 or x=12 and y=10 or x=14 and y=10 or x=16 and y=10 or x=18 and y=10 or x=20 and y=10 or y=10 and y=10) = 6
		set @testResult = @testResult+1

	if @testResult = 6
		print 'Test Passed'
	else
		print 'Test Failed'

rollback

--teste com pontos anteriormente inseridos

GO

begin tran
	declare @figureId int = 1

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, idx int, pointStyleId int)
	
	insert into dbo.Point values (5, 10)
	insert into dbo.Point values (0, 10)

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)

	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.Line where figureId = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.LineStyle as style inner join dbo.Line as line on style.figureStyleId = line.figureStyleId  and line.figureId = @figureId)
		set @testResult = @testResult+1

	if (select count(figureId) from dbo.Line_Point where figureId = @figureId) = 6
		set @testResult = @testResult+1
	
	if (select count(id) from dbo.Point where x=5 and y=10 or x=0 and y=10 or x=10 and y=10 or x=12 and y=10 or x=14 and y=10 or x=16 and y=10 or x=18 and y=10 or x=20 and y=10 or y=10 and y=10) = 8
		set @testResult = @testResult+1

	if @testResult = 6
		print 'Test Passed'
	else
		print 'Test Failed'

rollback

--teste com pontos anteriormente inseridos que pertencem também à reta a ser inserida

GO

begin tran
	declare @figureId int = 1

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, idx int, pointStyleId int)
	
	insert into dbo.Point values (10, 10)
	insert into dbo.Point values (12, 10)

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)

	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable

	declare @testResult int = 0
	if exists (select * from dbo.Board where @boardIdx=id)
		set @testResult = @testResult+1

	if exists (select * from dbo.Figure where boardId=@boardIdx and id = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.Line where figureId = @figureId)
		set @testResult = @testResult+1

	if exists (select * from dbo.LineStyle as style inner join dbo.Line as line on style.figureStyleId = line.figureStyleId  and line.figureId = @figureId)
		set @testResult = @testResult+1

	if (select count(figureId) from dbo.Line_Point where figureId = @figureId) = 6
		set @testResult = @testResult+1
	
	if (select count(id) from dbo.Point where x=10 and y=10 or x=12 and y=10 or x=14 and y=10 or x=16 and y=10 or x=18 and y=10 or x=20 and y=10 or y=10 and y=10) = 6
		set @testResult = @testResult+1

	if @testResult = 6
		print 'Test Passed'
	else
		print 'Test Failed'

rollback

--get line points test
GO

begin tran
	declare @figureId bigint = 1
	declare @figureId2 bigint = 2

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, pointStyleId int)

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)
	
	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable
	
	declare @testResult int = 0

	if exists (select * from dbo.GetLinesPoints(@boardIdx) where linePointX=10 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetLinesPoints(@boardIdx) where linePointX=12 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetLinesPoints(@boardIdx) where linePointX=14 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetLinesPoints(@boardIdx) where linePointX=16 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetLinesPoints(@boardIdx) where linePointX=18 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1
	if exists (select * from dbo.GetLinesPoints(@boardIdx) where linePointX=20 and linePointY=10 and pointWidth=5)
		set @testResult = @testResult+1

	select * from dbo.GetLinesPoints(@boardIdx)

	if @testResult = 6
		print 'Test Passed'
	else
		print 'Test Failed'

rollback



--get line info test
GO

begin tran
	declare @figureId bigint = 1

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, pointStyleId int)

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)
	
	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable
	
	declare @testResult int = 0

	if exists (select * from dbo.GetLinesInfo(@boardIdx) where isClosedForm=0 and lineColor='green')
		set @testResult = @testResult+1

	select * from dbo.GetLinesInfo(@boardIdx)

	if @testResult = 1
		print 'Test Passed'
	else
		print 'Test Failed'

rollback


--test when line change position
GO
begin tran
	declare @figureId bigint = 1
	declare @figureId2 bigint = 2

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, pointStyleId int)

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)
	
	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable

	declare @pointsTableUpdated dbo.Points
	insert into @pointsTableUpdated values(12, 15, 0, @pointStyleId)
	insert into @pointsTableUpdated values(14, 15, 1, @pointStyleId)
	insert into @pointsTableUpdated values(16, 15, 2, @pointStyleId)
	insert into @pointsTableUpdated values(18, 15, 3, @pointStyleId)
	insert into @pointsTableUpdated values(20, 15, 4, @pointStyleId)
	insert into @pointsTableUpdated values(22, 15, 5, @pointStyleId)

	exec dbo.UpdateLine @boardId=@boardIdx, @figureId=@figureId, @points=@pointsTableUpdated
	
	declare @testResult int = 0
	
	--verificar que os novos pontos foram inseridos na tabela Point e estão associados à linha
	if((select count(point.id) as counterx from dbo.Point as point
		inner join dbo.Line_Point as figP
		on figP.boardId = @boardIdx and figP.pointId = point.id
		where (x=12 and y=15 and pointIdx=0) or (x=14 and y=15 and pointIdx=1) or (x=16 and y=15 and pointIdx=2) or (x=18 and y=15 and pointIdx=3) or (x=20 and y=15 and pointIdx=4) or (x=22 and y=15 and pointIdx=5) 
		)= 6)
			set @testResult = @testResult+1

	if @testResult = 1
		print 'Test Passed'
	else
		print 'Test Failed'

rollback


--test when line change color
GO
begin tran
	declare @figureId bigint = 1

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, pointStyleId int)

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)
	
	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable

	exec dbo.UpdateLine @boardId=@boardIdx, @figureId=@figureId, @color='red'
	
	declare @testResult int = 0
	
	--verificar se o estilo da linha tem a nova cor
	if exists (select fs.color from dbo.Line lin inner join dbo.LineStyle as fs on lin.figureStyleId=fs.figureStyleId where fs.color='red')
			set @testResult = @testResult+1

	if @testResult = 1
		print 'Test Passed'
	else
		print 'Test Failed'

rollback

--test when line change position and color
GO
begin tran
	declare @figureId bigint = 1
	declare @figureId2 bigint = 2

	if type_id('dbo.Points')  is null
		create type dbo.Points as table(x int, y int, pointStyleId int)

	insert into dbo.Board values('test_board')
	declare @boardIdx int = IDENT_CURRENT('dbo.Board')

	insert into dbo.PointStyle values(5)
	declare @pointStyleId int = IDENT_CURRENT('dbo.PointStyle')
	
	declare @pointsTable dbo.Points
	insert into @pointsTable values(10, 10, 0, @pointStyleId)
	insert into @pointsTable values(12, 10, 1, @pointStyleId)
	insert into @pointsTable values(14, 10, 2, @pointStyleId)
	insert into @pointsTable values(16, 10, 3, @pointStyleId)
	insert into @pointsTable values(18, 10, 4, @pointStyleId)
	insert into @pointsTable values(20, 10, 5, @pointStyleId)
	
	exec dbo.InsertNewLine @figureId=@figureId, @boardId=@boardIdx, @color='green', @points = @pointsTable

	declare @pointsTableUpdated dbo.Points
	insert into @pointsTableUpdated values(12, 15, 0, @pointStyleId)
	insert into @pointsTableUpdated values(14, 15, 1, @pointStyleId)
	insert into @pointsTableUpdated values(16, 15, 2, @pointStyleId)
	insert into @pointsTableUpdated values(18, 15, 3, @pointStyleId)
	insert into @pointsTableUpdated values(20, 15, 4, @pointStyleId)
	insert into @pointsTableUpdated values(22, 15, 5, @pointStyleId)

	exec dbo.UpdateLine @boardId=@boardIdx, @figureId=@figureId, @color='red', @points=@pointsTableUpdated
	
	declare @testResult int = 0
	
	--verificar que os novos pontos foram inseridos na tabela Point e estão associados à linha
	if((select count(point.id) as counterx from dbo.Point as point
		inner join dbo.Line_Point as figP
		on figP.boardId = @boardIdx and figP.pointId = point.id
		where (x=12 and y=15 and pointIdx=0) or (x=14 and y=15 and pointIdx=1) or (x=16 and y=15 and pointIdx=2) or (x=18 and y=15 and pointIdx=3) or (x=20 and y=15 and pointIdx=4) or (x=22 and y=15 and pointIdx=5) 
		)= 6)
			set @testResult = @testResult+1


	--verificar se o estilo da linha tem a nova cor
	if exists (select fs.color from dbo.Line lin inner join dbo.LineStyle as fs on lin.figureStyleId=fs.figureStyleId where fs.color='red')
			set @testResult = @testResult+1

	if @testResult = 2
		print 'Test Passed'
	else
		print 'Test Failed'

rollback

