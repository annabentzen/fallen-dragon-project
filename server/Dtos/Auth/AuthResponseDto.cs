namespace DragonGame.Dtos.Auth
{
    /// <summary>
    /// Response returned after successful login/registration
    /// </summary>
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}