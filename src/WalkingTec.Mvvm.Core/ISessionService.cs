namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Session接口
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// 根据Key从Session中获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">Session键</param>
        /// <returns>获取到的数据</returns>
        T Get<T>(string key);
        /// <summary>
        /// 向Session中写入数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">Session键</param>
        /// <param name="val">要写入的数据</param>
        void Set<T>(string key, T val);
        /// <summary>
        /// Session的唯一标识
        /// </summary>
        string SessionId { get; }
    }
}
