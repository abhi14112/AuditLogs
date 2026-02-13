-- Migration: AddAuditLogV2Table
-- This migration adds the AuditLogsV2 table with all necessary indexes for V2 audit logging

-- Create AuditLogsV2 table
CREATE TABLE [dbo].[AuditLogsV2] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(100) NOT NULL,
    [UserName] NVARCHAR(200) NOT NULL,
    [ActionType] NVARCHAR(100) NOT NULL,
    [EntityName] NVARCHAR(100) NOT NULL,
    [EntityId] NVARCHAR(100) NOT NULL,
    [Message] NVARCHAR(500) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [OldValues] NVARCHAR(MAX) NULL,
    [NewValues] NVARCHAR(MAX) NULL,
    [ChangedFields] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(50) NULL
);

-- Create indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditLogsV2_Timestamp] ON [dbo].[AuditLogsV2] ([Timestamp] DESC);
CREATE NONCLUSTERED INDEX [IX_AuditLogsV2_UserId] ON [dbo].[AuditLogsV2] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogsV2_EntityId] ON [dbo].[AuditLogsV2] ([EntityId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogsV2_EntityName] ON [dbo].[AuditLogsV2] ([EntityName]);

-- Optional: Create composite index for common filter queries
CREATE NONCLUSTERED INDEX [IX_AuditLogsV2_Timestamp_EntityName] 
    ON [dbo].[AuditLogsV2] ([Timestamp] DESC, [EntityName]);
