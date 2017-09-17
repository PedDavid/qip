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
	LEFT JOIN dbo.User_Board AS ub
	ON(b.id = ub.boardId)
	WHERE b.id = @boardId AND ub.userId=@userId

	RETURN @permission
END
