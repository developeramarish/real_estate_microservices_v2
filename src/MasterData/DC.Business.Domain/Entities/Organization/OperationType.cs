using DC.Business.Domain.Enums;
using System;
namespace DC.Business.Domain.Entities.Organization
{
    public class OperationType
    {
        public long Id { get; set; }
        public OperationTypeEnum Type { get; set; }
        public string TypeName { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime Deleted { get; set; }
    }
}
