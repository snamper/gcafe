CREATE TRIGGER [tgr_order_detail]
	ON [dbo].[order_detail]
	FOR INSERT, UPDATE
	AS
	BEGIN
		SET NOCOUNT ON

		DECLARE @order_id INT, @quantity DECIMAL, @price DECIMAL, @is_cancel BIT, @order_price DECIMAL;

		SELECT @order_id = order_id, @quantity = quantity, @price = price, @is_cancel = is_cancle FROM inserted;
		SELECT @order_price = receivable FROM [dbo].[order] WHERE id = @order_id;

		IF @order_price IS NULL
			SET @order_price = 0.0;

		IF @is_cancel != 1
			SET @order_price = @order_price + @price * @quantity;
		ELSE
			SET @order_price = @order_price - @price * @quantity;

		UPDATE [dbo].[order] SET receivable = @order_price WHERE id = @order_id;
	END
