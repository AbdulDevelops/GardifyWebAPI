namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Group")]
    public partial class Group: _BaseEntity
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Group()
        //{
        //    Plants = new HashSet<Plant>();
        //}
        public Group()
        {
            this.SuperCategories = new HashSet<PlantTagSuperCategory>();
        }

        public string Name { get; set; }


        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Plant> Plants { get; set; }

        public virtual ICollection<Plant> PlantsWithThisGroupd { get; set; }
        public virtual ICollection<PlantTagSuperCategory> SuperCategories { get; set; }
    }

    public class GroupSimplified
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public GroupSimplified(Group group)
        {
            Id = group.Id;
            Name = group.Name;
        }
    }
}
