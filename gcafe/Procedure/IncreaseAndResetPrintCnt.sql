CREATE PROCEDURE [dbo].[IncreaseAndResetPrintCnt]
	@printerId int,
	@isIncrease bit = 1
AS
	DECLARE @cnt INT, @total_cnt INT;

	IF @isIncrease = 1
		BEGIN
			SELECT @cnt = print_cnt, @total_cnt = print_total_cnt FROM printer WHERE id = @printerId;
			SET @cnt = @cnt + 1;
			SET @total_cnt = @total_cnt + 1;
			UPDATE printer SET print_cnt = @cnt, print_total_cnt = @total_cnt WHERE id = @printerId;
		END
	ELSE
		UPDATE printer SET print_cnt = 0 WHERE id = @printerId;

	SELECT print_cnt FROM printer WHERE id = @printerId;
RETURN 0
