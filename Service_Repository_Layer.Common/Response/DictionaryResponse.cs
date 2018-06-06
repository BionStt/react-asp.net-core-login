using Service_Repository_Layer.Enums;

using System.Collections.Generic;

namespace Service_Repository_Layer.Common.Response
{
    public class DictionaryResponse<T1, T2> : Response
    {
        public Dictionary<T1, T2> Items { get; set; }

        public static new DictionaryResponse<T1, T2> Unauthorized { get { return new DictionaryResponse<T1, T2>(new ResponseModel(ResponseCode.Error, LogLevel.Error, "Unauthorized"), null); } }

        public DictionaryResponse(ResponseModel response, Dictionary<T1, T2> items)
            : base(response.Code, response.LogLevel, response.Message)
        {
            Items = items;
        }
    }
}
