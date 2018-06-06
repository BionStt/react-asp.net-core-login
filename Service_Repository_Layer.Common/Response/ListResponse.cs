using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Service_Repository_Layer.Enums;
using System.Collections.Generic;

namespace Service_Repository_Layer.Common.Response
{
    public class ListResponse<T> : Response
    {
        public List<T> Items { get; set; }
        public int? PaginationCount { get; set; }
        public int? TotalCount { get; set; }

        public static new ListResponse<T> Unauthorized { get { return new ListResponse<T>(new ResponseModel(ResponseCode.Error, LogLevel.Error, "Unauthorized"), null); } }

        public ListResponse(ResponseModel response, List<T> items) : base(response.Code, response.LogLevel, response.Message)
        {
            Items = items;            
        }
    }
}
