namespace GardifyModels.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PlantTagSuperCategory : _BaseEntity
    {
        public PlantTagSuperCategory()
        {
            this.Groups = new HashSet<Group>();
            this.TagsInThisCategory = new HashSet<PlantTag>();
            this.Childs = new HashSet<PlantTagCategory>();
            this.Todos = new HashSet<TodoTemplate>();
        }

        [Required]
        public string NameGerman { get; set; }
        public string NameLatin { get; set; }
        public string Synonym { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<PlantTag> TagsInThisCategory { get; set; }
        public virtual ICollection<PlantTagCategory> Childs { get; set; }
        public virtual ICollection<TodoTemplate> Todos { get; set; }

    }

    public class PlantTagSuperCategoryViewModel
    {
        public PlantTagSuperCategoryViewModel()
        {
            Images = new List<_HtmlImageViewModel>();
        }
        public string NameGerman { get; set; }
        public string NameLatin { get; set; }
        public string Synonym { get; set; }
        public string Description { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<PlantTag> TagsInThisCategory { get; set; }
        public ICollection<PlantTagCategory> Childs { get; set; }
        public ICollection<TodoTemplate> Todos { get; set; }
        public List<_HtmlImageViewModel> Images { get; set; }
    }
}
