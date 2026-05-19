-- Migration: add UserId and CreatedAt to existing Invoices table
USE POSDesktopSystem;
GO

IF COL_LENGTH('dbo.Invoices', 'UserId') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices ADD UserId INT NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices ADD CreatedAt DATETIME2 NULL;
END
GO

UPDATE dbo.Invoices
SET CreatedAt = Date
WHERE CreatedAt IS NULL;
GO

UPDATE dbo.Invoices
SET UserId = (SELECT TOP 1 Id FROM dbo.Users ORDER BY Id)
WHERE UserId IS NULL AND EXISTS (SELECT 1 FROM dbo.Users);
GO

IF COL_LENGTH('dbo.Invoices', 'UserId') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Invoices ALTER COLUMN UserId INT NOT NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'CreatedAt') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Invoices ALTER COLUMN CreatedAt DATETIME2 NOT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Invoices_Users')
BEGIN
    ALTER TABLE dbo.Invoices
        ADD CONSTRAINT FK_Invoices_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (Id);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Invoices_UserId' AND object_id = OBJECT_ID(N'dbo.Invoices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Invoices_UserId ON dbo.Invoices (UserId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Invoices_Date' AND object_id = OBJECT_ID(N'dbo.Invoices'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Invoices_Date ON dbo.Invoices (Date);
END
GO
