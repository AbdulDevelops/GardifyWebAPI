using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class LastViewedArticle:_BaseEntity
    {
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public int PreviousId { get; set; }
        public virtual Article Article { get; set; }

    }
}