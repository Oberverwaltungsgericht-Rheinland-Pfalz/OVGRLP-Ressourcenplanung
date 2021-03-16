IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Ressources] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(450) NOT NULL,
    [FunctionDescription] nvarchar(max) NULL,
    [Usability] nvarchar(max) NULL,
    [SpecialsDescription] nvarchar(max) NULL,
    [Type] nvarchar(max) NULL,
    CONSTRAINT [PK_Ressources] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SupplierGroups] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(450) NOT NULL,
    [GroupEmail] nvarchar(max) NULL,
    CONSTRAINT [PK_SupplierGroups] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] bigint NOT NULL IDENTITY,
    [ActiveDirectoryID] nvarchar(max) NULL,
    [Name] nvarchar(max) NOT NULL,
    [Organisation] nvarchar(max) NULL,
    [Email] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Gadgets] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [SuppliedById] bigint NULL,
    [RessourceId] bigint NULL,
    CONSTRAINT [PK_Gadgets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Gadgets_Ressources_RessourceId] FOREIGN KEY ([RessourceId]) REFERENCES [Ressources] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Gadgets_SupplierGroups_SuppliedById] FOREIGN KEY ([SuppliedById]) REFERENCES [SupplierGroups] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Allocations] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [ContactPhone] nvarchar(max) NULL,
    [Notes] nvarchar(3000) NULL,
    [From] datetime2 NOT NULL,
    [To] datetime2 NOT NULL,
    [IsAllDay] bit NOT NULL,
    [Status] int NOT NULL,
    [RessourceId] bigint NULL,
    [ScheduleSeriesGuid] uniqueidentifier NULL,
    [CreatedById] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastModified] datetime2 NOT NULL,
    [LastModifiedById] bigint NULL,
    [ApprovedById] bigint NULL,
    [ApprovedAt] datetime2 NOT NULL,
    [ReferencePersonId] bigint NULL,
    [SerializedHints] nvarchar(max) NULL,
    CONSTRAINT [PK_Allocations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Allocations_Users_ApprovedById] FOREIGN KEY ([ApprovedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Allocations_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Allocations_Users_LastModifiedById] FOREIGN KEY ([LastModifiedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Allocations_Users_ReferencePersonId] FOREIGN KEY ([ReferencePersonId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Allocations_Ressources_RessourceId] FOREIGN KEY ([RessourceId]) REFERENCES [Ressources] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AllocationGagdet] (
    [AllocationId] bigint NOT NULL,
    [GadgetId] bigint NOT NULL,
    CONSTRAINT [PK_AllocationGagdet] PRIMARY KEY ([AllocationId], [GadgetId]),
    CONSTRAINT [FK_AllocationGagdet_Allocations_AllocationId] FOREIGN KEY ([AllocationId]) REFERENCES [Allocations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AllocationGagdet_Gadgets_GadgetId] FOREIGN KEY ([GadgetId]) REFERENCES [Gadgets] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AllocationGagdet_GadgetId] ON [AllocationGagdet] ([GadgetId]);
GO

CREATE INDEX [IX_Allocations_ApprovedById] ON [Allocations] ([ApprovedById]);
GO

CREATE INDEX [IX_Allocations_CreatedById] ON [Allocations] ([CreatedById]);
GO

CREATE INDEX [IX_Allocations_LastModifiedById] ON [Allocations] ([LastModifiedById]);
GO

CREATE INDEX [IX_Allocations_ReferencePersonId] ON [Allocations] ([ReferencePersonId]);
GO

CREATE INDEX [IX_Allocations_RessourceId] ON [Allocations] ([RessourceId]);
GO

CREATE INDEX [IX_Gadgets_RessourceId] ON [Gadgets] ([RessourceId]);
GO

CREATE INDEX [IX_Gadgets_SuppliedById] ON [Gadgets] ([SuppliedById]);
GO

CREATE INDEX [IX_Ressources_Name] ON [Ressources] ([Name]);
GO

CREATE INDEX [IX_SupplierGroups_Title] ON [SupplierGroups] ([Title]);
GO

CREATE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200429075105_Initial_0-9-6', N'5.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Allocations] DROP CONSTRAINT [FK_Allocations_Ressources_RessourceId];
GO

ALTER TABLE [Allocations] DROP CONSTRAINT [FK_Allocations_Users_CreatedById];
GO

DROP INDEX [IX_Allocations_RessourceId] ON [Allocations];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Allocations]') AND [c].[name] = N'RessourceId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Allocations] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Allocations] ALTER COLUMN [RessourceId] bigint NOT NULL;
ALTER TABLE [Allocations] ADD DEFAULT CAST(0 AS bigint) FOR [RessourceId];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Allocations]') AND [c].[name] = N'CreatedById');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Allocations] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Allocations] ALTER COLUMN [CreatedById] bigint NULL;
GO

