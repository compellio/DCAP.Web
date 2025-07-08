using Compellio.DCAP.SmartContracts.DCAPv2;
using Compellio.DCAP.SmartContracts.DCAPv2.ContractDefinition;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Compellio.DCAP.Web.RestApi.Models.V1
{
    public class SelectiveDisclosureHash
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
    }

    public class TAR
    {
        private static string TARIndexFilenameLocal = "TARReceipts.csv";
        public static string StoragePath = String.Empty;
        public static string PrivateKey = String.Empty;
        public static string RPCEndPoint = String.Empty;
        public static string ChainId = String.Empty;
        public static string UriPrefix = String.Empty;
        public static string TARIndexFilename => StoragePath + TARIndexFilenameLocal;
        public string Id { get; set; }
        public string Receipt { get; set; }
        public JsonDocument Data { get; set; }
        public string Checksum { get; set; }
        public int Version { get; set; }
        public SelectiveDisclosureHash[] _sdHashes { get; set; }

        public static IList<TAR> TARs => File.Exists(TARIndexFilename) ? File.ReadAllLines(TARIndexFilename).Select(x => x.Split(';')).Select(x => new TAR()
        {
            Id = x[0],
            Receipt = x[1],
            Checksum = x[2],
            Data = JsonDocument.Parse(File.ReadAllText($"{StoragePath}{x[1]}.json")),
            Version = Int32.Parse(x[3]),
        }).ToArray() : new List<TAR>();

        public static void SaveTARs(IList<TAR> tars) => File.WriteAllLines(TARIndexFilename, tars.Select(x => $"{x.Id};{x.Receipt};{x.Checksum};{x.Version}"));

        public static async Task<TAR> AddOrUpdateAsync(string tarId, TAR tar)
        {
            if (String.IsNullOrEmpty(tar.Receipt))
            {
                throw new ArgumentException("Receipt cannot be empty", tar.Receipt);
            }

            using var hash = SHA256.Create();
            var checksumByteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(tar.Data.RootElement.ToString()));
            var checksum = "0x" + BitConverter.ToString(checksumByteArray).Replace("-", string.Empty);

            var contract = new TARContract();
            contract.SignatureKey = PrivateKey;

            File.WriteAllText($"{StoragePath}{tar.Receipt}.json", tar.Data.RootElement.ToString());
            var tars = TARs.ToList();
            var newTar = new TAR()
            {
                Data = tar.Data,
                Receipt = tar.Receipt,
                Checksum = checksum,
            };

            if (String.IsNullOrEmpty(tarId))
            {
                //var contractAddress = await DeployContractAndGetAddress(checksum);
                var contractResponse = await contract.DeployAsync(UriPrefix, checksumByteArray, RPCEndPoint, long.Parse(ChainId));
                if (contractResponse.Exception != null)
                {
                    throw new Exception("Contract deployment failed, please check that there are enough funds in the wallet corresponding to the provided private key in the configuration.", contractResponse.Exception);
                }

                newTar.Id = $"urn:tar:eip155.{ChainId}:{contractResponse.ContractAddress.Substring(2)}";
                newTar.Version = 1;
            }
            else
            {
                var splitAddress = tarId.Split(':');
                if (splitAddress.Length > 2 && splitAddress[2].StartsWith("eip"))
                {
                    contract.ContractAddress = "0x" + splitAddress.LastOrDefault();
                    var transactionHash = await contract.SetNewChecksum(checksumByteArray, RPCEndPoint, long.Parse(ChainId));
                }

                newTar.Id = tarId;
                newTar.Version = tars.Count(x => x.Id == tarId) + 1;
            }

            tars.Add(newTar);
            SaveTARs(tars);

            return newTar;
        }

        public static async Task<IList<TAR>> FindAsync(IDictionary<string, string> filter) => 
            TARs.Where(x => filter.Values.Any(v => x.Data.RootElement.ToString().Contains(v))).ToArray();

        public static async Task<TAR> GetByReceiptIdAsync(string receiptID) => TARs.FirstOrDefault(x => x.Receipt == receiptID);

        public static async Task<TAR> GetAsync(string tarID) => TARs.Where(x => x.Id == tarID).MaxBy(x => x.Version);

        public static async Task<TAR> GetByVersionAsync(string tarID, int tarVersion) => TARs.FirstOrDefault(x => x.Id == tarID && x.Version == tarVersion);

        public static async Task ArchiveAsync(string tarID) { }
    }
}
