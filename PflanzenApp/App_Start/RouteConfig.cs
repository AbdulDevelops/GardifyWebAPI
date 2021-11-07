using System.Web.Mvc;
using System.Web.Routing;

namespace PflanzenApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "MobileView",
				url: "m",
				defaults: new { controller = "Home", action = "Startpage", id = UrlParameter.Optional }				
			);

            routes.MapRoute(
                name: "AdminAreaImport",
                url: "i/import/{action}/{id}",
                defaults: new { controller = "AdminAreaImport", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaTaxonomicTree",
                url: "i/taxonomic-tree/{action}/{id}",
                defaults: new { controller = "AdminAreaTaxonomicTree", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
				name: "AdminAreaPlant",
				url: "i/plant/{action}/{id}",
				defaults: new { controller = "AdminAreaPlant", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "Controllers.AdminArea" }
			);

			routes.MapRoute(
				name: "AdminAreaPlantTag",
				url: "i/plant-tag/{action}/{id}",
				defaults: new { controller = "AdminAreaPlantTag", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] {"Controllers.AdminArea" }
			);

			routes.MapRoute(
				name: "AdminAreaFAQ",
				url: "i/faq/{action}/{id}",
				defaults: new { controller = "AdminAreaFaq", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "Controllers.AdminArea" }
			);

			routes.MapRoute(
				name: "AdminAreaNews",
				url: "i/news/{action}/{id}",
				defaults: new { controller = "AdminAreaNews", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "Controllers.AdminArea" }
			);

            routes.MapRoute(
                name: "AdminAreaLaunchPage",
                url: "i/launchpage/{action}/{id}",
                defaults: new { controller = "AdminAreaLaunchPage", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
				name: "AdminAreaArticle",
				url: "i/shop/{action}/{id}",
				defaults: new { controller = "AdminAreaArticle", action = "Overview", id = UrlParameter.Optional },
				namespaces: new[] { "Controllers.AdminArea" }
			);

            routes.MapRoute(
                name: "AdminAreaUtils",
                url: "i/utils/{action}/{id}",
                defaults: new { controller = "AdminAreaUtils", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaPoints",
                url: "i/points/{action}/{id}",
                defaults: new { controller = "AdminAreaPoints", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaTodo",
                url: "i/todo/{action}/{id}",
                defaults: new { controller = "AdminAreaTodo", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaParentGardenCategories",
                url: "i/parent-garden-category/{action}/{id}",
                defaults: new { controller = "AdminAreaParentGardenCategories", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
               name: "AdminAreaGroups",
               url: "i/groups/{action}/{id}",
               defaults: new { controller = "AdminAreaGroups", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Controllers.AdminArea" }
           );

            routes.MapRoute(
                name: "AdminAreaGardenCategories",
                url: "i/garden-category/{action}/{id}",
                defaults: new { controller = "AdminAreaGardenCategories", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
               name: "AdminAreaDevices",
               url: "i/devices/{action}/{id}",
               defaults: new { controller = "AdminAreaDevices", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Controllers.AdminArea" }
           );

            routes.MapRoute(
                name: "AdminAreaContentContribution",
                url: "i/content/{action}/{id}",
                defaults: new { controller = "AdminAreaContentContribution", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaAccount",
                url: "i/account/{action}/{id}",
                defaults: new { controller = "AdminAreaAccount", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaWarnings",
                url: "i/warnings/{action}/{id}",
                defaults: new { controller = "AdminAreaWarnings", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
               name: "AdminAreaVorschlagen",
               url: "i/vorschlagen/{action}/{id}",
               defaults: new { controller = "AdminAreaVorschlagen", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Controllers.AdminArea" }
           );

            routes.MapRoute(
                name: "AdminAreaOrders",
                url: "i/orders/{action}/{id}",
                defaults: new { controller = "AdminAreaOrders", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaVideo",
                url: "i/videos/{action}/{id}",
                defaults: new { controller = "AdminAreaVideo", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaArticleCategories",
                url: "i/article-categories/{action}/{id}",
                defaults: new { controller = "AdminAreaArticleCategories", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaEcoElements",
                url: "i/eco-elements/{action}/{id}",
                defaults: new { controller = "AdminAreaEcoElements", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaLexiconTerms",
                url: "i/lexicon/{action}/{id}",
                defaults: new { controller = "AdminAreaLexiconTerms", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "AdminAreaStats",
                url: "i/stats/{action}/{id}",
                defaults: new { controller = "AdminAreaStats", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
               name: "AdminAreaPlantDoc",
               url: "i/plant-doc/{action}/{id}",
               defaults: new { controller = "AdminAreaPlantDoc", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Controllers.AdminArea" }
           );

            routes.MapRoute(
              name: "AdminAreaGardenNotes",
              url: "i/garden-notes/{action}/{id}",
              defaults: new { controller = "AdminAreaGardenNotes", action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "Controllers.AdminArea" }
          );

            routes.MapRoute(
             name: "AdminAreaSettings",
             url: "i/settings/{action}/{id}",
             defaults: new { controller = "AdminAreaSettings", action = "Index", id = UrlParameter.Optional },
             namespaces: new[] { "Controllers.AdminArea" }
         );

            routes.MapRoute(
              name: "AdminAreaHome",
              url: "i/home/{action}/{id}",
              defaults: new { controller = "AdminAreaHome", action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "Controllers.AdminArea" }
          );

            routes.MapRoute(
                name: "AdminAreaDefault",
                url: "i/",
                defaults: new { controller = "AdminAreaPlant", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Controllers.AdminArea" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
		}
    }
}
