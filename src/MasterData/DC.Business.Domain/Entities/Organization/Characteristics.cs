using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Domain.Entities.Organization
{
    public class Characteristics
    {
        public int Id { get; private set; }
        public int PropertyId { get; private set; }
        public string Name { get; private set; }
        public int? CountNumber { get; private set; }
        public string IconName { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime UpdateDate { get; private set; }
        public bool? Deleted { get; private set; }

        public void Create(int propertyId, string name, int? countNumber, string iconName)
        {
            if (propertyId <= 0)
                throw new ArgumentNullException(nameof(propertyId));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (iconName == null)
                throw new ArgumentNullException(nameof(iconName));

            PropertyId = propertyId;
            Name = name;
            CountNumber = countNumber;
            IconName = iconName;
            CreationDate = DateTime.UtcNow; 
        }

        //public Characterisitcs Update(string name, int? countNumber, string iconName, bool? deleted)
        //{
        //    return new Characterisitcs()
        //    {
        //        Name = name,
        //        CountNumber = countNumber,
        //        IconName = iconName,
        //        UpdateDate = DateTime.UtcNow,
        //        Deleted = deleted
        //    };
        //}
    }
}
