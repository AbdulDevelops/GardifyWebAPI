using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class VideoEntry : _BaseEntity
    {
        public string YTLink { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Text { get; set; }
    }

    public class VideoEntryViewModel
    {
        public int Id { get; set; }
        public string YTLink { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Text { get; set; }
        public int ViewCount { get; set; }
        public string Duration { get; set; }
        public List<string> Tags { get; set; }
        public DateTime Date { get; set; }
    }
}