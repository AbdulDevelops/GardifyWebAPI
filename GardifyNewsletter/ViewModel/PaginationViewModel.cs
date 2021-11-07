using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GardifyNewsletter.ViewModel
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool EnablePrevious { get; set; }
        public bool EnableNext { get; set; }
        public string SelectedSector { get; set; }
        public string CurrentSort { get; set; }
        public string SearchString { get; set; }



    }
}
