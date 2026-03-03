using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public Guid? CreatedByUserId { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}
