ALTER TABLE [SupplierGroups] DROP CONSTRAINT [FK_SupplierGroups_Users_UserId];

GO

DROP INDEX [IX_SupplierGroups_UserId] ON [SupplierGroups];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SupplierGroups]') AND [c].[name] = N'UserId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [SupplierGroups] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [SupplierGroups] DROP COLUMN [UserId];

GO

ALTER TABLE [Allocations] ADD [SerializedHints] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200327092116_hintsforsuppliers', N'3.1.0');

GO

