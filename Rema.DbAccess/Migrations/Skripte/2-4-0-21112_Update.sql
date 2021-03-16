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

