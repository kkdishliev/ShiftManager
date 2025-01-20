using AutoMapper;
using ShiftManager.Domain.Entities;
using ShiftManager.Application.DTOs.Employees;
using ShiftManager.Application.DTOs.Roles;
using ShiftManager.Application.DTOs.Shifts;

namespace ShiftManager.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ReverseMap(); 

            CreateMap<Role, RoleDto>()
                .ReverseMap(); 

            CreateMap<Shift, ShiftDto>()
                .ReverseMap().ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<Shift, ShiftCreateDto>()
              .ReverseMap();

            CreateMap<Shift, ShiftUpdateDto>()
              .ReverseMap();

            CreateMap<Employee, EmployeeShiftsDto>();
        }
    }
}