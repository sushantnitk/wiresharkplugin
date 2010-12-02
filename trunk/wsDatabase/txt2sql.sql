USE [msqq]
GO

/****** Object:  Table [dbo].[msqqBulk]    Script Date: 11/30/2010 16:20:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[msqqBulk]') AND type in (N'U'))
DROP TABLE [dbo].[msqqBulk]
GO
CREATE TABLE msqqBulk2 
( 
 frame_number  int,
 frame_time  VARCHAR(32),
 frame_time_relative  VARCHAR(32),
 ip_src VARCHAR(32),
 ip_dst VARCHAR(32),
 tcp_srcport INT,
 tcp_dstport INT,
 msqq_qqNumber VARCHAR(32) null,
 msqq_Length INT null,
 msqq_Version  INT null,
 msqq_Command  INT null,
 msqq_Sequence  INT  null,
 message_type VARCHAR(32),
)

use [msqq]
GO
BULK INSERT msqqBulk2 
    FROM 'G:\chensheng\QQ抓包测试\心跳\aa.csv' 
    WITH 
    ( 
        FIELDTERMINATOR = ';', 
        ROWTERMINATOR = '\n' 
    )
	
update msqqBulk2 set message_type='心跳' where message_type='4'
    