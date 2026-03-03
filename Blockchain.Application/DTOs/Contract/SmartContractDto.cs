using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.DTOs.Contract
{
    public class SmartContractDto
    {
        public Guid Id { get; set; }
        public string ContractAddress { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Abi { get; set; } = string.Empty;
        public NetworkType Network { get; set; }
        public ContractStatus Status { get; set; }
        public string? DeployerAddress { get; set; }
        public string? DeployTxHash { get; set; }
        public DateTime? DeployedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddContractDto
    {
        public string ContractAddress { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Abi { get; set; } = string.Empty;
        public NetworkType Network { get; set; }
    }

    public class CallContractDto
    {
        public Guid ContractId { get; set; }
        public string FunctionName { get; set; } = string.Empty;
        public object[]? Parameters { get; set; }
    }

    public class SendContractTxDto
    {
        public Guid ContractId { get; set; }
        public Guid FromWalletId { get; set; }
        public string FunctionName { get; set; } = string.Empty;
        public object[]? Parameters { get; set; }
    }
}
