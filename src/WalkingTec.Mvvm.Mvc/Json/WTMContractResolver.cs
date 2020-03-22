using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Json
{
    /// <summary>
    /// WTMContractResolver
    /// </summary>
    public class WTMContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// ResolveContractConverter
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(DateRange))
            {
                return new DateRangeConvert();
            }
            else
            {
                if (objectType != typeof(string))
                {
                    return base.ResolveContractConverter(objectType);
                }

                return new StringIgnoreLTGTConvert();
            }
        }
    }

}
