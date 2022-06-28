namespace DaprExtention.DataClass
{
    public enum DaprResultCode
    {
        Non,
        OK,
        Fail
    }

    public class DaprResponse
    {
        public DaprResultCode ResultCode { get; set;}

        public string Message { get; set; }

        public dynamic DetailJsonString { get; set; }
    }
}