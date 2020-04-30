IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

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
VALUES (N'20200429075105_Initial_0-9-6', N'3.1.0');

GO

