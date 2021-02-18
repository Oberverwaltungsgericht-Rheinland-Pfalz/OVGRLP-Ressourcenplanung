BEGIN TRANSACTION;
GO

ALTER TABLE [Gadgets] DROP CONSTRAINT [FK_Gadgets_Ressources_RessourceId];
GO

DROP TABLE [AllocationGagdetOld];
GO

DROP INDEX [IX_Gadgets_RessourceId] ON [Gadgets];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ressources]') AND [c].[name] = N'Usability');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Ressources] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Ressources] DROP COLUMN [Usability];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gadgets]') AND [c].[name] = N'RessourceId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Gadgets] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Gadgets] DROP COLUMN [RessourceId];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Allocations]') AND [c].[name] = N'RessourceId');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Allocations] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Allocations] DROP COLUMN [RessourceId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210218160850_cleanUpDB2', N'5.0.0');
GO

COMMIT;
GO

