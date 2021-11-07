using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
    public class SynonymViewModels
    {
        public class SynonymIndexViewModel : _BaseViewModel
        {
            public IEnumerable<SynonymDetailsViewModel> SynonymList { get; set; }
        }

        public class SynonymDetailsViewModel : _BaseViewModel
        {
           public int Id { get; set; }
            [DisplayName("Synonym-Text")]
            public string Text { get; set; }
            [DisplayName("Gehört zu")]
            public string ReferenceName { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
        }

        public class SynonymCreateViewModel : _BaseViewModel
        {
            [DisplayName("Synonym-Text")]
            [Required]
            [_Title]
            public string Text { get; set; }
            public int ReferenceId { get; set; }
            [DisplayName("Gehört zu")]
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            [DisplayName("Wähle zugehöriges Objekt")]
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }

        }

        public class SynonymEditViewModel : _BaseViewModel
        {
            [DisplayName("Synonym-Text")]
            [Required]
            [_Title]
            public string Text { get; set; }
            public int ReferenceId { get; set; }
            [DisplayName("Gehört zu")]
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            [DisplayName("Zugehöriges Objekt")]
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }
            public int Id { get; set; }
        }
        public class SynonymDeleteViewModel : _BaseViewModel
        {
            public string Text { get; set; }
            public int Id { get; set; }
        }
    }
}
