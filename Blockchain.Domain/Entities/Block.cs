using Blockchain.Domain.Common;
using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Entities
{
    public class Block : BaseEntity
    {
        public long BlockNumber { get; set; }
        public string BlockHash { get; set; } = string.Empty;
        public string ParentHash { get; set; } = string.Empty;
        public string Miner { get; set; } = string.Empty;
        public decimal Difficulty { get; set; }
        public long GasLimit { get; set; }
        public long GasUsed { get; set; }
        public int TransactionCount { get; set; }
        public DateTime Timestamp { get; set; }
        public NetworkType Network { get; set; }
        public string? ExtraData { get; set; }
    }
}
