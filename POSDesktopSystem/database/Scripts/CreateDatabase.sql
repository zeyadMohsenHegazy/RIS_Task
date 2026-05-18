-- POS Desktop System - SQL Server Database Schema
-- Run this script to create the database and tables manually.

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'POSDesktopSystem')
BEGIN
    CREATE DATABASE POSDesktopSystem;
END
GO

USE POSDesktopSystem;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id            INT            IDENTITY(1, 1) NOT NULL,
        Username      NVARCHAR(100)  NOT NULL,
        PasswordHash  NVARCHAR(256)  NOT NULL,
        Role          INT            NOT NULL,
        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Users_Username UNIQUE (Username)
    );
END
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        Id             INT            IDENTITY(1, 1) NOT NULL,
        Name           NVARCHAR(200)  NOT NULL,
        Barcode        NVARCHAR(50)   NOT NULL,
        Price          DECIMAL(18, 2) NOT NULL,
        StockQuantity  INT            NOT NULL,
        CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Products_Barcode UNIQUE (Barcode)
    );
END
GO

IF OBJECT_ID(N'dbo.Invoices', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Invoices
    (
        Id             INT            IDENTITY(1, 1) NOT NULL,
        UserId         INT            NOT NULL,
        Date           DATETIME2      NOT NULL,
        CreatedAt      DATETIME2      NOT NULL,
        TotalAmount    DECIMAL(18, 2) NOT NULL,
        Discount       DECIMAL(18, 2) NOT NULL,
        Tax            DECIMAL(18, 2) NOT NULL,
        FinalAmount    DECIMAL(18, 2) NOT NULL,
        PaymentMethod  INT            NOT NULL,
        CONSTRAINT PK_Invoices PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Invoices_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (Id)
    );
END
GO

CREATE NONCLUSTERED INDEX IX_Invoices_UserId
    ON dbo.Invoices (UserId);
GO

CREATE NONCLUSTERED INDEX IX_Invoices_Date
    ON dbo.Invoices (Date);
GO

IF OBJECT_ID(N'dbo.InvoiceItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InvoiceItems
    (
        Id         INT            IDENTITY(1, 1) NOT NULL,
        InvoiceId  INT            NOT NULL,
        ProductId  INT            NOT NULL,
        Quantity   INT            NOT NULL,
        UnitPrice  DECIMAL(18, 2) NOT NULL,
        SubTotal   DECIMAL(18, 2) NOT NULL,
        CONSTRAINT PK_InvoiceItems PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_InvoiceItems_Invoices FOREIGN KEY (InvoiceId)
            REFERENCES dbo.Invoices (Id) ON DELETE CASCADE,
        CONSTRAINT FK_InvoiceItems_Products FOREIGN KEY (ProductId)
            REFERENCES dbo.Products (Id)
    );
END
GO

CREATE NONCLUSTERED INDEX IX_InvoiceItems_InvoiceId
    ON dbo.InvoiceItems (InvoiceId);
GO

CREATE NONCLUSTERED INDEX IX_InvoiceItems_ProductId
    ON dbo.InvoiceItems (ProductId);
GO
