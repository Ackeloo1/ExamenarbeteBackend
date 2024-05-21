namespace TestMediatR1.Player.Responses
{
    public class LoginResponse
    {
        public bool Success { get; }
        public string JwtToken { get; }

        public LoginResponse(bool success, string jwtToken)
        {
            Success = success;
            JwtToken = jwtToken;
        }
    }
}
