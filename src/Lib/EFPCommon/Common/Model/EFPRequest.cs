using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Common.Model
{
    public class EFPRequest
    {
        [JsonConstructor]
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

        public string? Service { get;set; }

        [DataMember]
        public dynamic Data { get; set; }
    }
}
