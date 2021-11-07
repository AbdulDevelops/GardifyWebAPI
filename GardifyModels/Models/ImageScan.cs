using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GardifyModels.Models.GoogleAPIResponse;

namespace GardifyModels.Models
{
    public class ImageScan : _BaseEntity
    {
        public Guid UserId { get; set; }
        public string ImageFileName { get; set; }

        public string ImageFilePath { get; set; }

        public DateTime Date { get; set; }
        public string GName { get; set; }
        public string PnName { get; set; }
        public string Family { get; set; }
    }

    public class ImageScanViewModel
    {
        public string ImageFileName { get; set; }

        public string ImageFilePath { get; set; }

        public DateTime Date { get; set; }
        public string GName { get; set; }
        public string PnName { get; set; }
        public string Family { get; set; }
    }

    public class SearchResult
    {
        public List<PlantViewModels.PlantViewModel> GPlants { get; set; }
        public List<ImageUrl> GImages { get; set; }
        public PlantNetResult PnResults { get; set; }
    }

    public class PlantNetResult
    {
        public List<PnPlant> results { get; set; }

        public List<PnPlant> doubleResult { get; set; }
        public List<PlantViewModels.PlantViewModel> InDb { get; set; }
    }

    public class PnResponse
    {
        [Key]
        public string session { get; set; }
        public List<PnPlant> results { get; set; }
    }

    public class PnPlant
    {
        [Key]
        public float score { get; set; }
        public PnSpeciesObj species { get; set; }
        public List<GSImage> images { get; set; }
    }

    public class CustomSearchResults
    {
        public List<GSImage> items { get; set; }
    }

    public class GSImage
    {
        public string link { get; set; } // original image
        public GSImageDetail image { get; set; }
    }

    public class GSImageDetail
    {
        public string thumbnailLink { get; set; } // image thumbnail
        public string contextLink { get; set; } // image homepage
    }

    public class PnSpeciesObj
    {
        public string[] commonNames { get; set; }
        [Key]
        public string scientificNameWithoutAuthor { get; set; }
        public PnFamily family { get; set; }
        public PnGenus genus { get; set; }
    }

    public class PnFamily
    {
        public string scientificNameWithoutAuthor { get; set; }
    }

    public class PnGenus
    {
        public string scientificNameWithoutAuthor { get; set; }
    }

    public class PnImage
    {
        public string author { get; set; }
        public string id { get; set; }
        public string m_url { get; set; }
        public string s_url { get; set; }
        public string url { get; set; }
        public string tag { get; set; }
        public string score { get; set; }
    }
}