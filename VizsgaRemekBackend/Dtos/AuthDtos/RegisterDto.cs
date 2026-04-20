namespace VizsgaRemekBackend.Dtos.AuthDtos
{
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        //public int Points { get; set; } = 0;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string? PhoneNumber { get; set; }
    }
}
