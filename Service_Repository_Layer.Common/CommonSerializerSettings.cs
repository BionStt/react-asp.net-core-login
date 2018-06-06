using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Service_Repository_Layer.Common
{
    public static class CommonSerializerSettings
    {
        public static JsonSerializerSettings GetSettings()
        {
            var _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            _serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            _serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            _serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return _serializerSettings;
        }
    }
}
