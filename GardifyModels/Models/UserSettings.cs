using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserSettings: _BaseEntity
    {
        public Guid UserId { get; set; }
        public bool ActiveStormAlert { get; set; }
        public bool ActiveFrostAlert { get; set; }
        public bool ActiveNewPlantAlert { get; set; }
        public bool AlertByEmail { get; set; }
        public bool AlertByPush { get; set; }
        public int FrostDegreeBuffer { get; set; }

        [DefaultValue(false)]
        public bool AlertStormEmailDisable { get; set; }
        [DefaultValue(false)]

        public bool AlertFrostEmailDisable { get; set; }
    }

    public class UserSettingsViewModel
    {
        public Guid UserId { get; set; }
        public bool ActiveStormAlert { get; set; }
        public bool ActiveFrostAlert { get; set; }
        public bool ActiveNewPlantAlert { get; set; }
        public bool AlertByEmail { get; set; }
        public bool AlertByPush { get; set; }
        public int FrostDegreeBuffer { get; set; }

        public bool AlertStormEmailDisable { get; set; }

        public bool AlertFrostEmailDisable { get; set; }
    }
}