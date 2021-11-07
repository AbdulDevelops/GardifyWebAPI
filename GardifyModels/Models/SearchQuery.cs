namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SearchQuery: _BaseEntity
    {
        public Guid UserId { get; set; }

        public string SearchName { get; set; }

        public string CookieString { get; set; }
        
    }
}
