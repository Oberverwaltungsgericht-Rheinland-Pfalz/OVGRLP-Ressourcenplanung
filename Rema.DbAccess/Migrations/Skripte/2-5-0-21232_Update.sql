BEGIN TRANSACTION;
GO

ALTER TABLE [SupplierGroups] ADD [Remind] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Allocations] ADD [SupportersReminded] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210607130426_remindSupporters', N'5.0.0');
GO

COMMIT;
GO

