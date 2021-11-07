using GardifyModels.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class VideoReferenceController : _BaseController
    {

        #region DB
        [NonAction]
        public IEnumerable<VideoReferenceViewModels.VideoReferenceDetailsViewModel> GetVideoDetailsViewModelList(int refId, ModelEnums.ReferenceToModelClass refType)
        {
            var videos = DbGetVideoReferenceList(refId, refType);
            List<VideoReferenceViewModels.VideoReferenceDetailsViewModel> list = new List<VideoReferenceViewModels.VideoReferenceDetailsViewModel>();
            foreach (VideoReference vid in videos){
                VideoReferenceViewModels.VideoReferenceDetailsViewModel vidViewModel = new VideoReferenceViewModels.VideoReferenceDetailsViewModel()
                {
                    Description = vid.Description,
                    Height = vid.Height,
                    Width = vid.Width,
                    Title = vid.Title,
                    VideoId = vid.VideoId
                };
                list.Add(vidViewModel);
            }
            return list;
        }

        [NonAction]
        public IEnumerable<VideoReference> DbGetVideoReferenceList(int refId, ModelEnums.ReferenceToModelClass refType)
        {
            var videos = (from ctx in ctx.VideoReference
                          where ctx.Deleted == false &&
                          ctx.ReferenceId == refId &&
                          ctx.ReferenceTypeId == refType
                          select ctx).ToList();
            return videos;
        }
        #endregion
    }
}