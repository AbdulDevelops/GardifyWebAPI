using GardifyModels.Models;
using System.Net;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class ErrorController : _BaseController
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("~/Views/Error/Details.cshtml", new ErrorViewModels.ErrorDetailsViewModel());
        }

        public ActionResult Details(Error error)
        {
            DbAddError(error);
            ErrorViewModels.ErrorDetailsViewModel evm = new ErrorViewModels.ErrorDetailsViewModel()
            {
                ErrorMessage = error.ErrorMessage,
                HttpStatusCode = error.HttpStatusCode
            };

            if (error.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return Error404View(evm);
            }
            else
            {
                return View("~/Views/Error/Details.cshtml", evm);
            }
        }

        public ActionResult Error404View(ErrorViewModels.ErrorDetailsViewModel evm)
        {
            return View("~/Views/Error/Details404.cshtml", evm);
        }

        public ActionResult Error404DefaultView()
        {
            return View("~/Views/Error/Details404.cshtml");
        }
       
        #region Db

        [NonAction]
        public void DbAddError(Error err)
        {
            ctx.Errors.Add(err);
            ctx.SaveChanges();
        }

        #endregion
    }
}