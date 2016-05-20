using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Kajero {
    public class Routes : IRouteProvider {

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 1001,
                    Route = new Route(
                        "Kajero/Save",
                        new RouteValueDictionary {
                            {"area", "Kajero"},
                            {"controller", "Kajero"},
                            {"action", "Save"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Kajero"}
                        },
                        new MvcRouteHandler())
                },

                new RouteDescriptor {
                    Priority = 1000,  //otherwise autoroute picks it up and sends it to default content route
                    Route = new Route(
                        "Kajero/{slug}",
                        new RouteValueDictionary {
                            {"area", "Kajero"},
                            {"controller", "Kajero"},
                            {"action", "Display"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Kajero"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
    }
}