using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class LexiconTerm: _BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class LexiconTermVM
    {
        public int Id { get; set; }
        public LexiconTermVM()
        {
            Images = new List<_HtmlImageViewModel>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<_HtmlImageViewModel> Images { get; set; }
    }
}