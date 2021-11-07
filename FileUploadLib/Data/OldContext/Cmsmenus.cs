using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Cmsmenus
    {
        public int CmsmenuId { get; set; }
        public string CmsmenuName { get; set; }
        public string CmsmenuDescription { get; set; }
        public int? CmsentityId { get; set; }
        public bool? MenuAddItemPossible { get; set; }
        public int? MenuLevelsMax { get; set; }
        public int? MenuNumberOfItemsMax { get; set; }
        public int? MenuItemCharLimit { get; set; }
    }
}
