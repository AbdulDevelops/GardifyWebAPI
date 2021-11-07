using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    public class SearchQueryAPIController : ApiController
    {
        private SearchQueryController sc = new SearchQueryController();

        // GET: api/SearchQueryAPI/5
        [HttpGet]
        [Route("api/SearchQueryAPI/count")]
        public int CountQueries()
        {
            return sc.CountQueries(); ;
        }

        [HttpGet]
        [Route("api/SearchQueryAPI/")]
        public IEnumerable<SearchQuery> GetQueries()
        {
            Guid userId = Utilities.GetUserId();
            return sc.DbGetSearchQueriesByUserId(userId);
        }

        [HttpPost]
        [Route("api/SearchQueryAPI")]
        // POST: api/SearchQueryAPI
        public void Post(QueryStringVM queries)
        {
            sc.Create(queries);
        }
    }

    public class QueryStringVM 
    {
        public string QueryString { get; set; }
    }

}
