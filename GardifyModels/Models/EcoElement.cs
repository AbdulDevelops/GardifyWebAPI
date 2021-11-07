using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    // Created by admins only
    public class EcoElement : _BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
    public class AdminEcoElementViewModel
    {
        public AdminEcoElementViewModel()
        {
            EcoElementsImages = new List<_HtmlImageViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<_HtmlImageViewModel> EcoElementsImages { get; set; }
    }
}