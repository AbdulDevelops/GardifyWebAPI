using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PlantSuggestMerge : _BaseEntity
    {
        public int SuggestedPlantId { get; set; }
        public int ExistingPlantId { get; set; }

    }
}