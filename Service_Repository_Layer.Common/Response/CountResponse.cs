using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Common.Response
{
    public class CountResponse : Response
    {
        public int Count { get; set; }

        public static new CountResponse Unauthorized { get { return new CountResponse(new ResponseModel(ResponseCode.Error, LogLevel.Error, "Unauthorized"), 0); } }

        public CountResponse(ResponseModel response, int count) : base(response.Code, response.LogLevel, response.Message)
        {
            Count = count;
        }
    }
}
