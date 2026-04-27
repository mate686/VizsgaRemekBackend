namespace VizsgaRemekBackend.Dtos.AuthDtos
{
    public class RegisterDto
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string? PhoneNumber { get; set; }
    }
}
