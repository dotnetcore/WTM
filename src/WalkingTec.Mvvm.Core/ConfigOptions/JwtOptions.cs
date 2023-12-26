using System.Text;

namespace WalkingTec.Mvvm.Core
{
    public class JwtOption
    {
        public string Issuer { get; set; } = "http://localhost";
        public string Audience { get; set; } = "http://localhost";
        public int Expires { get; set; } = 3600;
        private string _securiteKey= "wtmwtmwtmwtmwtmwtm";
        public string SecurityKey { get
            {
                return _securiteKey;
            }
            set {
                _securiteKey = value;
                if (_securiteKey.Length < 32)
                {
                    var count = 32 - _securiteKey.Length;
                    for (int i = 0; i < count; i++)
                    {
                        _securiteKey += "x";
                    }
                }

            }
        }
        public string LoginPath { get; set; }
    }
}
