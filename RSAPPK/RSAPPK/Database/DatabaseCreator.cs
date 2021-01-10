using System;

namespace RSAPPK.Database
{
    /// <summary>Assists in the creation of the database and tables for RSA PPK storage.</summary>
    public static class DatabaseCreator
    {
        /// <summary>Returns the create script for the database.</summary>
        /// <returns>The create database script.</returns>
        public static string GetCreateDatabaseScript()
        {
            return GetCreateDatabaseScript(@"C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\RsaPpk.mdf",
                @"C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\RsaPpk_log.ldf");
        }

        /// <summary>Returns the create script for the database (including table).</summary>
        /// <param name="databaseFile">The absolute path to the database, so including name and extension.</param>
        /// <param name="logFile">The absolute path to the log, so including name and extension.</param>
        /// <returns>The create database script.</returns>
        public static string GetCreateDatabaseScript(string databaseFile, string logFile)
        {
            return "USE [master]" + Environment.NewLine +
                "CREATE DATABASE [RsaPpk]" + Environment.NewLine +
                " CONTAINMENT = NONE" + Environment.NewLine +
                " ON PRIMARY " + Environment.NewLine +
                $"(NAME = N'RsaPpk', FILENAME = N'{databaseFile}' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )" + Environment.NewLine +
                " LOG ON " + Environment.NewLine +
                $"(NAME = N'RsaPpk_log', FILENAME = N'{logFile}' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )" + Environment.NewLine +
                " WITH CATALOG_COLLATION = DATABASE_DEFAULT" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "IF(1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) " + Environment.NewLine +
                "begin " + Environment.NewLine +
                "EXEC[RsaPpk].[dbo].[sp_fulltext_database] @action = 'enable' " + Environment.NewLine +
                "end" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET ANSI_NULL_DEFAULT OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET ANSI_NULLS OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET ANSI_PADDING OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET ANSI_WARNINGS OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET ARITHABORT OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET AUTO_CLOSE OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET AUTO_SHRINK OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET AUTO_UPDATE_STATISTICS ON" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET CURSOR_CLOSE_ON_COMMIT OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET CURSOR_DEFAULT  GLOBAL" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET CONCAT_NULL_YIELDS_NULL OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET NUMERIC_ROUNDABORT OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET QUOTED_IDENTIFIER OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET RECURSIVE_TRIGGERS OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET DISABLE_BROKER" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET AUTO_UPDATE_STATISTICS_ASYNC OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET DATE_CORRELATION_OPTIMIZATION OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET TRUSTWORTHY OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET ALLOW_SNAPSHOT_ISOLATION OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET PARAMETERIZATION SIMPLE" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET READ_COMMITTED_SNAPSHOT OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET HONOR_BROKER_PRIORITY OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET RECOVERY SIMPLE" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET MULTI_USER" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET PAGE_VERIFY CHECKSUM" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET DB_CHAINING OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET FILESTREAM(NON_TRANSACTED_ACCESS = OFF)" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET TARGET_RECOVERY_TIME = 60 SECONDS" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET DELAYED_DURABILITY = DISABLED" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET ACCELERATED_DATABASE_RECOVERY = OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET QUERY_STORE = OFF" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "ALTER DATABASE[RsaPpk] SET READ_WRITE" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "USE [RsaPpk]" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "SET ANSI_NULLS ON" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "SET QUOTED_IDENTIFIER ON" + Environment.NewLine +
                "GO" + Environment.NewLine +
                "CREATE TABLE[dbo].[RsaPpks](" + Environment.NewLine +
                "    [Id][int] IDENTITY(1, 1) NOT NULL," + Environment.NewLine +
                "    [Name] [varchar](250) NOT NULL," + Environment.NewLine +
                "    [RsaPpkXml] [varchar](1679) NOT NULL," + Environment.NewLine +
                " CONSTRAINT[PK_RsaPpks] PRIMARY KEY CLUSTERED ([Id] ASC)" + Environment.NewLine +
                " WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]" + Environment.NewLine +
                ") ON[PRIMARY]" + Environment.NewLine +
                "GO";
        }
    }
}
