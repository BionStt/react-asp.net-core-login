using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Common.Response
{
    public class ItemResponse<T> : Response
    {
        public T Item { get; set; }
        public string SerializedItem { get; set; }

        public static new ItemResponse<T> Unauthorized { get { return new ItemResponse<T>(new ResponseModel(ResponseCode.Error, LogLevel.Error, "Unauthorized"), default(T)); } }

        public ItemResponse(ResponseModel response, T item) : base (response.Code, response.LogLevel, response.Message)
        {
            Item = item;
        }
    }
}
