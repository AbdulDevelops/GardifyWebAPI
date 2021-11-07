using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class Event: _BaseEntity
    {
        [Required]
        public string Title { get; set; }
        public string Address { get; set; }
        public string Organizer { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsVisibleOnPage { get; set; }
    }

    public class EventViewModel
    {
        public EventViewModel()
        {
            EntryImages = new List<_HtmlImageViewModel>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Organizer { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsVisibleOnPage { get; set; }
        public List<_HtmlImageViewModel> EntryImages { get; set; }
    }

    public class EventsListViewModel
    {
        public EventsListViewModel()
        {
            Events = new List<EventViewModel>();
        }
        public List<EventViewModel> Events { get; set; }
    }
}