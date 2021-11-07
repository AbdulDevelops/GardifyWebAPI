namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Subscriber
    {
        public int ID { get; set; }

        [Required]
        public string email { get; set; }

        public bool isConfirmed { get; set; }

        public DateTime subscribeDate { get; set; }
    }
}
