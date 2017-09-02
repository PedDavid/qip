CREATE FUNCTION [dbo].[GetPermission]
(
	@userId varchar(128),
	@boardId BIGINT
)
RETURNS TINYINT
AS
BEGIN
	DECLARE @base TINYINT = 0
	DECLARE @specific TINYINT = 0

	IF(@userId IS NOT NULL)
		SELECT @specific=permission FROM dbo.User_Board WHERE userId=@userId and boardId=@boardId

	SELECT @base=basePermission FROM dbo.Board WHERE id=@boardId

	IF(@specific > @base)
		RETURN @specific

	RETURN @base
END
