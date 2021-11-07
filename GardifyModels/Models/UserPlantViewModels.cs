using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;
using static GardifyModels.Models.ArticleViewModels;
using static GardifyModels.Models.TodoViewModels;

namespace GardifyModels.Models
{
    public class UserPlantViewModels
    {
        public class UserPlantIndexViewModel : _BaseViewModel
        {
            public IEnumerable<UserPlantDetailsViewModel> UserPlants { get; set; }
        }

        public class UserPlantLightViewModel : IEquatable<UserPlantLightViewModel>
        {
            public UserPlantLightViewModel()
            {
                Images = new List<_HtmlImageViewModel>();
            }
            public List<_HtmlImageViewModel> Images { get; set; }
            public int PlantId { get; set; }
            public int Id { get; set; }
            public IEnumerable<Todo> Todos { get; set; }
            public IEnumerable<PlantTagSearchLite> Badges { get; set; }

            public string Name { get; set; }
            public string NameLatin { get; set; }
            public int Count { get; set; }
            public int Gardenid { get; set; }
            public int UserListId { get; set; }
            public string Notes { get; set; }
            public string Age { get; set; }
            public string Description { get; set; }
            public bool IsInPot { get; set; }

            public bool Equals(UserPlantLightViewModel other)
            {
                return this.PlantId == other.PlantId;
            }
        }
        public class UserPlantDetailsViewModelCount
        {
            public UserPlantDetailsViewModel UserPlant { get; set; }
            public string ListName { get; set; }
            public IEnumerable<string >ListNames { get; set; }
            public List<int> ListIds { get; set; }
            public int ListId { get; set; }
            public int Count { get; set; }
        }
        public class MoveUserplantToAnotherListVM
        {
            public int UserplantId { get; set; }
            public int NewListId { get; set; }
        }
        public class MoveAllUserplantsToAnotherListVM
        {
            public int CurrentListId { get; set; }
            public int NewListId { get; set; }
        }
        public class UserPlantDetailsViewModelCountList
        {
            public IEnumerable<UserPlantDetailsViewModelCount> UserPlantsList { get; set; }
            public int UserPlantInUserListCount { get; set; }
        }

        public class UserPlantFlowerDurationViewModelCount
        {
            public IEnumerable<UserPlantFlowerDurationViewModel> PlantFlowerDurationList { get; set; }
        }

        public class UserPlantFlowerDurationViewModel
        {
            public int Month { get; set; }

            public int PlantCount { get; set; }
        }

        public class UserPlantDetailsViewModel : _BaseViewModel
        {
            public UserPlantDetailsViewModel()
            {
                Images = new List<_HtmlImageViewModel>();
            }
            public int Id { get; set; }
            public int PlantId { get; set; }
            public string Description { get; set; }
            public virtual Plant Plant { get; set; }
            public int Count { get; set; }
            public int Gardenid { get; set; }
            public string Age { get; set; }
            public string Name { get; set; }
            public string NameLatin { get; set; }
            public string CustomName { get; set; }
            public string Synonym { get; set; }
            public bool IsInPot { get; set; }
            public string Notes { get; set; }

            public string CreatedNotes { get; set; }
            public DateTime DatePlanted { get; set; }
            public IEnumerable<PlantTag> PlantTag { get; set; }
            public IEnumerable<PlantTagSearchLite> Badges { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
            public IEnumerable<Todo> Todos { get; set; }
            public IEnumerable<TodoCyclicVM> CyclicTodos { get; set; }
            public TodoIndexViewModel TodosOld { get; set; }
            public FaqViewModels.FaqIndexViewModel FaqList { get; set; }
            public IEnumerable<VideoReferenceViewModels.VideoReferenceDetailsViewModel> VideoReferenceList { get; set; }
            public IEnumerable<ArticleViewModelLite> Articles { get; set; }
            public bool NotifyForFrost { get; set; }
            public bool NotifyForWind { get; set; }
        }

        public class UserPlantsCount
        {
            public int Sorts { get; set; }
            public int Total { get; set; }
        }
       

        public class UserPlantEditViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            public int PlantId { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
            public int Count { get; set; }
            [Required]
            [_Title]
            public string Name { get; set; }
            public int UserListId { get; set; }
            public string Notes { get; set; }
            public bool IsInPot { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
        }

        public class BorrowUserPlantViewModel : _BaseViewModel
        {
            public int GardenId { get; set; }
            public int PlantId { get; set; }
            public int InitialAgeInDays { get; set; }
            public int Count { get; set; }
            public bool IsInPot { get; set; }
            public List<_TodoCheckedTemplateViewModel> Todos { get; set; }
            public UserPlantToUserListView[] ArrayOfUserlist {get;set;}
        }

        public class UserPlantDeleteViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}