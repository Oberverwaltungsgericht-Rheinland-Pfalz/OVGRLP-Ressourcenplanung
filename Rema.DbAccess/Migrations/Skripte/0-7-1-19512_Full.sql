IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [AllocationPurposes] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(3000) NULL,
    [Notes] nvarchar(3000) NULL,
    [ContactPhone] nvarchar(max) NULL,
    CONSTRAINT [PK_AllocationPurposes] PRIMARY KEY ([Id])
);

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

CREATE TABLE [Users] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Mobile] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Allocations] (
    [Id] bigint NOT NULL IDENTITY,
    [From] datetime2 NOT NULL,
    [To] datetime2 NOT NULL,
    [IsAllDay] bit NOT NULL,
    [Status] int NOT NULL,
    [RessourceId] bigint NULL,
    [PurposeId] bigint NULL,
    [CreatedById] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastModified] datetime2 NOT NULL,
    [LastModifiedById] bigint NULL,
    [ApprovedById] bigint NULL,
    [ApprovedAt] datetime2 NOT NULL,
    [ReferencePersonId] bigint NULL,
    CONSTRAINT [PK_Allocations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Allocations_Users_ApprovedById] FOREIGN KEY ([ApprovedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Allocations_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Allocations_Users_LastModifiedById] FOREIGN KEY ([LastModifiedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Allocations_AllocationPurposes_PurposeId] FOREIGN KEY ([PurposeId]) REFERENCES [AllocationPurposes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Allocations_Users_ReferencePersonId] FOREIGN KEY ([ReferencePersonId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Allocations_Ressources_RessourceId] FOREIGN KEY ([RessourceId]) REFERENCES [Ressources] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [SupplierGroups] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [GroupEmail] nvarchar(max) NULL,
    [UserId] bigint NULL,
    CONSTRAINT [PK_SupplierGroups] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SupplierGroups_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Gadgets] (
    [Id] bigint NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [SuppliedById] bigint NULL,
    [AllocationPurposeId] bigint NULL,
    [RessourceId] bigint NULL,
    CONSTRAINT [PK_Gadgets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Gadgets_AllocationPurposes_AllocationPurposeId] FOREIGN KEY ([AllocationPurposeId]) REFERENCES [AllocationPurposes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Gadgets_Ressources_RessourceId] FOREIGN KEY ([RessourceId]) REFERENCES [Ressources] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Gadgets_SupplierGroups_SuppliedById] FOREIGN KEY ([SuppliedById]) REFERENCES [SupplierGroups] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_Allocations_ApprovedById] ON [Allocations] ([ApprovedById]);

GO

CREATE INDEX [IX_Allocations_CreatedById] ON [Allocations] ([CreatedById]);

GO

CREATE INDEX [IX_Allocations_LastModifiedById] ON [Allocations] ([LastModifiedById]);

GO

CREATE INDEX [IX_Allocations_PurposeId] ON [Allocations] ([PurposeId]);

GO

CREATE INDEX [IX_Allocations_ReferencePersonId] ON [Allocations] ([ReferencePersonId]);

GO

CREATE INDEX [IX_Allocations_RessourceId] ON [Allocations] ([RessourceId]);

GO

CREATE INDEX [IX_Gadgets_AllocationPurposeId] ON [Gadgets] ([AllocationPurposeId]);

GO

CREATE INDEX [IX_Gadgets_RessourceId] ON [Gadgets] ([RessourceId]);

GO

CREATE INDEX [IX_Gadgets_SuppliedById] ON [Gadgets] ([SuppliedById]);

GO

CREATE INDEX [IX_Ressources_Name] ON [Ressources] ([Name]);

GO

CREATE INDEX [IX_SupplierGroups_UserId] ON [SupplierGroups] ([UserId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190820111245_Inital', N'3.1.0');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SupplierGroups]') AND [c].[name] = N'Title');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [SupplierGroups] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [SupplierGroups] ALTER COLUMN [Title] nvarchar(450) NOT NULL;

GO

CREATE TABLE [GadgetPurpose] (
    [GadgetId] bigint NOT NULL,
    [AllocationPurposeId] bigint NOT NULL,
    CONSTRAINT [PK_GadgetPurpose] PRIMARY KEY ([GadgetId], [AllocationPurposeId]),
    CONSTRAINT [FK_GadgetPurpose_AllocationPurposes_AllocationPurposeId] FOREIGN KEY ([AllocationPurposeId]) REFERENCES [AllocationPurposes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GadgetPurpose_Gadgets_GadgetId] FOREIGN KEY ([GadgetId]) REFERENCES [Gadgets] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_SupplierGroups_Title] ON [SupplierGroups] ([Title]);

GO

CREATE INDEX [IX_GadgetPurpose_AllocationPurposeId] ON [GadgetPurpose] ([AllocationPurposeId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190909083751_MNGadgetPurpose', N'3.1.0');

GO

ALTER TABLE [GadgetPurpose] DROP CONSTRAINT [FK_GadgetPurpose_AllocationPurposes_AllocationPurposeId];

GO

ALTER TABLE [GadgetPurpose] DROP CONSTRAINT [FK_GadgetPurpose_Gadgets_GadgetId];

GO

ALTER TABLE [Gadgets] DROP CONSTRAINT [FK_Gadgets_AllocationPurposes_AllocationPurposeId];

GO

DROP INDEX [IX_Gadgets_AllocationPurposeId] ON [Gadgets];

GO

ALTER TABLE [GadgetPurpose] DROP CONSTRAINT [PK_GadgetPurpose];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gadgets]') AND [c].[name] = N'AllocationPurposeId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Gadgets] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Gadgets] DROP COLUMN [AllocationPurposeId];

GO

EXEC sp_rename N'[GadgetPurpose]', N'GadgetPurposes';

GO

EXEC sp_rename N'[GadgetPurposes].[IX_GadgetPurpose_AllocationPurposeId]', N'IX_GadgetPurposes_AllocationPurposeId', N'INDEX';

GO

ALTER TABLE [GadgetPurposes] ADD CONSTRAINT [PK_GadgetPurposes] PRIMARY KEY ([GadgetId], [AllocationPurposeId]);

GO

ALTER TABLE [GadgetPurposes] ADD CONSTRAINT [FK_GadgetPurposes_AllocationPurposes_AllocationPurposeId] FOREIGN KEY ([AllocationPurposeId]) REFERENCES [AllocationPurposes] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [GadgetPurposes] ADD CONSTRAINT [FK_GadgetPurposes_Gadgets_GadgetId] FOREIGN KEY ([GadgetId]) REFERENCES [Gadgets] ([Id]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190909090745_MNGadgetPurpose2', N'3.1.0');

GO

EXEC sp_rename N'[Users].[Phone]', N'Organisation', N'COLUMN';

GO

EXEC sp_rename N'[Users].[Mobile]', N'ActiveDirectoryID', N'COLUMN';

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191111141659_adUser', N'3.1.0');

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Email');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Users] ALTER COLUMN [Email] nvarchar(450) NOT NULL;

GO

CREATE INDEX [IX_Users_Email] ON [Users] ([Email]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191111142046_adUser2', N'3.1.0');

GO

