using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WalkingTec.Mvvm.Core.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = "http://localhost";
        public string Audience { get; set; } = "http://localhost";
        public int Expires { get; set; } = 3600;
        public string SecurityKey { get; set; } = "wtm";
        public string LoginPath { get; set; }
        private SecurityKey _symmetricSecurityKey;

        public SecurityKey SymmetricSecurityKey
        {
            get
            {
                if (_symmetricSecurityKey == null)
                {
                    _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
                }
                return _symmetricSecurityKey;
            }
        }
        public int RefreshTokenExpires { get; set; }
    }
}
