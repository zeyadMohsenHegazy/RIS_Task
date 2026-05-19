using POS.Application.DTOs;

namespace POS.Application.Interfaces;

public interface ISystemStatusProvider
{
    SystemStatusDto GetStatus();
}
