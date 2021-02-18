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

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gadgets]') AND [c].[name] = N'Title');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Gadgets] DROP CONSTRAINT [' + @var0 + '];');
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

