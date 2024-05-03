using System.IdentityModel.Tokens.Jwt;

namespace TestMediatR1.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetClaim(this HttpContext context, string type)
        {
            string jwt = context.Request.Headers["Authorization"];
    
            if (jwt == null)
                return null!;

            string[] jwtItems = new string[]
            {
                string.Empty,
                jwt
            };

            if (jwt.Contains("Bearer "))
            {
                jwtItems = jwt.Split("Bearer ");
            }

            if (jwtItems.Length != 2) 
                return null!;

            JwtSecurityToken decodedToken = new JwtSecurityToken(jwtItems[1]);

            return decodedToken.Claims.FirstOrDefault(c => c.Type == type)?.Value!;
        }
    }
}