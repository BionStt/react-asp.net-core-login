using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Common.Response
{
    public class ResponseModel
    {
        public ResponseCode Code { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }

        public ResponseModel(LogLevel logLevel, string message) : this(0, logLevel, message) { }

        public ResponseModel() : this(ResponseCode.Success, LogLevel.Information, null) { }

        public ResponseModel(ResponseCode code, LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
            Code = code;
        }
    }
}
