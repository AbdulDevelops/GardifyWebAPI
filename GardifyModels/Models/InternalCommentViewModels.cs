using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class InternalCommentViewModels 
    {
        public class InternalCommentDetailsViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public int ReferenceId { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public bool Finished { get; set; }
            public string UserName { get; set; }
            public string ReferenceName { get; set; }
        }

        public class InternalCommentIndexViewModel : _BaseViewModel
        {
            public IEnumerable<InternalCommentDetailsViewModel> ListOfComments { get; set; }
        }

        public class InternalCommentCreateViewModel : _BaseViewModel
        {
            [DisplayName("Kommentartext")]
            public string Text { get; set; }
            public int ReferenceID { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
        }
    }
}