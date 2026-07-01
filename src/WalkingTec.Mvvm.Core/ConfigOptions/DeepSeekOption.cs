namespace WalkingTec.Mvvm.Core
{
    public class DeepSeekOption
    {
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; } = "https://api.deepseek.com";
        public string Model { get; set; } = "deepseek-chat";
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 2048;
    }
}
