using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileUploadLib.Services
{
    public class ViewRenderService
    {
        private IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly HttpContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewRenderService(IRazorViewEngine viewEngine, IServiceProvider serviceProvider,
            IHttpContextAccessor accessor, ITempDataProvider tempDataProvider)
        {
            _httpContextAccessor = accessor;
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
            _context = accessor.HttpContext;
        }

        public string Render(string viewPath)
        {
            return Render(viewPath, string.Empty);
        }

        public string Render<TModel>(string viewPath, TModel model)
        {
            var actionContext = new ActionContext(_context, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                try
                {
                    var viewEngineResult = _viewEngine.GetView("~/", viewPath, false);

                    if (viewEngineResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewPath} does not match any available view");
                    }

                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };

                    var viewContext = new ViewContext(
                        actionContext,
                        viewEngineResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    );

                    viewEngineResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    //throw new TemplateServiceException("Failed to render template due to a razor engine failure", ex);
                }
                return sw.ToString();
            }
        }
    }
}
