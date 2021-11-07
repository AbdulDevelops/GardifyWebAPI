using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/LexiconAPI")]
    public class LexiconAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/LexiconAPI
        public IEnumerable<LexiconTermVM> GetLexiconTerms(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var userId = Utilities.GetUserId();
            var Terms = db.LexiconTerms.Where(t => !t.Deleted).OrderBy(t => t.Name);
            List<LexiconTermVM> res = new List<LexiconTermVM>();
            foreach (LexiconTerm term in Terms) {
                LexiconTermVM vm = new LexiconTermVM()
                {
                    Name = term.Name,
                    Description = term.Description
                };
                vm.Images = GetLexiconTermImages(term.Id);
                res.Add(vm);
            }
            
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, userId, (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.AZTerms, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, userId, (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.AZTerms, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, userId, (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.AZTerms, EventObjectType.PageName);
                }


            
            return res;
        }

        private List<_HtmlImageViewModel> GetLexiconTermImages(int termId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            List<_HtmlImageViewModel> ret = new List<_HtmlImageViewModel>();

            HelperClasses.DbResponse imageResponse = rc.DbGetLexiconTermReferencedImages(termId);
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                ret = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }
            else
            {
                ret.Add(new _HtmlImageViewModel
                {
                    SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return ret;
        }
    }
}
