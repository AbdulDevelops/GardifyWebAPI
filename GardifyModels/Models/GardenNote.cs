using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class GardenNote: _BaseEntity
    {
        public string NoteTitle { get; set; }
    }

    public class GardenNoteCreateModel
    {
        public string NoteTitle { get; set; }
    }

    public class GardenNoteEditModel
    {
        public int Id { get; set; }

        public string NoteTitle { get; set; }
    }

}