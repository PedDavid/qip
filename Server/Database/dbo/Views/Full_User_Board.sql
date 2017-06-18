CREATE VIEW [dbo].[Full_User_Board]
AS 
	SELECT ub.permission, u.id AS userId, u.username, u.[name], b.id AS boardId, b.[name] AS boardName, b.maxDistPoints 
	FROM dbo.User_Board AS ub 
	INNER JOIN dbo.[User] AS u
	ON (ub.userId = u.id)
	INNER JOIN dbo.Board AS b
	ON(ub.boardId = b.id)