/****** Object:  StoredProcedure [dbo].[InsertProductCodeForChannel]    Script Date: 01/15/2016 15:41:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProductCodeForChannel]') AND type in (N'P', N'PC'))
DROP PROCEDURE InsertProductCodeForChannel
GO

/****** Object:  StoredProcedure [dbo].[InsertProductCodeForChannel]    Script Date: 01/15/2016 15:41:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[InsertProductCodeForChannel]
(
@ChannelName nvarchar(500),
@ContentCategory int,
@ProductCode varchar(20)
)

As 

Begin
DECLARE @ContentId AS INT
DECLARE @ProductCodeCount AS INT


SET @ProductCodeCount = (select Count(1) from ProductCodeConfiguration where ProductCode = @ProductCode);

If (@ProductCodeCount > 0)
	update ProductCodeConfiguration set ProductCode = @ProductCode  + CONVERT(varchar(10), (SELECT FLOOR(RAND() * POWER(CAST(10 as BIGINT), 5)))) where ProductCode = @ProductCode;

SET @ContentId = (Select Id from Channel where Name = @ChannelName)
	insert into ProductCodeConfiguration (ContentType,ContentId,ContentCategory,ProductCode,AccountType)	values  (1,@ContentId, @ContentCategory,@ProductCode,1)    
end

GO


