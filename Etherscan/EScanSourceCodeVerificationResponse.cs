using Newtonsoft.Json;

namespace Compellio.DCAP.Etherscan
{
    public class EScanSourceCodeVerificationResponse : EScanResponse
    {
        [JsonProperty("result")]
        public string Guid { get; set; }
    }
}
