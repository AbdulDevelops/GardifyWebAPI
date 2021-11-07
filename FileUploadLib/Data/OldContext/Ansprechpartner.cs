using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Ansprechpartner
    {
        public int Id { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public string Telefon { get; set; }
        public string Telefon2 { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string ProfilBild { get; set; }
        public string Instanz { get; set; }
        public int EntityId { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
    }
}
