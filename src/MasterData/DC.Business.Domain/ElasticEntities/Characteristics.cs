using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Domain.ElasticEntities
{
    public class Characteristics
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public int? CountNumber { get; set; }
        public string IconName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool? Deleted { get; set; }
    }
}
