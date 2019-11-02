using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WalkingTec.Mvvm.Core.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expires { get; set; }
        public string SecurityKey { get; set; }
        public string LoginUrl { get; set; }
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
