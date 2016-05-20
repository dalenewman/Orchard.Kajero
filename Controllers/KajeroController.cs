using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Kajero.Models;
using Orchard;
using Orchard.Alias.Implementation.Holder;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace Kajero.Controllers {

    [Themed(false)]
    public class KajeroController : Controller {

        private readonly IOrchardServices _services;
        private readonly IAliasHolder _aliasHolder;
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        public KajeroController(
            IOrchardServices services,
            IAliasHolder aliasHolder
        ) {
            _services = services;
            _aliasHolder = aliasHolder;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        [HttpPost]
        public ActionResult Save() {

            // if (User.Identity.IsAuthenticated) {

                var item = _services.ContentManager.Get(Request.Form["id"] == null ? 0 : Convert.ToInt32(Request.Form["id"]));

                if (item == null)
                    return new HttpNotFoundResult();

                if (item.As<CommonPart>().Owner.UserName == User.Identity.Name || _services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.EditContent, item)) {
                    item.As<BodyPart>().Text = Request.Form["markdown"] ?? item.As<BodyPart>().Text;
                    return new JsonResult { Data = new SaveResponse() };
                }

                return new HttpUnauthorizedResult();

            //}
            //return new HttpUnauthorizedResult();
        }

        public ActionResult Display(string slug) {

            if (User.Identity.IsAuthenticated) {

                IDictionary<string, string> routeValues;
                if (!_aliasHolder.GetMap("Contents").TryGetAlias("Kajero/" + slug, out routeValues)) {
                    return new HttpNotFoundResult();
                }
                var id = Convert.ToInt32(routeValues["Id"]);
                var item = _services.ContentManager.Get(id);

                if (item == null)
                    return new HttpNotFoundResult();

                if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, item, T("Permission to view this page is denied")))
                    return new HttpUnauthorizedResult();

                return View("Display", item);
            }

            System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            return null;
        }


    }

}