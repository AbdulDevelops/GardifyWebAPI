using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserEcoElement: _BaseEntity
    {
        public Guid UserId { get; set; }
        public bool Checked { get; set; }

        [ForeignKey("EcoElement")]
        public int EcoElementId { get; set; }

        public int EcoCount { get; set; }

        public virtual EcoElement EcoElement { get; set; }
    }
   
    public class EcoElementViewModel
    {
        public EcoElementViewModel()
        {
            EcoElementsImages = new List<_HtmlImageViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Checked { get; set; }

        public int EcoCount { get; set; }
        public List<_HtmlImageViewModel> EcoElementsImages { get; set; }
    }

    public class EcoElementUpdateViewModel
    {
        public int Id { get; set; }
        public bool Checked { get; set; }
    }

    public class EcoElementCountUpdateViewModel
    {
        public int Id { get; set; }
        public bool Checked { get; set; }

        public int EcoCount { get; set; }
    }
}