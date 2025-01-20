using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShiftManager.Application.Common;
using ShiftManager.Application.DTOs;
using ShiftManager.Application.DTOs.Roles;
using ShiftManager.Application.Interfaces;
using ShiftManager.Application.Services;
using ShiftManager.Common.Extensions;
using ShiftManager.Domain.Entities;
using ShiftManager.Infrastructure.Data.UnitOfWork;

namespace ShiftManager.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQueryService _queryService;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper, IQueryService queryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _queryService = queryService;
        }
        public async Task<RoleApiResponse<RoleDto>> GetAllRolesAsync(
            int start,
            int size,
            string? globalFilter = null,
            string? filters = null,
            string? sorting = null)
        {
            var query = _unitOfWork.Roles.Query();

            query = _queryService.ApplyFiltersAndSorting<Role>(query, globalFilter, filters, sorting);

            var totalCount = await query.CountAsync(); 
            var roles =  await query.Page(start, size).ToListAsync();

            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);

            return new RoleApiResponse<RoleDto>
            {
                Data = roleDtos,
                Meta = new MetaData { TotalRowCount = totalCount }
            };
        }

        public async Task<ServiceResult<RoleDto>> GetRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);

            if (role == null)
            {
                return ServiceResult<RoleDto>.Failure($"Role with ID {id} not found.");
            }

            return ServiceResult<RoleDto>.Success(_mapper.Map<RoleDto>(role), "Role retrieved successfully.");
        }

        public async Task<ServiceResult<RoleDto>> AddRoleAsync(RoleDto roleDto)
        {
            try
            {
                var role = _mapper.Map<Role>(roleDto);

                await _unitOfWork.Roles.AddAsync(role);
                await _unitOfWork.CompleteAsync();

                var createdRoleDto = _mapper.Map<RoleDto>(role);

                return ServiceResult<RoleDto>.Success(createdRoleDto, "Role added successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<RoleDto>.Failure(ex.Message, "An error occurred while adding the role.");
            }
        }

        // Актуализира съществуваща роля
        public async Task<ServiceResult> UpdateRoleAsync(RoleDto roleDto)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleDto.Id);
                if (role == null)
                {
                    return ServiceResult.Failure($"Role with ID {roleDto.Id} not found.");
                }

                _mapper.Map(roleDto, role);

                await _unitOfWork.Roles.UpdateAsync(role);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success("Role updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ex.Message, "An error occurred while updating the role.");
            }
        }

        // Изтрива роля
        public async Task<ServiceResult> DeleteRoleAsync(int id)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return ServiceResult.Failure($"Role with ID {id} not found.");
                }

                await _unitOfWork.Roles.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success("Role deleted successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ex.Message, "An error occurred while deleting the role.");
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleDto>>> GetRolesByEmployeeIdAsync(int employeeId)
        {
            var roles = await _unitOfWork.Roles.GetRolesByEmployeeIdAsync(employeeId);
            if (roles == null || !roles.Any())
            {
                return ServiceResult<IEnumerable<RoleDto>>.Failure("No roles found for the specified employee.");
            }

            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            });

            return ServiceResult<IEnumerable<RoleDto>>.Success(roleDtos);
        }

        public async Task<ServiceResult<IEnumerable<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            if (roles == null || !roles.Any())
            {
                return ServiceResult<IEnumerable<RoleDto>>.Failure("No roles found.");
            }

            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            });

            return ServiceResult<IEnumerable<RoleDto>>.Success(roleDtos);
        }
    }
}
