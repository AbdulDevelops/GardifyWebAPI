using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class ArticleCategory: _BaseEntity
    {
        public ArticleCategory()
        {
            ArticlesInthisCategory = new HashSet<Article>();
        }
        [Required]
        public string Title { get; set; }
        public bool IsGiftIdea { get; set; }
        public virtual ICollection<Article> ArticlesInthisCategory { get; set; }
    }

    public class ArticleCategoryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsGiftIdea { get; set; }
    }
}