/****** Object:  Table [dbo].[Files]    Script Date: 09/15/2010 10:31:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Files](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Files_id]  DEFAULT (newid()),
	[FileName] [varchar](512) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[processdate] [datetime] NULL,
	[md5] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF