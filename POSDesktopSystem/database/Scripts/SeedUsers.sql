-- cashier / cashier123
-- manager / manager123

USE POSDesktopSystem;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'cashier')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, Role)
    VALUES (
        N'cashier',
        N'b4c94003c562bb0d89535eca77f07284fe560fd48a7cc1ed99f0a56263d616ba',
        1
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'manager')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, Role)
    VALUES (
        N'manager',
        N'866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5',
        2
    );
END
GO
