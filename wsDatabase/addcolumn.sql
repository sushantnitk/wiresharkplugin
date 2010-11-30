use [msqq]
GO
BULK INSERT msqqBulk 
    FROM 'G:\chensheng\QQ×¥°ü²âÊÔ\µÇÂ½2\aa.csv' 
    WITH 
    ( 
        FIELDTERMINATOR = ';', 
        ROWTERMINATOR = '\n' 
    )
	
update msqqBulk set message_type='µÇÂ½2' where message_type='4'