CREATE TABLE [AllocationRessource] (
    [AllocationsId] bigint NOT NULL,
    [RessourcesId] bigint NOT NULL,
    CONSTRAINT [PK_AllocationRessource] PRIMARY KEY ([AllocationsId], [RessourcesId]),
    CONSTRAINT [FK_AllocationRessource_Allocations_AllocationsId] FOREIGN KEY ([AllocationsId]) REFERENCES [Allocations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AllocationRessource_Ressources_RessourcesId] FOREIGN KEY ([RessourcesId]) REFERENCES [Ressources] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AllocationRessource_RessourcesId] ON [AllocationRessource] ([RessourcesId]);
GO

ALTER TABLE [Allocations] ADD CONSTRAINT [FK_Allocations_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201119110627_2-0-Multiroom', N'5.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Allocations] ADD [Reminded] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210122151207_reminderUpdate', N'5.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AllocationGagdet] DROP CONSTRAINT [FK_AllocationGagdet_Allocations_AllocationId];
GO

ALTER TABLE [AllocationGagdet] DROP CONSTRAINT [FK_AllocationGagdet_Gadgets_GadgetId];
GO

ALTER TABLE [AllocationGagdet] DROP CONSTRAINT [PK_AllocationGagdet];
GO

EXEC sp_rename N'[AllocationGagdet]', N'AllocationGagdetOld';
GO

EXEC sp_rename N'[AllocationGagdetOld].[IX_AllocationGagdet_GadgetId]', N'IX_AllocationGagdetOld_GadgetId', N'INDEX';
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gadgets]') AND [c].[name] = N'Title');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Gadgets] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Gadgets] ALTER COLUMN [Title] nvarchar(max) NOT NULL;
ALTER TABLE [Gadgets] ADD DEFAULT N'' FOR [Title];
GO

ALTER TABLE [AllocationGagdetOld] ADD CONSTRAINT [PK_AllocationGagdetOld] PRIMARY KEY ([AllocationId], [GadgetId]);
GO

CREATE TABLE [AllocationGadget] (
    [AllocationsId] bigint NOT NULL,
    [GadgetsId] bigint NOT NULL,
    CONSTRAINT [PK_AllocationGadget] PRIMARY KEY ([AllocationsId], [GadgetsId]),
    CONSTRAINT [FK_AllocationGadget_Allocations_AllocationsId] FOREIGN KEY ([AllocationsId]) REFERENCES [Allocations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AllocationGadget_Gadgets_GadgetsId] FOREIGN KEY ([GadgetsId]) REFERENCES [Gadgets] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AllocationGadget_GadgetsId] ON [AllocationGadget] ([GadgetsId]);
GO

ALTER TABLE [AllocationGagdetOld] ADD CONSTRAINT [FK_AllocationGagdetOld_Allocations_AllocationId] FOREIGN KEY ([AllocationId]) REFERENCES [Allocations] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [AllocationGagdetOld] ADD CONSTRAINT [FK_AllocationGagdetOld_Gadgets_GadgetId] FOREIGN KEY ([GadgetId]) REFERENCES [Gadgets] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210218004403_allocationGadgetDirect', N'5.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Gadgets] DROP CONSTRAINT [FK_Gadgets_Ressources_RessourceId];
GO

DROP TABLE [AllocationGagdetOld];
GO

DROP INDEX [IX_Gadgets_RessourceId] ON [Gadgets];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ressources]') AND [c].[name] = N'Usability');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Ressources] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Ressources] DROP COLUMN [Usability];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gadgets]') AND [c].[name] = N'RessourceId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Gadgets] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Gadgets] DROP COLUMN [RessourceId];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Allocations]') AND [c].[name] = N'RessourceId');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Allocations] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Allocations] DROP COLUMN [RessourceId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210218160850_cleanUpDB2', N'5.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Ressources] ADD [IsDeactivated] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Gadgets] ADD [IsDeactivated] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210316091644_deactivatableRessources', N'5.0.0');
GO

COMMIT;
GO

