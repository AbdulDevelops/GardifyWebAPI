using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GardifyModels.Models.UserPlantViewModels;

namespace GardifyModels.Models
{
    public class WarningViewModel
    {
        public WarningViewModel()
        {
            UserPlants = new List<UserPlantDetailsViewModel>();
            UserDevices = new List<Device>();
            Alerts = new List<AlertViewModels.AlertLiteViewModel>();
        }
        public UserSettings UserSettings { get; set; }
        public string UserName { get; set; }
        public List<AlertViewModels.AlertLiteViewModel> Alerts { get; set; }
        public float MaxWindSpeed { get; set; }
        public float MinTemp { get; set; }
        public DateTime Date { get; set; }
        public List<UserPlantDetailsViewModel> UserPlants { get; set; }
        public List<Device> UserDevices { get; set; }
    }
}