namespace GardifyModels.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AlertTrigger: _BaseEntity
    {
        [Required]
        [ForeignKey("Alert")]
        public int AlertId { get; set; }
        public virtual Alert Alert { get; set; }
        public virtual ICollection<AlertCondition> Conditions { get; set; }

        [NotMapped]
        private string _ReadableCondition { get; set; }
        [NotMapped]
        public string ReadableCondition
        {
            get
            {
                _ReadableCondition = "";
                if (Conditions != null && Conditions.Any(c => !c.Deleted))
                {
                    int count = 0;
                    foreach (AlertCondition condition in Conditions.Where(c => !c.Deleted))
                    {
                        if (count > 0)
                        {
                            _ReadableCondition += " UND ";
                        }
                        _ReadableCondition += condition.ReadableCondition;

                        count++;
                    }
                }
                else
                {
                    _ReadableCondition = "leere Bedingungsgruppe";
                }
                return _ReadableCondition;
            }
            private set { _ReadableCondition = value; }
        }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public AlertTrigger()
        //{
        //    AlertConditions = new HashSet<AlertCondition>();
        //}


        //public int AlertId { get; set; }



        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<AlertCondition> AlertConditions { get; set; }

        //public virtual Alert Alert { get; set; }
    }
}
