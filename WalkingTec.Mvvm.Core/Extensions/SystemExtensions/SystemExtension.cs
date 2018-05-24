using System;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// System Extension
    /// </summary>
    public static class SystemExtension
    {
        #region Guid Extensions

        public static string ToNoSplitString(this Guid self)
        {
            return self.ToString().Replace("-", string.Empty);
        }

        #endregion
    }
}
