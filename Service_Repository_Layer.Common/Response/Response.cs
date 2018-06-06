using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Common.Response
{
    public class Response : ResponseModel
    {
        public static Response Unauthorized { get { return new Response(ResponseCode.Error, LogLevel.Error, "Unauthorized"); } }

        public Response(ResponseCode code, LogLevel logLevel, string message) : base(code, logLevel, message)
        {
        }

        public Response() : base()
        {
        }
    }
}
