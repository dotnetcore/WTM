using System;
using System.Collections.Concurrent;

namespace WalkingTec.Mvvm.Core.Support.Agent
{
    /// <summary>
    /// Agent相关表的建表保障。
    /// 新库由EF的EnsureCreated自动建表；对于已存在的旧库（EnsureCreated不会增量建表），
    /// 首次访问Agent功能时按数据库类型执行CREATE TABLE IF NOT EXISTS等价语句，
    /// 列定义与EF默认映射兼容
    /// </summary>
    public static class AgentTableHelper
    {
        /// <summary>
        /// 每个连接串只执行一次检查
        /// </summary>
        private static readonly ConcurrentDictionary<string, bool> Ensured = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// 确保AgentFlows/AgentRuns表存在；内部按连接串去重，
        /// 内存库（测试）由EnsureCreated建表，跳过
        /// </summary>
        /// <param name="dc">数据上下文</param>
        public static void EnsureTables(IDataContext dc)
        {
            if (dc == null || dc.DBType == DBTypeEnum.Memory)
                return;
            var key = $"{dc.DBType}|{dc.GetType().FullName}";
            if (Ensured.ContainsKey(key))
                return;
            lock (Ensured)
            {
                if (Ensured.ContainsKey(key))
                    return;
                try
                {
                    var sql = BuildSql(dc.DBType);
                    dc.RunSQL(sql);
                    Ensured[key] = true;
                }
                catch
                {
                    //建表失败不阻断请求；若表确实缺失，后续查询会抛出明确异常
                }
            }
        }

