using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Compellio.DCAP.Etherscan
{
    public class EtherscanContractVerifier
    {
        public string BaseUrl { get; }
        public string ApiKey { get; }

        public EtherscanContractVerifier(string baseUrl, string apiKey)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
        }

        /// <summary>
        /// Submit a contract source code to Etherscan for verification.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<EScanSourceCodeVerificationResponse> VerifySmartContractAsync(EScanContractCodeVerificationModel payload)
        {
            return await SendAsync(payload);
        }

        internal async Task<EScanSourceCodeVerificationResponse> SendAsync<T>(T payload)
        {
            HttpClient client = new HttpClient();
            string requestUrl = BaseUrl;

            //if (EScanClient.ThrottleMs.HasValue)
            //{
            //    await Task.Delay(EScanClient.ThrottleMs.Value);
            //}

            string payloadJson = JsonConvert.SerializeObject(payload);
            IDictionary<string, string> payloadDict = JsonConvert.DeserializeObject<IDictionary<string, string>>(payloadJson).Where(x => x.Value != null).ToDictionary<string, string>();
            payloadDict.Add("apikey", ApiKey);
            payloadDict.Add("module", "contract");
            payloadDict.Add("action", "verifysourcecode");

            HttpContent content = new FormUrlEncodedContent(payloadDict);

            try
            {
                HttpResponseMessage result = await client.PostAsync(requestUrl, content);
                if (!result.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Server issue (" + result.StatusCode + "): " + result.ReasonPhrase);
                }

                string resultContent = await result.Content.ReadAsStringAsync();

                var responseObject = JsonConvert.DeserializeObject(resultContent, typeof(EScanSourceCodeVerificationResponse));
                return (EScanSourceCodeVerificationResponse)responseObject;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
