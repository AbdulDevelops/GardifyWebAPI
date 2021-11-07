using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models.GoogleAPIResponse
{

    public class LabelAnnotation: _BaseEntity
    {
        public string mid { get; set; }
        public string description { get; set; }
        public double score { get; set; }
        public double topicality { get; set; }
    }

    public class WebDetection
    {
        public List<ImageUrl> visuallySimilarImages { get; set; }
    }

    public class ImageUrl
    {
        public string url { get; set; }
    }

    public class Response
    {
        public List<LabelAnnotation> labelAnnotations { get; set; }
        public WebDetection webDetection { get; set; }
    }

    public class RootObject
    {
        public List<Response> responses { get; set; }
    }

    public class TranslateObject
    {
        // words to translate
        public List<string> q { get; set; }

        // target language. Ex: en, de, fr
        public string target { get; set; }
    }

    public class TranslateResponse
    {
        public TranslatedDataObj data { get; set; }
    }

    public class TranslatedDataObj
    {
        public List<TranslatedWord> translations { get; set; }
    }

    public class TranslatedWord
    {
        public string translatedText { get; set; }
        public string detectedSourceLanguage { get; set; }
    }
}

namespace GardifyModels.Models.GoogleAPIRequest
{
    public class Image
    {
        public string content { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public int maxResults { get; set; }
    }

    public class ImageContext
    {
        public List<string> languageHints { get; set; }
    }

    public class Request
    {
        public Image image { get; set; }
        public List<Feature> features { get; set; }
        public ImageContext imageContext { get; set; }
    }

    public class RootObject
    {
        public List<Request> requests { get; set; }
    }
}