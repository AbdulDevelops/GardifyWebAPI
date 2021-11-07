using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Cmspages2
    {
        public Cmspages2()
        {
            CmspageContent2 = new HashSet<CmspageContent2>();
            HeaderFooterItems = new HashSet<HeaderFooterItems>();
        }

        public int SitemapExternId { get; set; }
        public int? EntityId { get; set; }
        public string LinkTitle { get; set; }
        public string KeywordUrl { get; set; }
        public string FriendlyUrl { get; set; }
        public int? ParentId { get; set; }
        public int? BelongsToPageId { get; set; }
        public int? RedirectToPageId { get; set; }
        public int? TypeOfPage { get; set; }
        public int? MenuId { get; set; }
        public int? TemplateId { get; set; }
        public string ModulePageLinkExternal { get; set; }
        public string ModulePageLinkInternal { get; set; }
        public string ModuleXmlLink { get; set; }
        public string Language { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaAuthor { get; set; }
        public string MetaCopyright { get; set; }
        public string MetaPublisher { get; set; }
        public string MetaIndexFollow { get; set; }
        public string Priority { get; set; }
        public bool? Navigation1 { get; set; }
        public bool? Navigation2 { get; set; }
        public bool? Navigation3 { get; set; }
        public bool? Navigation4 { get; set; }
        public bool? Navigation5 { get; set; }
        public string Roles { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public int? Sort { get; set; }
        public bool? IsHome { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public bool? NotEditable { get; set; }

        public virtual ICollection<CmspageContent2> CmspageContent2 { get; set; }
        public virtual ICollection<HeaderFooterItems> HeaderFooterItems { get; set; }
    }
}
