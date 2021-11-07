using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileUploadLib.Data
{
    public abstract class _BaseModel : IBaseModel<int>
    {
        public abstract int Id { get; }
        [Display(Name = "Erstellt am")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Erstellt von")]
        [ScaffoldColumn(false)]
        [StringLength(128)]
        public string CreatedBy { get; set; }
        [Display(Name = "Bearbeitet am")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [ScaffoldColumn(false)]
        public DateTime EditedDate { get; set; }
        [Display(Name = "Bearbeitet von")]
        [ScaffoldColumn(false)]
        [StringLength(128)]
        public string EditedBy { get; set; }
        [Display(Name = "Archiviert")]
        [ScaffoldColumn(false)]
        public bool Deleted { get; set; }

        [Display(Name = "Alte Id")]
        [ScaffoldColumn(false)]
        public int? OldId { get; set; }

        public void OnCreate()
        {
            var userName = "SYSTEM";
            OnCreate(userName);
        }

        public void OnCreate(string userName)
        {
            CreatedBy = userName;
            CreatedDate = DateTime.Now;
            EditedBy = userName;
            EditedDate = DateTime.Now;
        }

        public void OnDelete()
        {
            var userName = "SYSTEM";
            OnDelete(userName);
        }

        public void OnDelete(string userName)
        {
            EditedBy = userName;
            EditedDate = DateTime.Now;
            Deleted = true;
        }

        public void OnEdit()
        {
            var userName = "SYSTEM";
            OnEdit(userName);
        }

        public void OnEdit(string userName)
        {
            EditedBy = userName;
            EditedDate = DateTime.Now;
        }
    }

    public abstract class _BaseGuidModel : IBaseModel<Guid>
    {
        public abstract Guid Id { get; }
        [Display(Name = "Erstellt am")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Erstellt von")]
        [ScaffoldColumn(false)]
        [StringLength(128)]
        public string CreatedBy { get; set; }
        [Display(Name = "Bearbeitet am")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [ScaffoldColumn(false)]
        public DateTime EditedDate { get; set; }
        [Display(Name = "Bearbeitet von")]
        [ScaffoldColumn(false)]
        [StringLength(128)]
        public string EditedBy { get; set; }
        [Display(Name = "Archiviert")]
        [ScaffoldColumn(false)]
        public bool Deleted { get; set; }

        public void OnCreate()
        {
            var userName = "SYSTEM";
            OnCreate(userName);
        }

        public void OnCreate(string userName)
        {
            CreatedBy = userName;
            CreatedDate = DateTime.Now;
            EditedBy = userName;
            EditedDate = DateTime.Now;
        }

        public void OnDelete()
        {
            var userName = "SYSTEM";
            OnDelete(userName);
        }

        public void OnDelete(string userName)
        {
            EditedBy = userName;
            EditedDate = DateTime.Now;
            Deleted = true;
        }

        public void OnEdit()
        {
            var userName = "SYSTEM";
            OnEdit(userName);
        }

        public void OnEdit(string userName)
        {
            EditedBy = userName;
            EditedDate = DateTime.Now;
        }
    }

    public interface IBaseModel<TKey>
    {
        TKey Id { get; }
        DateTime CreatedDate { get; set; }
        [StringLength(100)]
        string CreatedBy { get; set; }
        DateTime EditedDate { get; set; }
        [StringLength(100)]
        string EditedBy { get; set; }
        bool Deleted { get; set; }
        void OnCreate();
        void OnCreate(string userName);
        void OnEdit();
        void OnEdit(string userName);
        void OnDelete();
        void OnDelete(string userName);
    }
}
