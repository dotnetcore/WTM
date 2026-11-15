namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// AI代码功能配置，对应 appsettings.json 的 AiCode 节
    /// </summary>
    public class AiCodeOptions
    {
        private bool? _enabled;

        /// <summary>
        /// 是否启用AI代码功能，默认启用
        /// </summary>
        public bool Enabled
        {
            get { return _enabled ?? true; }
            set { _enabled = value; }
        }

        private bool? _reviewRequired;

        /// <summary>
        /// 保存前AI安全审查是否强制：true时审查服务不可用则拒绝保存，默认true
        /// </summary>
        public bool ReviewRequired
        {
            get { return _reviewRequired ?? true; }
            set { _reviewRequired = value; }
        }

        private int? _timeoutSeconds;

        /// <summary>
        /// AI代码执行超时秒数，默认5秒
        /// </summary>
        public int TimeoutSeconds
        {
            get { return _timeoutSeconds ?? 5; }
            set { _timeoutSeconds = value; }
        }
    }
}
