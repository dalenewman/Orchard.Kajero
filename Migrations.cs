using System;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;

namespace Kajero {

    public class Migrations : DataMigrationImpl {
        protected INotifier Notifier { get; set; }
        protected Localizer T { get; set; }
        protected ILogger Logger { get; set; }

        public Migrations(INotifier notifier) {
            Notifier = notifier;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public int Create() {

            try {

                var proxy = new ContentTypeDefinition("Kajero", "Kajero");
                ContentDefinitionManager.StoreTypeDefinition(proxy);
                ContentDefinitionManager.AlterTypeDefinition("Kajero", cfg => cfg.Creatable()
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                    .WithPart("ContentPermissionsPart", p => p
                        .WithSetting("ContentPermissionsPartSettings.View", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.ViewOwn", "Administrator,Authenticated")
                        .WithSetting("ContentPermissionsPartSettings.Publish", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.PublishOwn", "Administrator,Authenticated")
                        .WithSetting("ContentPermissionsPartSettings.Edit", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.EditOwn", "Administrator,Authenticated")
                        .WithSetting("ContentPermissionsPartSettings.Delete", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.DeleteOwn", "Administrator,Authenticated")
                        .WithSetting("ContentPermissionsPartSettings.Preview", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.PreviewOwn", "Administrator,Authenticated")
                        .WithSetting("ContentPermissionsPartSettings.DisplayedRoles", "Anonymous,Authenticated")
                    )
                    .WithPart("AutoroutePart", p => p
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "false")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "true")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Slugified Title', Pattern: 'Kajero/{Content.Slug}', Description: 'Slugified Title'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0")
                    )
                    .WithPart("BodyPart", p => p
                        .WithSetting("BodySettings.Flavor", "text")
                    )
                );

            } catch (Exception e) {
                var message = string.Format("Error creating Kajero module. {0}", e.Message);
                Logger.Warning(message);
                Notifier.Warning(T(message));
                return 0;
            }
            return 1;
        }

    }
}