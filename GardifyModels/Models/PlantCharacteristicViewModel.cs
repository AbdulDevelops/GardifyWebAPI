using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PlantCharacteristicViewModel
    {
        public int CategoryId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}