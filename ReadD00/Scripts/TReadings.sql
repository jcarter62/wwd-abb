﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TReadings](
	[id] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_TReadings_id]  DEFAULT (newid()),
	[SiteName] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ChName] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[dtime] [datetime] NULL,
	[reading] [float] NULL,
	[session] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TReadings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF