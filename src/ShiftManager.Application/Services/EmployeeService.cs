using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShiftManager.Application.Common;
using ShiftManager.Application.DTOs;
using ShiftManager.Application.DTOs.Employees;
using ShiftManager.Application.Interfaces;
using ShiftManager.Application.Services;
using ShiftManager.Common.Extensions;
using ShiftManager.Domain.Entities;
using ShiftManager.Infrastructure.Data.UnitOfWork;

namespace ShiftManager.Infrastructure.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQueryService _queryService;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IQueryService queryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _queryService = queryService;
        }

        public async Task<EmployeeApiResponse<EmployeeDto>> GetAllEmployeesAsync(
            int start,
            int size,
            string globalFilter,
            string? filters = null,
            string? sorting = null)
        {
            var query = _unitOfWork.Employees.Query();

            query = _queryService.ApplyFiltersAndSorting<Employee>(
                query, globalFilter, filters, sorting);

            var totalCount = await query.CountAsync();

            var employees = await query.Page(start, size).ToListAsync();

            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

            var response = new EmployeeApiResponse<EmployeeDto>
            {
                Data = employeeDtos,
                Meta = new MetaData
                {
                    TotalRowCount = totalCount
                }
            };

            return response;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _unitOfWork.Employees.GetByIdAsync(id);
        }

        public async Task<ServiceResult<EmployeeDto>> AddEmployeeAsync(EmployeeDto dto)
        {
            try
            {
                var employee = _mapper.Map<Employee>(dto);
                if (dto.Roles != null && dto.Roles.Any())
                {
                    var roleIds = dto.Roles.Select(r => r.Id).ToList();
                    var roles = _unitOfWork.Roles.GetAllAsync().Result.Where(r => roleIds.Contains(r.Id)).ToList();

                    employee.Roles = roles;
                }

                await _unitOfWork.Employees.AddAsync(employee);
                await _unitOfWork.CompleteAsync();

                var createdEmployeeDto = _mapper.Map<EmployeeDto>(employee);

                return ServiceResult<EmployeeDto>.Success(createdEmployeeDto, "Employee created successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.Failure(ex.Message, "An error occurred while creating the employee.");
            }
        }

        public async Task<ServiceResult<EmployeeDto>> UpdateEmployeeAsync(EmployeeDto dto)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.Id);
            if (employee == null)
            {
                return ServiceResult<EmployeeDto>.Failure("Employee not found.");
            }

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;

            await _unitOfWork.Employees.UpdateAsync(employee);
            await _unitOfWork.CompleteAsync();

            var updatedEmployeeDto = _mapper.Map<EmployeeDto>(employee);

            return ServiceResult<EmployeeDto>.Success(updatedEmployeeDto, "Employee updated successfully.");
        }

        public async Task<ServiceResult> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (employee == null)
                {
                    return ServiceResult.Failure("Employee not found", "Failed to delete employee");
                }

                await _unitOfWork.Employees.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ex.Message, "An error occurred while deleting the employee");
            }
        }

        public async Task<Employee> GetEmployeeWithRolesAsync(int id)
        {
            return await _unitOfWork.Employees.GetEmployeeWithRolesAsync(id);
        }

        public async Task<ServiceResult<EmployeeDto>> AddRolesToEmployeeAsync(int employeeId, List<int> roleIds)
        {
            try
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return ServiceResult<EmployeeDto>.Failure("Employee not found.");
                }

                var roles = await _unitOfWork.Roles.GetByIdsAsync(roleIds);
                if (roles.Count != roleIds.Count)
                {
                    return ServiceResult<EmployeeDto>.Failure("One or more roles not found.");
                }

                foreach (var role in roles)
                {
                    if (!employee.Roles.Contains(role))
                    {
                        employee.Roles.Add(role);
                    }
                }

                await _unitOfWork.CompleteAsync();

                var employeeDto = _mapper.Map<EmployeeDto>(employee);
                // employeeDto.RoleIds = employee.Roles.Select(r => r.Id).ToList();

                return ServiceResult<EmployeeDto>.Success(employeeDto, "Roles added successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.Failure(ex.Message, "An error occurred while adding roles to the employee.");
            }
        }

        public async Task<int> GetTotalEmployeeCountAsync(string globalFilter)
        {
            if (string.IsNullOrWhiteSpace(globalFilter))
            {
                return await _unitOfWork.Employees.CountAsync();
            }

            return await _unitOfWork.Employees.CountAsync(e => e.FirstName.Contains(globalFilter) ||
                                                               e.LastName.Contains(globalFilter));
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesForDropdownAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllEmployeesAsync(); 

            return employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName
            }).ToList();
        }
    }
}