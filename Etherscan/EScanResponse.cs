using Newtonsoft.Json;

namespace Compellio.DCAP.Etherscan
{
    public class EScanResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public bool IsOk { get => Status == "1"; }
    }
}
