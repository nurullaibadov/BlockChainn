using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Contract;
using Blockchain.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>Smart contract management and interaction</summary>
    [Tags("SmartContracts")]
    [Authorize]
    public class SmartContractController : BaseApiController
    {
        private readonly ISmartContractService _contractService;

        public SmartContractController(ISmartContractService contractService) => _contractService = contractService;

        /// <summary>Add/register a smart contract</summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddContractDto dto, CancellationToken ct)
            => HandleResult(await _contractService.AddContractAsync(CurrentUserId, dto, ct));

        /// <summary>Get all my contracts (paginated)</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _contractService.GetContractsAsync(CurrentUserId, query, ct));

        /// <summary>Get contract by ID</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => HandleResult(await _contractService.GetContractByIdAsync(id, ct));

        /// <summary>Call (read) a contract function</summary>
        [HttpPost("call")]
        public async Task<IActionResult> CallFunction([FromBody] CallContractDto dto, CancellationToken ct)
            => HandleResult(await _contractService.CallContractFunctionAsync(CurrentUserId, dto, ct));

        /// <summary>Send a state-changing contract transaction</summary>
        [HttpPost("send-tx")]
        public async Task<IActionResult> SendTransaction([FromBody] SendContractTxDto dto, CancellationToken ct)
            => HandleResult(await _contractService.SendContractTransactionAsync(CurrentUserId, dto, ct));

        /// <summary>Remove a contract</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
            => HandleResult(await _contractService.DeleteContractAsync(id, CurrentUserId, ct));
    }
}
