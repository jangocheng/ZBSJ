-- =============================================
-- Author:		�ֽ���
-- Create date: 2010-12-17
-- Description:	ɾ����¼
-- =============================================
CREATE PROCEDURE [dbo].[Table_Delete]
@TableName varchar(20), --Ҫ��ѯ�ı���
@IDName varchar(20),    --Ҫ��ѯ�������ֶ���
@IDValues varchar(200)  --Ҫ��ѯ�������ֶ�����ֵ
 AS 
	 BEGIN
		DECLARE @SQL VARCHAR(300)
		SET @SQL=N'DELETE ' + @TableName + N' where ' + @IDName + N' in(' + @IDValues + N')'
		EXEC(@SQL)
		return @@rowcount
	 END

GO

	 -- =============================================
-- Author:		�ֽ���
-- Create date: 2010-12-30
-- Description:	��ѯ�ж��ټ�¼
-- =============================================
CREATE procedure [dbo].[Table_Count] 
(
@TableName varchar(50), --Ҫ��ѯ�ı���
@Where varchar (5000)     --Ҫ��ѯ������
)
as
DECLARE @SQL VARCHAR(1500)       --ƴ�ӵ�SQL
DECLARE @TableWhere varchar (5000) --��ѯ�ı����Ͳ�ѯ����

if(@where<>'')
begin
	SET @TableWhere = @TableName + N' where ' + @Where
end
else
begin
	SET @TableWhere = @TableName
end

SET @SQL=N'select count(*) as count from '+ @TableWhere
EXEC(@SQL)

GO

-- =============================================
-- Author:		�ֽ���
-- Create date: 2010-12-30
-- Description:	��ҳ�洢����SQL2005������ͨ��
CREATE procedure [dbo].[Table_List] 
(
@TableName varchar(20), --Ҫ��ѯ�ı���
@Fields varchar(500),   --Ҫ��ѯ���ֶ�����
@Where varchar (5000),    --Ҫ��ѯ������
@OrderBy varchar(200),   --����
@PageSize int,          --��ʾ����
@PageIndex int          --ҳ��
)
as
DECLARE @SQL VARCHAR(1500)       --ƴ�ӵ�SQL
DECLARE @TableWhere varchar (5000) --��ѯ�ı����Ͳ�ѯ����
DECLARE @StartNumber VARCHAR(10) --�ڼ������ݿ�ʼ
DECLARE @EndNumber VARCHAR(10)   --�ڼ������ݽ���
DECLARE @Row VARCHAR(100)        --��ʾ�ڼ������ڼ���

if(@where<>'')
begin
	SET @TableWhere = @TableName + N' where ' + @Where
end
else
begin
	SET @TableWhere = @TableName
end
IF(@PageSize = 0)
BEGIN
	SET @Row = N'' 
END
ELSE
BEGIN
	SET @StartNumber =(@PageIndex-1)*@PageSize+1
	SET @EndNumber =(@PageIndex-1)*@PageSize+@PageSize
	SET @Row = N' where Row between '+ @StartNumber + N' and '+ @EndNumber
END
SET @SQL=N'with temptbl as (SELECT ROW_NUMBER() OVER (ORDER BY '+@OrderBy + N')AS Row, '+@Fields +N' from '+@TableWhere + N') SELECT * FROM temptbl'+ @Row
EXEC(@SQL)

GO

-- =============================================
-- Author:		�ֽ���
-- Create date: 2015-12-30
-- Description:	ͨ��SQL��ҳ

CREATE PROCEDURE [dbo].[sys_Page_v2]  
@PCount int output,    --��ҳ�����  
@RCount int output,    --�ܼ�¼�����  
@sys_Table nvarchar(100),    --��ѯ����  
@sys_Key varchar(50),        --����  
@sys_Fields nvarchar(500),    --��ѯ�ֶ�  
@sys_Where nvarchar(3000),    --��ѯ����  
@sys_Order nvarchar(100),    --�����ֶ�  
@sys_Begin int,        --��ʼλ��  
@sys_PageIndex int,        --��ǰҳ��  
@sys_PageSize int        --ҳ��С  
AS  
SET NOCOUNT ON  
SET ANSI_WARNINGS ON  
IF @sys_PageSize < 0 OR @sys_PageIndex < 0  
BEGIN          
RETURN  
END  
DECLARE @new_where1 NVARCHAR(3000)  
DECLARE @new_order1 NVARCHAR(100)  
DECLARE @new_order2 NVARCHAR(100)  
DECLARE @Sql NVARCHAR(4000)  
DECLARE @SqlCount NVARCHAR(4000)  
DECLARE @Top int  
if(@sys_Begin <=0)  
    set @sys_Begin=0  
else  
    set @sys_Begin=@sys_Begin-1  
IF ISNULL(@sys_Where,'') = ''  
    SET @new_where1 = ' '  
ELSE  
    SET @new_where1 = ' WHERE ' + @sys_Where  
IF ISNULL(@sys_Order,'') <> ''   
BEGIN  
    SET @new_order1 = ' ORDER BY ' + Replace(@sys_Order,'desc','')  
    SET @new_order1 = Replace(@new_order1,'asc','desc')  
    SET @new_order2 = ' ORDER BY ' + @sys_Order  
END  
ELSE  
BEGIN  
    SET @new_order1 = ' ORDER BY ID DESC'  
    SET @new_order2 = ' ORDER BY ID ASC'  
END  
SET @SqlCount = 'SELECT @RCount=COUNT(1),@PCount=CEILING((COUNT(1)+0.0)/'  
            + CAST(@sys_PageSize AS NVARCHAR)+') FROM ' + @sys_Table + @new_where1  
EXEC SP_EXECUTESQL @SqlCount,N'@RCount INT OUTPUT,@PCount INT OUTPUT',  
               @RCount OUTPUT,@PCount OUTPUT  
IF @sys_PageIndex > CEILING((@RCount+0.0)/@sys_PageSize)    --�������ĵ�ǰҳ������ʵ����ҳ��,���ʵ����ҳ����ֵ����ǰҳ��  
BEGIN  
    SET @sys_PageIndex =  CEILING((@RCount+0.0)/@sys_PageSize)  
END  
set @sql = 'select '+ @sys_fields +' from ' + @sys_Table + ' w1 '  
    + ' where '+ @sys_Key +' in ('  
        +'select top '+ ltrim(str(@sys_PageSize)) +' ' + @sys_Key + ' from '  
        +'('  
            +'select top ' + ltrim(STR(@sys_PageSize * @sys_PageIndex + @sys_Begin)) + ' ' + @sys_Key + ' FROM '  
        + @sys_Table + @new_where1 + @new_order2   
        +') w ' + @new_order1  
    +') ' + @new_order2  
print(@sql)  
Exec(@sql)  

GO 