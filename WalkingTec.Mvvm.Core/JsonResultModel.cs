using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DataTableResult
    /// </summary>
    public class DataTableResult<T> : JsonResultT<IEnumerable<T>>
        where T : TopBasePoco
    {
        /// <summary>
        /// Data Count
        /// </summary>
        public long Count { get; set; }
    }

    /// <summary>
    /// JsonResultT
    /// </summary>
    public class JsonResultT<T>
    {
        /// <summary>
        /// Status Code
        /// <see cref="Microsoft.AspNetCore.Http.StatusCodes">
        /// </summary>
        public int Code { get; set; }
        
        /// <summary>
        /// Message
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public T Data { get; set; }
    }
}