        /// <summary>
        /// 按数据库类型生成两条表的建表语句（含发布方式字段），
        /// 并对已存在的旧表幂等补充新列
        /// </summary>
        private static string BuildSql(DBTypeEnum dbType)
        {
            switch (dbType)
            {
                case DBTypeEnum.SqlServer:
                    return @"
IF OBJECT_ID(N'[dbo].[AgentFlows]', 'U') IS NULL
CREATE TABLE [dbo].[AgentFlows](
    [ID] uniqueidentifier NOT NULL PRIMARY KEY,
    [Name] nvarchar(100) NULL,
    [Description] nvarchar(500) NULL,
    [GraphJson] nvarchar(max) NULL,
    [IsSkill] bit NOT NULL DEFAULT 0,
    [IsScheduled] bit NOT NULL DEFAULT 0,
    [ScheduleCron] nvarchar(100) NULL,
    [ScheduleInputsJson] nvarchar(max) NULL,
    [LastScheduleTime] datetime2 NULL,
    [CreateTime] datetime2 NULL,
    [CreateBy] nvarchar(50) NULL,
    [UpdateTime] datetime2 NULL,
    [UpdateBy] nvarchar(50) NULL,
    [IsValid] bit NOT NULL
);
IF OBJECT_ID(N'[dbo].[AgentRuns]', 'U') IS NULL
CREATE TABLE [dbo].[AgentRuns](
    [ID] uniqueidentifier NOT NULL PRIMARY KEY,
    [FlowId] uniqueidentifier NULL,
    [FlowName] nvarchar(100) NULL,
    [Status] int NOT NULL,
    [InputJson] nvarchar(max) NULL,
    [OutputJson] nvarchar(max) NULL,
    [LogsJson] nvarchar(max) NULL,
    [Error] nvarchar(max) NULL,
    [DurationMs] bigint NOT NULL,
    [CreateTime] datetime2 NULL,
    [CreateBy] nvarchar(50) NULL,
    [UpdateTime] datetime2 NULL,
    [UpdateBy] nvarchar(50) NULL,
    [IsValid] bit NOT NULL
);
--旧库幂等补充发布方式列
IF COL_LENGTH('dbo.AgentFlows','IsSkill') IS NULL ALTER TABLE dbo.AgentFlows ADD [IsSkill] bit NOT NULL DEFAULT 0;
IF COL_LENGTH('dbo.AgentFlows','IsScheduled') IS NULL ALTER TABLE dbo.AgentFlows ADD [IsScheduled] bit NOT NULL DEFAULT 0;
IF COL_LENGTH('dbo.AgentFlows','ScheduleCron') IS NULL ALTER TABLE dbo.AgentFlows ADD [ScheduleCron] nvarchar(100) NULL;
IF COL_LENGTH('dbo.AgentFlows','ScheduleInputsJson') IS NULL ALTER TABLE dbo.AgentFlows ADD [ScheduleInputsJson] nvarchar(max) NULL;
IF COL_LENGTH('dbo.AgentFlows','LastScheduleTime') IS NULL ALTER TABLE dbo.AgentFlows ADD [LastScheduleTime] datetime2 NULL;
IF OBJECT_ID(N'[dbo].[AgentChatMessages]', 'U') IS NULL
CREATE TABLE [dbo].[AgentChatMessages](
    [ID] uniqueidentifier NOT NULL PRIMARY KEY,
    [UserCode] nvarchar(50) NULL,
    [Role] nvarchar(20) NULL,
    [Content] nvarchar(max) NULL,
    [CreateTime] datetime2 NULL
);";
                case DBTypeEnum.MySql:
                    return @"
CREATE TABLE IF NOT EXISTS AgentFlows(
    ID char(36) NOT NULL PRIMARY KEY,
    Name varchar(100) NULL,
    Description varchar(500) NULL,
    GraphJson longtext NULL,
    IsSkill tinyint(1) NOT NULL DEFAULT 0,
    IsScheduled tinyint(1) NOT NULL DEFAULT 0,
    ScheduleCron varchar(100) NULL,
    ScheduleInputsJson longtext NULL,
    LastScheduleTime datetime NULL,
    CreateTime datetime NULL,
    CreateBy varchar(50) NULL,
    UpdateTime datetime NULL,
    UpdateBy varchar(50) NULL,
    IsValid tinyint(1) NOT NULL DEFAULT 1
);
CREATE TABLE IF NOT EXISTS AgentRuns(
    ID char(36) NOT NULL PRIMARY KEY,
    FlowId char(36) NULL,
    FlowName varchar(100) NULL,
    Status int NOT NULL DEFAULT 0,
    InputJson longtext NULL,
    OutputJson longtext NULL,
    LogsJson longtext NULL,
    Error longtext NULL,
    DurationMs bigint NOT NULL DEFAULT 0,
    CreateTime datetime NULL,
    CreateBy varchar(50) NULL,
    UpdateTime datetime NULL,
    UpdateBy varchar(50) NULL,
    IsValid tinyint(1) NOT NULL DEFAULT 1
);";
                case DBTypeEnum.PgSql:
                    return @"
CREATE TABLE IF NOT EXISTS ""AgentFlows""(
    ""ID"" uuid NOT NULL PRIMARY KEY,
    ""Name"" varchar(100) NULL,
    ""Description"" varchar(500) NULL,
    ""GraphJson"" text NULL,
    ""IsSkill"" boolean NOT NULL DEFAULT false,
    ""IsScheduled"" boolean NOT NULL DEFAULT false,
    ""ScheduleCron"" varchar(100) NULL,
    ""ScheduleInputsJson"" text NULL,
    ""LastScheduleTime"" timestamp NULL,
    ""CreateTime"" timestamp NULL,
    ""CreateBy"" varchar(50) NULL,
    ""UpdateTime"" timestamp NULL,
    ""UpdateBy"" varchar(50) NULL,
    ""IsValid"" boolean NOT NULL DEFAULT true
);
CREATE TABLE IF NOT EXISTS ""AgentRuns""(
    ""ID"" uuid NOT NULL PRIMARY KEY,
    ""FlowId"" uuid NULL,
    ""FlowName"" varchar(100) NULL,
    ""Status"" int NOT NULL,
    ""InputJson"" text NULL,
    ""OutputJson"" text NULL,
    ""LogsJson"" text NULL,
    ""Error"" text NULL,
    ""DurationMs"" bigint NOT NULL,
    ""CreateTime"" timestamp NULL,
    ""CreateBy"" varchar(50) NULL,
    ""UpdateTime"" timestamp NULL,
    ""UpdateBy"" varchar(50) NULL,
    ""IsValid"" boolean NOT NULL DEFAULT true
);
--旧库幂等补充发布方式列
ALTER TABLE ""AgentFlows"" ADD COLUMN IF NOT EXISTS ""IsSkill"" boolean NOT NULL DEFAULT false;
ALTER TABLE ""AgentFlows"" ADD COLUMN IF NOT EXISTS ""IsScheduled"" boolean NOT NULL DEFAULT false;
ALTER TABLE ""AgentFlows"" ADD COLUMN IF NOT EXISTS ""ScheduleCron"" varchar(100) NULL;
ALTER TABLE ""AgentFlows"" ADD COLUMN IF NOT EXISTS ""ScheduleInputsJson"" text NULL;
ALTER TABLE ""AgentFlows"" ADD COLUMN IF NOT EXISTS ""LastScheduleTime"" timestamp NULL;";
                case DBTypeEnum.SQLite:
                    return @"
CREATE TABLE IF NOT EXISTS AgentFlows(
    ID TEXT NOT NULL PRIMARY KEY,
    Name TEXT NULL,
    Description TEXT NULL,
    GraphJson TEXT NULL,
    IsSkill INTEGER NOT NULL DEFAULT 0,
    IsScheduled INTEGER NOT NULL DEFAULT 0,
    ScheduleCron TEXT NULL,
    ScheduleInputsJson TEXT NULL,
    LastScheduleTime TEXT NULL,
    CreateTime TEXT NULL,
    CreateBy TEXT NULL,
    UpdateTime TEXT NULL,
    UpdateBy TEXT NULL,
    IsValid INTEGER NOT NULL DEFAULT 1
);
CREATE TABLE IF NOT EXISTS AgentRuns(
    ID TEXT NOT NULL PRIMARY KEY,
    FlowId TEXT NULL,
    FlowName TEXT NULL,
    Status INTEGER NOT NULL DEFAULT 0,
    InputJson TEXT NULL,
    OutputJson TEXT NULL,
    LogsJson TEXT NULL,
    Error TEXT NULL,
    DurationMs INTEGER NOT NULL DEFAULT 0,
    CreateTime TEXT NULL,
    CreateBy TEXT NULL,
    UpdateTime TEXT NULL,
    UpdateBy TEXT NULL,
    IsValid INTEGER NOT NULL DEFAULT 1
);";
                default:
                    //Oracle等其余类型由EnsureCreated或手工建表
                    return "";
            }
        }
    }
}
