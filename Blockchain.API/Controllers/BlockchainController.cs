using Blockchain.Domain.Enums;
using Blockchain.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>Public blockchain data queries</summary>
    [Tags("Blockchain")]
    [Authorize]
    public class BlockchainController : BaseApiController
    {
        private readonly IBlockchainService _blockchainService;

        public BlockchainController(IBlockchainService blockchainService) => _blockchainService = blockchainService;

        /// <summary>Get balance for any address</summary>
        [HttpGet("balance/{address}")]
        public async Task<IActionResult> GetBalance(string address, [FromQuery] NetworkType network = NetworkType.Ethereum, CancellationToken ct = default)
        {
            var balance = await _blockchainService.GetBalanceAsync(address, network);
            return Ok(new ApiResponse<object> { Success = true, Data = new { address, balance, network } });
        }

        /// <summary>Get latest block number</summary>
        [HttpGet("latest-block")]
        public async Task<IActionResult> GetLatestBlock([FromQuery] NetworkType network = NetworkType.Ethereum, CancellationToken ct = default)
        {
            var blockNumber = await _blockchainService.GetLatestBlockNumberAsync(network);
            return Ok(new ApiResponse<object> { Success = true, Data = new { blockNumber, network } });
        }

        /// <summary>Get gas price (Gwei)</summary>
        [HttpGet("gas-price")]
        public async Task<IActionResult> GetGasPrice([FromQuery] NetworkType network = NetworkType.Ethereum, CancellationToken ct = default)
        {
            var gasPrice = await _blockchainService.GetGasPriceAsync(network);
            return Ok(new ApiResponse<object> { Success = true, Data = new { gasPriceGwei = gasPrice, network } });
        }

        /// <summary>Validate an Ethereum address</summary>
        [HttpGet("validate-address/{address}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateAddress(string address, CancellationToken ct = default)
        {
            var isValid = await _blockchainService.IsValidAddressAsync(address);
            return Ok(new ApiResponse<object> { Success = true, Data = new { address, isValid } });
        }

        /// <summary>Get transaction receipt from blockchain</summary>
        [HttpGet("tx-receipt/{txHash}")]
        public async Task<IActionResult> GetTxReceipt(string txHash, [FromQuery] NetworkType network = NetworkType.Ethereum, CancellationToken ct = default)
        {
            var receipt = await _blockchainService.GetTransactionReceiptAsync(txHash, network);
            return Ok(new ApiResponse<object> { Success = true, Data = receipt });
        }
    }
}
