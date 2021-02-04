BEGIN TRANSACTION;
GO

ALTER TABLE [Allocations] ADD [Reminded] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210122151207_reminderUpdate', N'5.0.0');
GO

COMMIT;
GO

