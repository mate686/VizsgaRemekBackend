namespace VizsgaRemekBackend.Dtos.AuthDtos
{
    public class UpdateProfileDto
    {
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
