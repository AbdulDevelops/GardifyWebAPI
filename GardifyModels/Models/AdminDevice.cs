using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class AdminDevice:_BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public bool isActive { get; set; }
        public bool notifyForWind { get; set; }
        public bool notifyForFrost { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
    }
    public class AdminDeviceListViewModel : _BaseViewModel
    {
        public IEnumerable<AdminDeviceViewModel> DevicesListEntries { get; set; }
        public _modalStatusMessageViewModel StatusMessage { get; set; }
    }
    public class AdminDeviceViewModel
    {
        public AdminDeviceViewModel()
        {
            DevicesImages = new List<_HtmlImageViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
        public bool notifyForWind { get; set; }
        public bool notifyForFrost { get; set; }
        public List<_HtmlImageViewModel> DevicesImages { get; set; }
        public string Note { get; set; }
        public _modalStatusMessageViewModel StatusMessage { get; set; }
    }
}