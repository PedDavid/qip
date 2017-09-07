
--test insert board

GO

begin tran
	declare @boardName varchar(100) = 'boardTestName'
	declare @boardId bigint = -1
	declare @userId varchar(128) = 'userTestId'
	declare @maxDistPoints tinyint = 0

	exec dbo.InsertBoard @name=@boardName, @maxDistPoints=@maxDistPoints, @userId=@userId, @boardId=@boardId out
	
	declare @testResult int = 0
	if exists (select * from dbo.Board where id=@boardId and [name]=@boardName)
		set @testResult = @testResult+1

	if exists (select * from dbo.User_Board where boardId=@boardId and userId=@userId)
		set @testResult = @testResult+1

	if @testResult = 2
		print 'Test Passed'
	else begin
		rollback;
		throw 51000, 'Test Failed', 1;	
		return;
	end

rollback

--test insert board without user

GO

begin tran
	declare @boardName varchar(20) = 'boardTestName'
	declare @boardId int = -1000
	declare @userId varchar(20) = 'userTestId'
	declare @maxDistPoints int = 0

	exec dbo.InsertBoard @name=@boardName, @maxDistPoints=@maxDistPoints, @boardId=@boardId out

	declare @testResult int = 0
	if exists (select * from dbo.Board where id=@boardId)
		set @testResult = @testResult+1

	if not exists (select * from dbo.User_Board where boardId=@boardId and userId=@userId)
		set @testResult = @testResult+1

	if @testResult = 2
		print 'Test Passed'
	else begin
		rollback;
		throw 51000, 'Test Failed', 1;	
		return;
	end

rollback

select * from dbo.Board
select * from dbo.User_Board
UPDATE dbo.Board
SET basePermission = 2 WHERE id=30016;