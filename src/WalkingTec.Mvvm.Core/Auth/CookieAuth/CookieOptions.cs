namespace WalkingTec.Mvvm.Core.Auth
{
    public class CookieOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expires { get; set; }
        public bool SlidingExpiration { get; set; } = true;
    }
}
