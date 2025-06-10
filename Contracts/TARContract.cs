using Compellio.DCAP.Etherscan;
using Compellio.DCAP.SmartContracts.DCAPv2.ContractDefinition;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.Exceptions;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Net;
using System.Numerics;

namespace Compellio.DCAP.SmartContracts.DCAPv2
{
    public class TARContract
    {
        const string etherlinkUrl = "https://testnet.explorer.etherlink.com/api";
        const string contractJson = @"{
    ""language"": ""Solidity"",
    ""sources"": {
        ""@openzeppelin/contracts/utils/introspection/IERC165.sol"": {
            ""content"": ""// SPDX-License-Identifier: MIT\n// OpenZeppelin Contracts (last updated v5.1.0) (utils/introspection/IERC165.sol)\n\npragma solidity ^0.8.20;\n\n/**\n * @dev Interface of the ERC-165 standard, as defined in the\n * https://eips.ethereum.org/EIPS/eip-165[ERC].\n *\n * Implementers can declare support of contract interfaces, which can then be\n * queried by others ({ERC165Checker}).\n *\n * For an implementation, see {ERC165}.\n */\ninterface IERC165 {\n    /**\n     * @dev Returns true if this contract implements the interface defined by\n     * `interfaceId`. See the corresponding\n     * https://eips.ethereum.org/EIPS/eip-165#how-interfaces-are-identified[ERC section]\n     * to learn more about how these ids are created.\n     *\n     * This function call must use less than 30 000 gas.\n     */\n    function supportsInterface(bytes4 interfaceId) external view returns (bool);\n}\n""
        },
        ""contracts/DCAPv2.sol"": {
            ""content"": ""// SPDX-License-Identifier: GPL-3.0-or-later\n// SPDX-FileCopyrightText: Copyright (C) 2025 Compellio S.A.\n\npragma solidity ^0.8.24;\n\nimport {ITAR} from \""./interfaces/ITAR.sol\"";\nimport {IERC165} from \""@openzeppelin/contracts/utils/introspection/IERC165.sol\"";\n\n/// @title DCAPv2 - Token Asset Record contract for DCAP\ncontract DCAPv2 is ITAR {\n    bytes16 private constant HEX_DIGITS = \""0123456789abcdef\"";\n\n    address private _owner;\n    string private _uriPrefix;\n\n    uint256 private _nextVersionId;\n    mapping(uint256 => bytes32) private _checksums;\n    mapping(bytes32 => uint256) private _checksumToVersion;\n\n    constructor(string memory uriPrefix_, bytes32 checksum_) {\n        _owner = msg.sender;\n        _uriPrefix = uriPrefix_;\n        push(checksum_);\n    }\n\n    modifier onlyOwner() {\n        require(msg.sender == _owner, \""DCAPv2: unauthorized\"");\n        _;\n    }\n\n    function push(bytes32 checksum_) public virtual override onlyOwner {\n        uint256 version = _nextVersionId++;\n        _checksums[version] = checksum_;\n        _checksumToVersion[checksum_] = version;\n        emit Updated(version, toHexString(uint256(checksum_), 32));\n    }\n\n    function getChecksum(uint256 version_) public view override returns (bytes32) {\n        require(hasVersion(version_), \""DCAPv2: version does not exist\"");\n        return _checksums[version_];\n    }\n\n    function checksum(uint256 version) public view returns (string memory) {\n        require(hasVersion(version), \""DCAPv2: version does not exist\"");\n        return toHexString(uint256(_checksums[version]), 32);\n    }\n\n    function hasVersion(uint256 version_) public view override returns (bool) {\n        return version_ < _nextVersionId;\n    }\n\n    function hasChecksum(bytes32 checksum_) public view override returns (bool) {\n        return _checksumToVersion[checksum_] != 0 || (_nextVersionId > 0 && _checksums[0] == checksum_);\n    }\n\n    function getVersion(bytes32 checksum_) public view override returns (uint256) {\n        require(hasChecksum(checksum_), \""DCAPv2: checksum not found\"");\n        return _checksumToVersion[checksum_];\n    }\n\n    function totalVersions() public view override returns (uint256) {\n        return _nextVersionId;\n    }\n\n    function dataUri(uint256) public view override returns (string memory) {\n        return string(abi.encodePacked(_uriPrefix, toHexString(uint256(uint160(address(this))), 20)));\n    }\n\n    function owner() public view returns (address) {\n        return _owner;\n    }\n\n    function supportsInterface(bytes4 interfaceId) public view virtual override returns (bool) {\n        return\n            interfaceId == type(IERC165).interfaceId ||\n            interfaceId == type(ITAR).interfaceId;\n    }\n\n    function toHexString(uint256 value, uint256 length) internal pure returns (string memory) {\n        bytes memory buffer = new bytes(2 * length + 2);\n        buffer[0] = \""0\"";\n        buffer[1] = \""x\"";\n        for (uint256 i = 2 * length + 1; i > 1; --i) {\n            buffer[i] = HEX_DIGITS[value & 0xf];\n            value >>= 4;\n        }\n        return string(buffer);\n    }\n}\n""
        },
        ""contracts/interfaces/ITAR.sol"": {
            ""content"": ""// SPDX-License-Identifier: GPL-3.0-or-later\n// SPDX-FileCopyrightText: Copyright (C) 2025 Compellio S.A.\n\npragma solidity ^0.8.24;\n\nimport {IERC165} from \""@openzeppelin/contracts/utils/introspection/IERC165.sol\"";\n\n/**\n * @title Token Asset Record Standard\n */\ninterface ITAR is IERC165 {\n    /**\n     * This event emits when a new version is created.\n     *\n     * @param version The new version number\n     * @param checksum The new version checksum\n     */\n    event Updated(uint256 indexed version, string checksum);\n\n    /**\n     * @notice Creates a new TAR version\n     *\n     * @param checksum The checksum of the new version (bytes32)\n     */\n    function push(bytes32 checksum) external;\n\n    /**\n     * @notice Returns the total amount of versions stored by the contract\n     *\n     * @return A count of all versions tracked by this contract\n     */\n    function totalVersions() external view returns (uint256);\n\n    /**\n     * @notice Returns whether `version` exists.\n     *\n     * @param version A potential version number\n     * @return True if _version exists, False otherwise\n     */\n    function hasVersion(uint256 version) external view returns (bool);\n\n    /**\n     * @notice Returns the checksum of a specific version.\n     *\n     * @param version A version number less than `total()`\n     * @return The checksum of version\n     */\n    function getChecksum(uint256 version) external view returns (bytes32);\n\n    /**\n     * @notice Returns whether a checksum is already stored\n     *\n     * @param checksum The checksum value\n     * @return True if already stored, False otherwise\n     */\n    function hasChecksum(bytes32 checksum) external view returns (bool);\n\n    /**\n     * @notice Returns the version associated with a given checksum\n     *\n     * @param checksum The checksum value\n     * @return The corresponding version number\n     */\n    function getVersion(bytes32 checksum) external view returns (uint256);\n\n    /**\n     * @notice Returns the URI for `version`.\n     *\n     * @param version A version number less than `total()`\n     * @return An RFC 3986 URI pointing to a JSON file containing the TAR's payload\n     */\n    function dataUri(uint256 version) external view returns (string memory);\n}\n""
        }
    },
    ""settings"": {
        ""evmVersion"": ""paris"",
        ""optimizer"": {
            ""enabled"": false,
            ""runs"": 200
        },
        ""outputSelection"": {
            ""*"": {
                ""*"": [
                    ""abi"",
                    ""evm.bytecode"",
                    ""evm.deployedBytecode"",
                    ""evm.methodIdentifiers"",
                    ""metadata""
                ],
                """": [
                    ""ast""
                ]
            }
        }
    }
}";
        private Web3 web3;
        private DCAPv2Service service;
        private string contractAddress, signatureKey;
        private long chainId;
        private string chainUrl;
        private readonly bool useLegacy;

        public string SignatureKey 
        {
            get => signatureKey;
            set 
            {
                signatureKey = value;
                if (chainId != 0 && String.IsNullOrWhiteSpace(chainUrl) == false)
                {
                    var account = new Account(signatureKey, chainId);
                    web3 = new Web3(account, chainUrl);
                    web3.TransactionManager.UseLegacyAsDefault = useLegacy;
                }
            }
        }

        public string ContractAddress
        {
            get => contractAddress;
            set
            {
                contractAddress = value;
                if (chainId != 0 && String.IsNullOrWhiteSpace(chainUrl) == false)
                {
                    var account = new Account(signatureKey, chainId);
                    web3 = new Web3(account, chainUrl);
                    web3.TransactionManager.UseLegacyAsDefault = useLegacy;
                }
            }
        }

        public TARContract()
        {
            this.useLegacy = true;
        }

        public TARContract(string privateKey, bool useLegacy = true)
        {
            this.useLegacy = useLegacy;
            SignatureKey = privateKey;
        }

        public async Task<ContractResponse> DeployAsync(string uriPrefix, byte[] checksum, string chainUrl, long chainId)
        {
            this.chainUrl = chainUrl;
            this.chainId = chainId;
            SignatureKey = signatureKey;

            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<DCAPv2Deployment>();
            ClientBase.ConnectionTimeout = new TimeSpan(0, 5, 0);
            var deployment = new DCAPv2Deployment() { UriPrefix = uriPrefix, Checksum = checksum };
            var transactionHash = await deploymentHandler.SendRequestAsync(deployment);
            Thread.Sleep(2000);

            var transaction = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            Thread.Sleep(2000);

            ContractResponse contractResponse = null;
            try
            {
                var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                ContractAddress = receipt.ContractAddress;
                contractResponse = new ContractResponse(receipt);
            }
            catch (Exception ex)
            {
                if (ex is ContractDeploymentException)
                {
                    var e = ex as ContractDeploymentException;
                    var receipt = e != null ? new ContractResponse(e.TransactionReceipt) : new ContractResponse(ex);
                    receipt.Exception = ex;
                    return receipt;
                }
                else
                {
                    return new ContractResponse(ex);
                }
            }

            if (contractResponse != null && chainId == 128123)
            {
                var verificationPayload = new EScanContractCodeVerificationModel()
                {
                    CodeFormat = "solidity-standard-json-input",
                    CompilerVersion = "v0.8.28+commit.7893614a",
                    ContractAddress = contractResponse.ContractAddress,
                    ContractName = "DCAPv2",
                    LicenseType = "5", // GPL v3.0
                    OptimizationUsed = "1",
                    Runs = "200",
                    SourceCode = contractJson,
                };
                var verifier = new EtherscanContractVerifier(etherlinkUrl, "api_key");
                var response = await verifier.SendAsync(verificationPayload);
            }

            return contractResponse;
        }

        public async Task<string> GetChecksum(int version, string chainUrl, long chainId)
        {
            if (string.IsNullOrWhiteSpace(contractAddress))
            {
                throw new ArgumentNullException(nameof(contractAddress));
            }

            if (version < 0)
            {
                throw new ArgumentException(nameof(version));
            }

            this.chainUrl = chainUrl;
            this.chainId = chainId;
            SignatureKey = signatureKey;
            service = new DCAPv2Service(web3, contractAddress);
            var value = await service.ChecksumQueryAsync(version);
            return value;
        }

        public async Task<string> SetNewChecksum(byte[] checksum, string chainUrl, long chainId)
        {
            if (string.IsNullOrWhiteSpace(contractAddress))
            {
                throw new ArgumentNullException(nameof(contractAddress));
            }

            if (checksum == null || checksum.Length != 32)
            {
                throw new ArgumentNullException(nameof(checksum));
            }

            this.chainUrl = chainUrl;
            this.chainId = chainId;
            SignatureKey = signatureKey;
            service = new DCAPv2Service(web3, contractAddress);
            var value = await service.PushRequestAsync(checksum);
            return value;
        }
    }
}
