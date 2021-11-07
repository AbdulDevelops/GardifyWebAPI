namespace GardifyModels.Models
{
    using GardifyModels.Models;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Alert : _BaseEntity
    {
        [Required]
        public int RelatedObjectId { get; set; }
        [Required]
        public ModelEnums.ReferenceToModelClass ObjectType { get; set; }
        [StringLength(256)]
        public string Title { get; set; }
        [StringLength(4096)]
        public string Text { get; set; }
        public virtual ICollection<AlertTrigger> Triggers { get; set; }
        [NotMapped]
        private string _ReadableCondition { get; set; }
        [NotMapped]
        public string ReadableCondition
        {
            get
            {
                _ReadableCondition = "";
                if (Triggers != null && Triggers.Any())
                {
                    if (Triggers.Count() > 1)
                    {
                        _ReadableCondition += "(";
                    }
                    int count = 0;
                    foreach (AlertTrigger trigger in Triggers)
                    {
                        if (count > 0)
                        {
                            _ReadableCondition += ") ODER (";
                        }
                        _ReadableCondition += trigger.ReadableCondition;
                        count++;
                    }
                    if (Triggers.Count() > 1)
                    {
                        _ReadableCondition += ")";
                    }
                }
                else
                {
                    _ReadableCondition = "Keine Bedingungen vorhanden";
                }

                return _ReadableCondition;
            }
            private set { ReadableCondition = value; }
        }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Alert()
        //{
        //    AlertTriggers = new HashSet<AlertTrigger>();
        //}

        //public int RelatedObjectId { get; set; }

        //public int ObjectType { get; set; }

        //[StringLength(256)]
        //public string Title { get; set; }

        //public string Text { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<AlertTrigger> AlertTriggers { get; set; }
    }
}
