using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyModels.Models
{
    public class InsatPostEntry : _BaseEntity
    {
     
            public string PostId { get; set; }
            [AllowHtml]
            public string Caption { get; set; }
            public string MediaType { get; set; }

            public string MediaUrl { get; set; }
        public string PermaLink { get; set; }

        public string Username { get; set; }

            public string Timestamp { get; set; }

            public string ThumbnailUrl { get; set; }
            public string RelatedLink { get; set; }
            public string ChildrenId { get; set; }
            //public string PermaLink { get; set; }



    }

    public class InsatPostEntryView
    {
        public InsatPostEntryView(InsatPostEntry model)
        {
            PostId = model.PostId;
            Caption = model.Caption;
            MediaType = model.MediaType;
            MediaUrl = model.MediaUrl;
            Username = model.Username;
            Timestamp = model.Timestamp;
            ThumbnailUrl = model.ThumbnailUrl;
            RelatedLink = model.RelatedLink;
            ChildrenId = model.ChildrenId;
            EncodedIdUrl = "https://www.instagram.com/p/" + encodeInstaId(model.PostId);
        }

        public string PostId { get; set; }
        [AllowHtml]
        public string Caption { get; set; }
        public string MediaType { get; set; }

        public string MediaUrl { get; set; }

        public string Username { get; set; }

        public string Timestamp { get; set; }

        public string ThumbnailUrl { get; set; }
        public string RelatedLink { get; set; }
        public string ChildrenId { get; set; }

        public string EncodedIdUrl { get; set; }


        public string encodeInstaId(string id)
        {
            string postId = "";
            long longId = long.Parse(id);
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
            while (longId > 0)
            {
                long remainder = (longId % 64);
                longId = (longId - remainder) / 64;
                postId = alphabet[(int)remainder] + postId;
            }
            return postId;
        }



    }
}