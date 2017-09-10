CREATE FUNCTION [dbo].[GetPermission]
(
	@userId varchar(128),
	@boardId BIGINT
)
RETURNS TINYINT
AS
BEGIN
	DECLARE @permission TINYINT = 0

	SELECT @permission = IIF(ub.permission >= b.basePermission, ub.permission, b.basePermission)
	FROM dbo.Board AS b
	LEFT JOIN (SELECT * FROM dbo.User_Board WHERE userId=@userId) AS ub
	ON(b.id = ub.boardId)
	WHERE b.id = @boardId

	RETURN @permission
END
