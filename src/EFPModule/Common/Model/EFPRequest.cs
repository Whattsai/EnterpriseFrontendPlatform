namespace Common.Model
{
    public class EFPRequest
    {
        public EFPRequest()
        {
        }

        public EFPRequest(EFPRequest request, string id)
        {
            Token = request.Token;
            ID = id;
            Data = request.Data;
        }

        public Guid? Token { get; set; }

        public string ID { get; set; }

        public object Data { get; set; }
    }
}
