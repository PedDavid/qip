CREATE PROCEDURE [dbo].[InsertOrUpdatePreferences]
	@userId VARCHAR(128),
	@favorites varchar(max),
	@penColors varchar(max),
	@created BIT OUT
AS
	begin try
		set transaction isolation level serializable
		begin TRAN
			IF(EXISTS(SELECT * FROM dbo.Preferences WHERE id = @userId))
			begin
				UPDATE dbo.Preferences SET favorites = @favorites, penColors = @penColors WHERE id = @userId
				SET @created = 0
			END
			ELSE
			BEGIN
				INSERT INTO dbo.Preferences(id, favorites, penColors) VALUES(@userId, @favorites, @penColors)
				SET @created = 1
			END
		COMMIT
	end try
	begin catch
		if @@TRANCOUNT <> 0
			rollback;
		throw
	end catch
