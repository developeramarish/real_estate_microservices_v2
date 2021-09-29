using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Domain.Entities.Organization
{
    public class Image
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string ImageName { get; set; }
        public string  ImageUrl { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? Deleted { get; set; }
    }
}
