CREATE PROCEDURE [dbo].[InsertOrUpdatePreferences]
	@userId VARCHAR(128),
	@favorites varchar(max),
	@penColors varchar(max),
	@defaultPen VARCHAR(256),
	@defaultEraser VARCHAR(256),
	@currTool VARCHAR(256),
	@settings VARCHAR(256),
	@created BIT OUT
AS
	begin try
		set transaction isolation level serializable
		begin TRAN
			IF(EXISTS(SELECT * FROM dbo.Preferences WHERE id = @userId))
			begin
				UPDATE dbo.Preferences 
				SET favorites = @favorites, penColors = @penColors, defaultPen = @defaultPen, defaultEraser = @defaultEraser, currTool = @currTool, settings = @settings
				WHERE id = @userId
				
				SET @created = 0
			END
			ELSE
			BEGIN
				INSERT INTO dbo.Preferences(id, favorites, penColors, defaultPen, defaultEraser, currTool, settings) 
									 VALUES(@userId, @favorites, @penColors, @defaultPen, @defaultEraser, @currTool, @settings)

				SET @created = 1
			END
		COMMIT
	end try
	begin catch
		if @@TRANCOUNT <> 0
			rollback;
		throw
	end catch
	