using ShiftManager.Application.Common;
using ShiftManager.Application.DTOs.Roles;

namespace ShiftManager.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleApiResponse<RoleDto>> GetAllRolesAsync(
            int start,
            int size,
            string? globalFilter = null,
            string? filters = null,
            string? sorting = null);
        Task<ServiceResult<IEnumerable<RoleDto>>> GetAllRolesAsync();
        Task<ServiceResult<RoleDto>> GetRoleByIdAsync(int id);
        Task<ServiceResult<RoleDto>> AddRoleAsync(RoleDto roleDto);
        Task<ServiceResult> UpdateRoleAsync(RoleDto roleDto);
        Task<ServiceResult> DeleteRoleAsync(int id);
        Task<ServiceResult<IEnumerable<RoleDto>>> GetRolesByEmployeeIdAsync(int employeeId);
    }
}