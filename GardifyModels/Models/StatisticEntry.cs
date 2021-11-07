using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class StatisticEntry
    {
        public int StatisticEntryId { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("StatisticEvent")]
        public int EventId { get; set; }

        public int ObjectId { get; set; }
        public EventObjectType ObjectType { get; set; }

        public virtual StatisticEvent StatisticEvent { get; set; }

        public DateTime Date { get; set; }
        public string PossibleAddress { get; set; }
        public bool DemoMode { get; set; }
        public int ApiCallEventId { get; set; }
    }

    public enum EventObjectType
    {
        NotSet = 0, PageName = 1, Article = 2
    }

    public enum GardifyPages
    {
        Home = 1, News = 2, Shop = 3, Garden = 4, AZTerms = 5, Todo = 6, Search = 7, Scanner = 8, Weather = 9, PlantDoc = 10, Videos = 11, BioScan = 12, Settings = 13, Team = 14, Community = 15
    }
}