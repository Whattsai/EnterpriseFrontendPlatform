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

        [DataMember]
        public object Data { get; set; }
    }
}
