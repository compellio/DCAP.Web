using Compellio.DCAP.SmartContracts.TestBytes32.ContractDefinition;
using Nethereum.RPC.Eth.Exceptions;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;

namespace Compellio.DCAP.SmartContracts.TestBytes32
{
    public class TARContract
    {
        const string etherscanUrl = "https://api-sepolia.etherscan.io/api";
        const string tarContractSourceCode = @"// SPDX-License-Identifier: GPL-3.0-only
// SPDX-FileCopyrightText: Copyright (C) 2024 Compellio S.A.

pragma solidity >=0.4.0 <0.8.0;

contract TestBytes32 {
    bytes16 private constant HEX_DIGITS = ""0123456789abcdef"";
    uint256 private _nextVersionId;
    bytes32[5] private _checksums;

    function push(bytes32 checksum_) public {
        uint256 tokenId = _nextVersionId++;
        _checksums[tokenId] = checksum_;
    }

    function total() public view returns (uint256) {
        return _nextVersionId;
    }

    function checksum(uint256 version) public view returns (string memory) {
        return toHexString(uint256(_checksums[version]), 32);
    }

    function toHexString(uint256 value, uint256 length) internal pure returns (string memory) {
        uint256 localValue = value;
        bytes memory buffer = new bytes(2 * length + 2);
        buffer[0] = ""0"";
        buffer[1] = ""x"";
        for (uint256 i = 2 * length + 1; i > 1; --i) {
            buffer[i] = HEX_DIGITS[localValue & 0xf];
            localValue >>= 4;
        }
        return string(buffer);
    }

}";
        private Web3 web3;
        private TestBytes32Service service;
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

        public async Task<ContractResponse> DeployAsync(byte[] checksum, string chainUrl, long chainId) => 
            await DeployAsync(800000, Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei), checksum, chainUrl, chainId);

        public async Task<ContractResponse> DeployAsync(BigInteger gas, BigInteger gasPrice, byte[] checksum, string chainUrl, long chainId)
        {
            this.chainUrl = chainUrl;
            this.chainId = chainId;
            SignatureKey = signatureKey;

            var deployment = new TestBytes32Deployment()
            {
                Checksum = checksum,
                GasPrice = gasPrice,
                Gas = gas,
            };

            //var count = web3.Eth.Transactions.GetTransactionCount;
            var h = web3.Eth.GetContractDeploymentHandler<TestBytes32Deployment>();
            //var x = h.CreateTransactionInputEstimatingGasAsync(deployment);
            var gasEstimate = await h.EstimateGasAsync(deployment);
            deployment.Gas = gasEstimate;
            deployment.MaxFeePerGas = gasEstimate + new BigInteger(1000000);

            ContractResponse contractResponse = null;
            try
            {
                var receipt = await TestBytes32Service.DeployContractAndWaitForReceiptAsync(web3, deployment);
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
            service = new TestBytes32Service(web3, contractAddress);
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
            service = new TestBytes32Service(web3, contractAddress);
            var value = await service.PushRequestAsync(checksum);
            return value;
        }
    }
}
