using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Interfaces
{
    public interface IBlockchainService
    {
        Task<decimal> GetBalanceAsync(string address, NetworkType network = NetworkType.Ethereum);
        Task<string> SendTransactionAsync(string fromPrivateKey, string toAddress, decimal amount, NetworkType network);
        Task<object?> GetTransactionReceiptAsync(string txHash, NetworkType network);
        Task<object?> GetBlockAsync(long blockNumber, NetworkType network);
        Task<long> GetLatestBlockNumberAsync(NetworkType network);
        Task<decimal> GetGasPriceAsync(NetworkType network);
        Task<bool> IsValidAddressAsync(string address);
        Task<(string address, string privateKey, string publicKey)> CreateWalletAsync();
        Task<string> CallContractFunctionAsync(string contractAddress, string abi, string functionName, object[]? parameters, NetworkType network);
        Task<string> SendContractTransactionAsync(string fromPrivateKey, string contractAddress, string abi, string functionName, object[]? parameters, NetworkType network);
    }

}
