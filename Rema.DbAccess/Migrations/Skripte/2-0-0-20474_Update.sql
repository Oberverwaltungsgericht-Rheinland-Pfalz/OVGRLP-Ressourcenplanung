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

