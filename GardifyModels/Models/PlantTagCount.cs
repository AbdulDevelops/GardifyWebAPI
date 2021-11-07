using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PlantTagCount : _BaseEntity
    {
        public int PlantTagId { get; set; }
        [ForeignKey("PlantTagId")]
        public virtual PlantTag PlantTag { get; set; }

        public string PlantTagIdentifier { get; set; }
        public int? Count { get; set; }
    }
}