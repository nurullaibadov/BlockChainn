using Blockchain.Domain.Enums;
using Blockchain.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Blockchain.Infrastructure.Services
{
    public class BlockchainService : IBlockchainService
    {
        private readonly IConfiguration _config;

        public BlockchainService(IConfiguration config) => _config = config;

        private Web3 GetWeb3(NetworkType network)
        {
            var url = network == NetworkType.Ethereum
                ? _config["BlockchainSettings:NodeUrl"]
                : _config["BlockchainSettings:TestNodeUrl"];
            return new Web3(url!);
        }

        private Web3 GetWeb3WithAccount(string privateKey, NetworkType network)
        {
            var url = network == NetworkType.Ethereum
                ? _config["BlockchainSettings:NodeUrl"]
                : _config["BlockchainSettings:TestNodeUrl"];
            var account = new Account(privateKey);
            return new Web3(account, url!);
        }

        public async Task<decimal> GetBalanceAsync(string address, NetworkType network = NetworkType.Ethereum)
        {
            var web3 = GetWeb3(network);
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance.Value);
        }

        public async Task<string> SendTransactionAsync(string fromPrivateKey, string toAddress, decimal amount, NetworkType network)
        {
            var web3 = GetWeb3WithAccount(fromPrivateKey, network);
            var amountWei = Web3.Convert.ToWei(amount);
            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();

            var txInput = new TransactionInput
            {
                To = toAddress,
                Value = new HexBigInteger(amountWei),
                Gas = new HexBigInteger(21000),
                GasPrice = gasPrice
            };

            var txHash = await web3.Eth.TransactionManager.SendTransactionAsync(txInput);
            return txHash;
        }

        public async Task<object?> GetTransactionReceiptAsync(string txHash, NetworkType network)
        {
            var web3 = GetWeb3(network);
            return await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
        }

        public async Task<object?> GetBlockAsync(long blockNumber, NetworkType network)
        {
            var web3 = GetWeb3(network);
            return await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber
                .SendRequestAsync(new HexBigInteger(blockNumber));
        }

        public async Task<long> GetLatestBlockNumberAsync(NetworkType network)
        {
            var web3 = GetWeb3(network);
            var block = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            return (long)block.Value;
        }

        public async Task<decimal> GetGasPriceAsync(NetworkType network)
        {
            var web3 = GetWeb3(network);
            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            return Web3.Convert.FromWei(gasPrice.Value, Nethereum.Util.UnitConversion.EthUnit.Gwei);
        }

        public Task<bool> IsValidAddressAsync(string address)
        {
            var isValid = !string.IsNullOrEmpty(address) &&
                          address.StartsWith("0x") &&
                          address.Length == 42;
            return Task.FromResult(isValid);
        }

        public Task<(string address, string privateKey, string publicKey)> CreateWalletAsync()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex(true);
            var address = ecKey.GetPublicAddress();
            var publicKey = ecKey.GetPubKeyNoPrefix().ToHex(true);
            return Task.FromResult((address, privateKey, publicKey));
        }

        public async Task<string> CallContractFunctionAsync(string contractAddress, string abi, string functionName, object[]? parameters, NetworkType network)
        {
            var web3 = GetWeb3(network);
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(functionName);
            var result = await function.CallAsync<string>(parameters ?? Array.Empty<object>());
            return result ?? string.Empty;
        }

        public async Task<string> SendContractTransactionAsync(string fromPrivateKey, string contractAddress, string abi, string functionName, object[]? parameters, NetworkType network)
        {
            var web3 = GetWeb3WithAccount(fromPrivateKey, network);
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(functionName);
            var txHash = await function.SendTransactionAsync(
                web3.Eth.TransactionManager.Account!.Address,
                parameters ?? Array.Empty<object>());
            return txHash;
        }
    }
}
