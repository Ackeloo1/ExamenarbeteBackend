


namespace TestMediatR1.Player.Responses
{
    public class RegisterResponse
    {
        public bool Success { get; }
        public string Message { get; }

        public RegisterResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
