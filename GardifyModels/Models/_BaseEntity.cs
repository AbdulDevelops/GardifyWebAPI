using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GardifyModels.Models
{
	public class _BaseEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Required]
		[StringLength(256)]
		public string CreatedBy { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; }
		[StringLength(256)]
		public string EditedBy { get; set; }
		public DateTime EditedDate { get; set; }
		[Required]
		public bool Deleted { get; set; }



		public void OnCreate(string createdBy)
		{
			CreatedBy = createdBy;
			CreatedDate = DateTime.Now;
			EditedBy = createdBy;
			EditedDate = CreatedDate;
            Deleted = false;
        }

		public void OnEdit(string editedBy)
		{
			this.EditedBy = editedBy;
			EditedDate = DateTime.Now;
		}
	}
}