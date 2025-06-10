using Newtonsoft.Json;

namespace Compellio.DCAP.Etherscan
{
    public class EScanContractCodeVerificationModel
    {
        /// <summary>
        /// Contract Address starts with 0x... 
        /// </summary>
        [JsonProperty("contractaddress")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// Contract Source Code (Flattened if necessary)
        /// </summary>
        [JsonProperty("sourceCode")]
        public string SourceCode { get; set; }

        /// <summary>
        /// Solidity-single-file (default) or solidity-standard-json-input (for std-input-json-format support
        /// </summary>
        [JsonProperty("codeformat")]
        public string CodeFormat { get; set; }

        /// <summary>
        /// ContractName (if codeformat=solidity-standard-json-input, then enter contractname as ex: erc20.sol:erc20)
        /// </summary>
        [JsonProperty("contractname")]
        public string ContractName { get; set; }

        /// <summary>
        /// See https://etherscan.io/solcversions for list of support versions
        /// </summary>
        [JsonProperty("compilerversion")]
        public string CompilerVersion { get; set; }

        /// <summary>
        /// 0 = No Optimization, 1 = Optimization used (applicable when codeformat=solidity-single-file)
        /// </summary>
        [JsonProperty("optimizationUsed")]
        public string OptimizationUsed { get; set; }

        /// <summary>
        /// Set to 200 as default unless otherwise  (applicable when codeformat=solidity-single-file)
        /// </summary>
        [JsonProperty("runs")]
        public string Runs { get; set; }

        /// <summary>
        /// If applicable
        /// </summary>
        [JsonProperty("contstructorArguments")]
        public string ContstructorArguments { get; set; }

        /// <summary>
        /// Leave blank for compiler default, homestead, tangerineWhistle, spuriousDragon, byzantium,
        /// constantinople, petersburg, istanbul (applicable when codeformat=solidity-single-file)
        /// </summary>
        [JsonProperty("evmversion")]
        public string EvmVersion { get; set; }

        /// <summary>
        /// Valid codes 1-14 where 1=No License .. 14=Business Source License 1.1, see https://etherscan.io/contract-license-types
        /// </summary>
        [JsonProperty("licenseType")]
        public string LicenseType { get; set; }

        #region Libraries
        [JsonProperty("library1Name")]
        public string LibraryName1 { get; set; }
        [JsonProperty("library1Address")]
        public string LibraryAddress1 { get; set; }

        [JsonProperty("library2Name")]
        public string LibraryName2 { get; set; }
        [JsonProperty("library2Address")]
        public string LibraryAddress2 { get; set; }

        [JsonProperty("library3Name")]
        public string LibraryName3 { get; set; }
        [JsonProperty("library3Address")]
        public string LibraryAddress3 { get; set; }

        [JsonProperty("library4Name")]
        public string LibraryName4 { get; set; }
        [JsonProperty("library4Address")]
        public string LibraryAddress4 { get; set; }

        [JsonProperty("library5Name")]
        public string LibraryName5 { get; set; }
        [JsonProperty("library5Address")]
        public string LibraryAddress5 { get; set; }

        [JsonProperty("library6Name")]
        public string LibraryName6 { get; set; }
        [JsonProperty("library6Address")]
        public string LibraryAddress6 { get; set; }

        [JsonProperty("library7Name")]
        public string LibraryName7 { get; set; }
        [JsonProperty("library7Address")]
        public string LibraryAddress7 { get; set; }

        [JsonProperty("library8Name")]
        public string LibraryName8 { get; set; }
        [JsonProperty("library8Address")]
        public string LibraryAddress8 { get; set; }

        [JsonProperty("library9Name")]
        public string LibraryName9 { get; set; }
        [JsonProperty("library9Address")]
        public string LibraryAddress9 { get; set; }

        [JsonProperty("library10Name")]
        public string LibraryName10 { get; set; }
        [JsonProperty("library10Address")]
        public string LibraryAddress10 { get; set; }
        #endregion
    }
}
