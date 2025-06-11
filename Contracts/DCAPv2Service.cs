using Compellio.DCAP.SmartContracts.DCAPv2.ContractDefinition;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Numerics;

namespace Compellio.DCAP.SmartContracts.DCAPv2
{
    public partial class DCAPv2Service : ContractWeb3ServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, DCAPv2Deployment testBytes32Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<DCAPv2Deployment>().SendRequestAndWaitForReceiptAsync(testBytes32Deployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, DCAPv2Deployment testBytes32Deployment)
        {
            return web3.Eth.GetContractDeploymentHandler<DCAPv2Deployment>().SendRequestAsync(testBytes32Deployment);
        }

        public static async Task<DCAPv2Service> DeployContractAndGetServiceAsync(IWeb3 web3, DCAPv2Deployment testBytes32Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, testBytes32Deployment, cancellationTokenSource);
            return new DCAPv2Service(web3, receipt.ContractAddress);
        }

        public DCAPv2Service(IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<byte[]> ChecksumQueryAsync(ChecksumFunction checksumFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ChecksumFunction, byte[]>(checksumFunction, blockParameter);
        }


        public Task<byte[]> ChecksumQueryAsync(BigInteger version, BlockParameter blockParameter = null)
        {
            var checksumFunction = new ChecksumFunction();
            checksumFunction.Version = version;

            return ContractHandler.QueryAsync<ChecksumFunction, byte[]>(checksumFunction, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }


        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> PushRequestAsync(PushFunction pushFunction)
        {
            return ContractHandler.SendRequestAsync(pushFunction);
        }

        public Task<TransactionReceipt> PushRequestAndWaitForReceiptAsync(PushFunction pushFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(pushFunction, cancellationToken);
        }

        public Task<string> PushRequestAsync(byte[] checksum)
        {
            var pushFunction = new PushFunction();
            pushFunction.Checksum = checksum;

            return ContractHandler.SendRequestAsync(pushFunction);
        }

        public Task<TransactionReceipt> PushRequestAndWaitForReceiptAsync(byte[] checksum, CancellationTokenSource cancellationToken = null)
        {
            var pushFunction = new PushFunction();
            pushFunction.Checksum = checksum;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(pushFunction, cancellationToken);
        }

        public Task<BigInteger> TotalQueryAsync(TotalFunction totalFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalFunction, BigInteger>(totalFunction, blockParameter);
        }


        public Task<BigInteger> TotalQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalFunction, BigInteger>(null, blockParameter);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(ChecksumFunction),
                typeof(OwnerFunction),
                typeof(PushFunction),
                typeof(TotalFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {

            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {

            };
        }
    }
}
