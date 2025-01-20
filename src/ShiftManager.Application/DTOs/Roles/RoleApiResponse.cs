namespace ShiftManager.Application.DTOs.Roles
{
    public class RoleApiResponse<T>
    {
        public required IEnumerable<T> Data { get; set; }
        public required MetaData Meta { get; set; }
    }
}