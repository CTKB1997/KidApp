USE [master]
GO
/****** Object:  Database [KidApp]    Script Date: 6/10/2018 11:10:02 PM ******/
CREATE DATABASE [KidApp]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'KidApp', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\KidApp.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'KidApp_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\KidApp_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [KidApp] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [KidApp].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [KidApp] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [KidApp] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [KidApp] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [KidApp] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [KidApp] SET ARITHABORT OFF 
GO
ALTER DATABASE [KidApp] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [KidApp] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [KidApp] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [KidApp] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [KidApp] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [KidApp] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [KidApp] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [KidApp] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [KidApp] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [KidApp] SET  DISABLE_BROKER 
GO
ALTER DATABASE [KidApp] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [KidApp] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [KidApp] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [KidApp] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [KidApp] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [KidApp] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [KidApp] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [KidApp] SET RECOVERY FULL 
GO
ALTER DATABASE [KidApp] SET  MULTI_USER 
GO
ALTER DATABASE [KidApp] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [KidApp] SET DB_CHAINING OFF 
GO
ALTER DATABASE [KidApp] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [KidApp] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [KidApp] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [KidApp] SET QUERY_STORE = OFF
GO
USE [KidApp]
GO
/****** Object:  Table [dbo].[EngResult]    Script Date: 6/10/2018 11:10:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EngResult](
	[engId] [varchar](50) NOT NULL,
	[eng1] [varchar](50) NULL,
	[eng2] [varchar](50) NULL,
	[eng3] [varchar](50) NULL,
	[active] [bit] NULL,
 CONSTRAINT [PK_EngResult] PRIMARY KEY CLUSTERED 
(
	[engId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Image]    Script Date: 6/10/2018 11:10:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[imageId] [varchar](50) NOT NULL,
	[imageName] [varchar](50) NULL,
	[timeShoot] [float] NULL,
	[userId] [varchar](50) NULL,
	[active] [bit] NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[imageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 6/10/2018 11:10:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[userId] [varchar](50) NOT NULL,
	[userName] [varchar](50) NULL,
	[password] [nvarchar](500) NULL,
	[dob] [date] NULL,
	[address] [nvarchar](100) NULL,
	[active] [bit] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VieResult]    Script Date: 6/10/2018 11:10:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VieResult](
	[vieId] [varchar](50) NOT NULL,
	[vie1] [varchar](50) NULL,
	[vie2] [varchar](50) NULL,
	[vie3] [varchar](50) NULL,
	[active] [bit] NULL,
 CONSTRAINT [PK_VieResult] PRIMARY KEY CLUSTERED 
(
	[vieId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[EngResult] ([engId], [eng1], [eng2], [eng3], [active]) VALUES (N'Image1', N'1', N'1', N'1', 1)
INSERT [dbo].[Image] ([imageId], [imageName], [timeShoot], [userId], [active]) VALUES (N'Image0', N'1', 1, N'User1', 1)
INSERT [dbo].[Image] ([imageId], [imageName], [timeShoot], [userId], [active]) VALUES (N'Image1', N'1', 1, N'User1', 1)
INSERT [dbo].[Image] ([imageId], [imageName], [timeShoot], [userId], [active]) VALUES (N'Image2', N'1', 1, N'User1', 1)
INSERT [dbo].[Image] ([imageId], [imageName], [timeShoot], [userId], [active]) VALUES (N'Image3', N'1', 1, N'User1', 1)
INSERT [dbo].[Image] ([imageId], [imageName], [timeShoot], [userId], [active]) VALUES (N'Image4', N'1', 1, N'User1', 1)
INSERT [dbo].[Image] ([imageId], [imageName], [timeShoot], [userId], [active]) VALUES (N'Image5', N'1', 1, N'User1', 1)
INSERT [dbo].[User] ([userId], [userName], [password], [dob], [address], [active]) VALUES (N'User0', N'test', N'test', CAST(N'1900-01-01' AS Date), N'111', 1)
INSERT [dbo].[User] ([userId], [userName], [password], [dob], [address], [active]) VALUES (N'User1', N'abcdef', N'Hl?QC??~-???? ????9"?Z?????', CAST(N'2018-10-06' AS Date), N'123, Nguyễn Thị Minh Khai, Quận 1', 1)
INSERT [dbo].[VieResult] ([vieId], [vie1], [vie2], [vie3], [active]) VALUES (N'Image1', N'1', N'2', N'3', 1)
ALTER TABLE [dbo].[EngResult]  WITH CHECK ADD  CONSTRAINT [FK_EngResult_Image] FOREIGN KEY([engId])
REFERENCES [dbo].[Image] ([imageId])
GO
ALTER TABLE [dbo].[EngResult] CHECK CONSTRAINT [FK_EngResult_Image]
GO
ALTER TABLE [dbo].[Image]  WITH CHECK ADD  CONSTRAINT [FK_Image_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([userId])
GO
ALTER TABLE [dbo].[Image] CHECK CONSTRAINT [FK_Image_User]
GO
ALTER TABLE [dbo].[VieResult]  WITH CHECK ADD  CONSTRAINT [FK_VieResult_Image] FOREIGN KEY([vieId])
REFERENCES [dbo].[Image] ([imageId])
GO
ALTER TABLE [dbo].[VieResult] CHECK CONSTRAINT [FK_VieResult_Image]
GO
USE [master]
GO
ALTER DATABASE [KidApp] SET  READ_WRITE 
GO
