USE [SSRSPro]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CatalogItem](
	[ItemID] [uniqueidentifier] NOT NULL,
	[Path] [varchar](500) NOT NULL,
	[Content] [varchar](max) NULL,
	[ItemType] [varchar](100) NOT NULL,
	[MimeType] [varchar](100) NULL,
	[Properties] [varchar](max) NULL,
	[ItemPermissions] [varchar](max) NULL,
	[CreatedBy] [varchar](100) NOT NULL,
	[ModifiedBy] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY NONCLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE CLUSTERED 
(
	[Path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
USE [SSRSPro]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReportSession](
	[SessionID] [uniqueidentifier] NOT NULL,
	[ReportPath] [varchar](1000) NULL,
	[Content] [varchar](max) NULL,
	[LastUsed] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
USE [SSRSPro]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Subscriptions](
	[SubscriptionID] [uniqueidentifier] NOT NULL,
	[OwnerID] [varchar](100) NOT NULL,
	[ItemID] [uniqueidentifier] NOT NULL,
	[Status] [varchar](100) NOT NULL,
	[StatusTime] [datetime] NULL,
	[DeliveryStatus] [varchar](100) NULL,
	[Message] [varchar](2000) NULL,
	[DeliveryStatusTime] [datetime] NULL,
	[NextRun] [datetime] NULL,
	[Data] [varchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO




