CREATE VIEW [dbo].[Full_User_Board]
AS 
	SELECT ub.permission, ub.userId, b.id AS boardId, b.[name] AS boardName, b.maxDistPoints , b.basePermission
	FROM dbo.User_Board AS ub 
	INNER JOIN dbo.Board AS b
	ON(ub.boardId = b.id)