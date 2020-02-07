ALTER TABLE [Gadgets] DROP CONSTRAINT [FK_Gadgets_Allocations_AllocationId];

GO

DROP INDEX [IX_Gadgets_AllocationId] ON [Gadgets];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gadgets]') AND [c].[name] = N'AllocationId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Gadgets] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Gadgets] DROP COLUMN [AllocationId];

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

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200204095700_AllocationReplacePurpose', N'3.1.0');

GO

