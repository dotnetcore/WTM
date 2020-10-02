using System.Text;

namespace WalkingTec.Mvvm.Core
{
    public class JwtOption
    {
        public string Issuer { get; set; } = "http://localhost";
        public string Audience { get; set; } = "http://localhost";
        public int Expires { get; set; } = 3600;
        public string SecurityKey { get; set; } = "wtm";
        public string LoginPath { get; set; }
        public int RefreshTokenExpires { get; set; }
    }
}
