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

namespace GardifyNewsletter.Services
{
    public class TemplateService : ITemplateService
    {
        private IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly HttpContext _context;
        private readonly IHttpContextAccessor _accessor;

        public TemplateService(IRazorViewEngine viewEngine, IServiceProvider serviceProvider,
            IHttpContextAccessor accessor, ITempDataProvider tempDataProvider)
        {
            _accessor = accessor;
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
            _context = accessor.HttpContext;
        }

        public async Task<string> RenderTemplateAsync(string filename, object viewModel)
        {
            var actionContext = new ActionContext(_context, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                try
                {
                    var viewResult = _viewEngine.FindView(actionContext, filename, false);

                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{filename} does not match any available view");
                    }

                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = viewModel
                    };

                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    );
                    viewContext.RouteData = _context.GetRouteData();

                    await viewResult.View.RenderAsync(viewContext);
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