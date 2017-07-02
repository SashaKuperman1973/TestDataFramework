USE [master]
GO
/****** Object:  Database [TestDataFramework]    Script Date: 7/2/2017 2:40:38 PM ******/
CREATE DATABASE [TestDataFramework]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TestDataFramework', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\TestDataFramework.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TestDataFramework_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\TestDataFramework_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [TestDataFramework] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TestDataFramework].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TestDataFramework] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TestDataFramework] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TestDataFramework] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TestDataFramework] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TestDataFramework] SET ARITHABORT OFF 
GO
ALTER DATABASE [TestDataFramework] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TestDataFramework] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TestDataFramework] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TestDataFramework] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TestDataFramework] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TestDataFramework] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TestDataFramework] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TestDataFramework] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TestDataFramework] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TestDataFramework] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TestDataFramework] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TestDataFramework] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TestDataFramework] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TestDataFramework] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TestDataFramework] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TestDataFramework] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TestDataFramework] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TestDataFramework] SET RECOVERY FULL 
GO
ALTER DATABASE [TestDataFramework] SET  MULTI_USER 
GO
ALTER DATABASE [TestDataFramework] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TestDataFramework] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TestDataFramework] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TestDataFramework] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TestDataFramework] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'TestDataFramework', N'ON'
GO
ALTER DATABASE [TestDataFramework] SET QUERY_STORE = OFF
GO
USE [TestDataFramework]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [TestDataFramework]
GO
/****** Object:  Table [dbo].[ForeignToAutoPrimaryTable]    Script Date: 7/2/2017 2:40:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForeignToAutoPrimaryTable](
	[ForignKey] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ManualKeyForeignTable]    Script Date: 7/2/2017 2:40:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManualKeyForeignTable](
	[UserId] [uniqueidentifier] NOT NULL,
	[FirstForeignKey] [int] NULL,
	[ForeignKey1] [varchar](20) NULL,
	[ForeignKey2] [int] NULL,
	[AShort] [smallint] NOT NULL,
	[ALong] [bigint] NOT NULL,
 CONSTRAINT [PK_ManualKeyForeignTable] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ManualKeyPrimaryTable]    Script Date: 7/2/2017 2:40:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManualKeyPrimaryTable](
	[Tester] [int] IDENTITY(1,1) NOT NULL,
	[Key1] [varchar](20) NOT NULL,
	[Key2] [int] NOT NULL,
	[AString] [varchar](100) NOT NULL,
	[ADecimal] [decimal](18, 2) NULL,
	[AFloat] [float] NOT NULL,
 CONSTRAINT [PK_ManualKeyPrimaryTable] PRIMARY KEY CLUSTERED 
(
	[Tester] ASC,
	[Key1] ASC,
	[Key2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 7/2/2017 2:40:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subject](
	[Key] [int] NOT NULL,
	[GuidKey] [uniqueidentifier] NOT NULL,
	[Integer] [int] NOT NULL,
	[IntegerWithMax] [int] NOT NULL,
	[NullableInteger] [int] NULL,
	[LongInteger] [bigint] NOT NULL,
	[LongIntegerWithMax] [bigint] NOT NULL,
	[NullableLong] [bigint] NULL,
	[ShortInteger] [smallint] NOT NULL,
	[ShortIntegerWithMax] [smallint] NOT NULL,
	[NullableShort] [smallint] NULL,
	[Text] [varchar](100) NOT NULL,
	[TextWithLength] [varchar](5) NOT NULL,
	[Character] [char](1) NOT NULL,
	[Decimal] [decimal](18, 0) NOT NULL,
	[DecimalWithPrecision] [decimal](18, 4) NOT NULL,
	[Boolean] [bit] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[DateTimeWithTense] [datetime] NULL,
	[Byte] [tinyint] NOT NULL,
	[Double] [float] NOT NULL,
	[DoubleWithPrecision] [decimal](18, 4) NOT NULL,
	[AnEmailAddress] [varchar](100) NOT NULL,
	[AnotherEmailAddress] [varchar](100) NOT NULL,
	[ANullableGuid] [uniqueidentifier] NULL,
	[AGuid] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TertiaryManualKeyForeignTable]    Script Date: 7/2/2017 2:40:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TertiaryManualKeyForeignTable](
	[Pk] [int] IDENTITY(1,1) NOT NULL,
	[FkManualKeyForeignTable] [uniqueidentifier] NOT NULL,
	[FkStringForeignKey] [varchar](20) NOT NULL,
	[AnInt] [int] NOT NULL,
 CONSTRAINT [PK_TertiaryManualKeyForeignTable] PRIMARY KEY CLUSTERED 
(
	[Pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnresolvedKeyTable]    Script Date: 7/2/2017 2:40:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnresolvedKeyTable](
	[DoesntExist] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ManualKeyPrimaryTable]    Script Date: 7/2/2017 2:40:38 PM ******/
CREATE NONCLUSTERED INDEX [IX_ManualKeyPrimaryTable] ON [dbo].[ManualKeyPrimaryTable]
(
	[Tester] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ManualKeyForeignTable]  WITH CHECK ADD  CONSTRAINT [FK_ManualKeyForeignTable_ManualKeyPrimaryTable] FOREIGN KEY([FirstForeignKey], [ForeignKey1], [ForeignKey2])
REFERENCES [dbo].[ManualKeyPrimaryTable] ([Tester], [Key1], [Key2])
GO
ALTER TABLE [dbo].[ManualKeyForeignTable] CHECK CONSTRAINT [FK_ManualKeyForeignTable_ManualKeyPrimaryTable]
GO
USE [master]
GO
ALTER DATABASE [TestDataFramework] SET  READ_WRITE 
GO
