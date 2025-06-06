using Nethereum.RPC.Eth.DTOs;

namespace Compellio.DCAP.SmartContracts
{
    public class ContractResponse : TransactionReceipt
    {
        public Exception Exception { get; set; }

        public ContractResponse(Exception ex)
        {
            Exception = ex;
        }

        public ContractResponse(TransactionReceipt receipt) 
        {
            this.BlockHash = receipt.BlockHash;
            this.BlockNumber = receipt.BlockNumber;
            this.ContractAddress = receipt.ContractAddress;
            this.CumulativeGasUsed = receipt.CumulativeGasUsed;
            this.EffectiveGasPrice = receipt.EffectiveGasPrice;
            this.From = receipt.From;
            this.GasUsed = receipt.GasUsed;
            this.Logs = receipt.Logs;
            this.LogsBloom = receipt.LogsBloom;
            this.Root = receipt.Root;
            this.Status = receipt.Status;
            this.To = receipt.To;
            this.TransactionHash = receipt.TransactionHash;
            this.TransactionIndex = receipt.TransactionIndex;
        }
    }
}
