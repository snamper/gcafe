CREATE TRIGGER [tgr_printcnt]
	ON [dbo].[printer]
	FOR UPDATE
	AS
	BEGIN
		SET NOCOUNT ON

		DECLARE @cnt INT, @pg_id INT, @total_cnt INT;
		SELECT @cnt = print_cnt, @pg_id = printer_group_id FROM inserted;
		IF @cnt != 0
			BEGIN
				SELECT @cnt = print_cnt, @total_cnt = print_total_cnt FROM printer_group WHERE id = @pg_id;
				SET @cnt = @cnt + 1;
				SET @total_cnt = @total_cnt + 1;
				UPDATE printer_group SET print_cnt = @cnt, print_total_cnt = @total_cnt WHERE id = @pg_id;

				SELECT @cnt = print_cnt FROM sys_info;
				SET @cnt = @cnt + 1;
				UPDATE sys_info SET print_cnt = @cnt;
			END
	END
