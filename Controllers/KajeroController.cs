using System;
using System.Web.Mvc;
using Kajero.Models;
using Orchard;
using Orchard.Alias.Implementation.Storage;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace Kajero.Controllers {

    [Themed(false)]
    public class KajeroController : Controller {

        private readonly IOrchardServices _services;
        private readonly IAliasStorage _aliasStorage;
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        public KajeroController(
            IOrchardServices services,
            IAliasStorage aliasStorage
        ) {
            _services = services;
            _aliasStorage = aliasStorage;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        [HttpPost]
        public ActionResult Save() {

            if (User.Identity.IsAuthenticated) {

                var item = _services.ContentManager.Get(Request.Form["id"] == null ? 0 : Convert.ToInt32(Request.Form["id"]));

                if (item == null)
                    return new HttpNotFoundResult();

                if (item.As<CommonPart>().Owner.UserName == User.Identity.Name || _services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.EditContent, item)) {
                    item.As<BodyPart>().Text = Request.Form["markdown"] ?? item.As<BodyPart>().Text;
                    return new JsonResult { Data = new SaveResponse() };
                }

                return new HttpUnauthorizedResult();

            }
            return new HttpUnauthorizedResult();
        }

        public ActionResult Display(string slug) {

            if (User.Identity.IsAuthenticated) {


                var path = "Kajero/" + slug;
                var routeValues = _aliasStorage.Get(path);  //note: this is compatible betwen 1.8 and 1.10

                if(routeValues == null)
                    return new HttpNotFoundResult();

                var item = _services.ContentManager.Get(Convert.ToInt32(routeValues["Id"]));

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