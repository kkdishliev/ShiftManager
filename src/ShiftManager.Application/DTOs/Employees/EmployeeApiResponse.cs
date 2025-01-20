namespace ShiftManager.Application.DTOs.Employees
{
    public class EmployeeApiResponse<T>
    {
        public required IEnumerable<T> Data { get; set; }
        public required MetaData Meta { get; set; }
    }
}