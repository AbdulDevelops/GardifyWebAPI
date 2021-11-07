using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public interface IReferencedObject
    {
        [NotMapped]
        ModelEnums.ReferenceToModelClass ReferenceType { get; }
        [NotMapped]
        int Id { get; set; }
        [NotMapped]
        string Name { get; set; }
      
    }
}