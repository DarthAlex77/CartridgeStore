using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartridgeStore.Models
{
    public class Unit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(AutoGenerateField = false)]
        public Guid UnitId { get; set; }

        [Display(Name = "Подразделение")]
        public string Name { get; set; }
    }
    public class UnitComparer:IEqualityComparer<Unit>
    {
        public bool Equals(Unit x, Unit y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.UnitId.Equals(y.UnitId) && x.Name == y.Name;
        }

        public int GetHashCode(Unit obj)
        {
            unchecked
            {
                return (obj.UnitId.GetHashCode() * 397) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }
    }
}