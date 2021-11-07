using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardifyModels.Models
{
    public class VideoReferenceViewModels
    {
        public class VideoReferenceDetailsViewModel : _BaseViewModel
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string VideoId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

            public string GetHmlCode()
            {
                string code = "<iframe width=\"{0}\" height=\"{1}\" src =\"https://www.youtube.com/embed/{2}?rel=0\" frameborder =\"0\" allowfullscreen ></iframe>";
                code = string.Format(code, Width, Height, VideoId);
                return code;
            }
        }
    }
}
