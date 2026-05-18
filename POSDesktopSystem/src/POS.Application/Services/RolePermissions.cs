using POS.Application.DTOs;
using POS.Domain.Enums;

namespace POS.Application.Services;

public static class RolePermissions
{
    public static bool CanProcessSales(UserRole role) =>
        role is UserRole.Cashier or UserRole.Manager;

    public static bool CanManageProducts(UserRole role) =>
        role == UserRole.Manager;

    public static bool CanViewInvoiceHistory(UserRole role) =>
        role == UserRole.Manager;

    public static bool CanProcessSales(AuthenticatedUserDto user) =>
        user.IsCashier || user.IsManager;

    public static bool CanManageProducts(AuthenticatedUserDto user) =>
        user.IsManager;

    public static bool CanViewInvoiceHistory(AuthenticatedUserDto user) =>
        user.IsManager;

    public static bool CanViewReports(UserRole role) =>
        role == UserRole.Manager;

    public static bool CanManageUsers(UserRole role) =>
        role == UserRole.Manager;

    public static string GetRoleDisplayName(UserRole role) =>
        role switch
        {
            UserRole.Cashier => "Cashier",
            UserRole.Manager => "Manager",
            _ => role.ToString()
        };
}
