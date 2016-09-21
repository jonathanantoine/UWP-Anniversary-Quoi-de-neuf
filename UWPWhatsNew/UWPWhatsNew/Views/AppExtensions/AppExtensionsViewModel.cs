using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppExtensions;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using UWPWhatsNew.Common;
using UWPWhatsNew.Common.AsyncHelpers;

namespace UWPWhatsNew.Views.AppExtensions
{
    public class AppExtensionsViewModel : BindableBase
    {
        public AppExtensionsViewModel()
        {
            _catalog = AppExtensionCatalog.Open("licorne");

            _catalog.PackageInstalled += Catalog_OnPackageInstalled;
            _catalog.PackageUninstalling += Catalog_OnPackageUninstalling;
            _catalog.PackageUpdated += _catalog_OnPackageUpdated;
            _catalog.PackageStatusChanged += _catalog_OnPackageStatusChanged;

            InitAsync();
        }


        private async Task InitAsync()
        {
            var allInstalled = await _catalog.FindAllAsync();
            AppExtensions = allInstalled.ToList();
        }

        private List<AppExtension> _appExtensions = new List<AppExtension>();
        private readonly AppExtensionCatalog _catalog;

        public List<AppExtension> AppExtensions
        {
            get { return _appExtensions; }
            set { SetProperty(ref _appExtensions, value); }
        }


        private void _catalog_OnPackageUpdated(AppExtensionCatalog sender, AppExtensionPackageUpdatedEventArgs args)
        {
            RefreshOnDispatcherThread();
        }

        private void Catalog_OnPackageUninstalling(AppExtensionCatalog sender, AppExtensionPackageUninstallingEventArgs args)
        {
            RefreshOnDispatcherThread();
        }

        private void Catalog_OnPackageInstalled(AppExtensionCatalog sender, AppExtensionPackageInstalledEventArgs args)
        {
            RefreshOnDispatcherThread();
        }

        private void _catalog_OnPackageStatusChanged(AppExtensionCatalog sender, AppExtensionPackageStatusChangedEventArgs args)
        {
            RefreshOnDispatcherThread();
        }

        private void RefreshOnDispatcherThread()
        {
            DispatcherHelper.UIDispatcher.RunIdleAsync
                (
                 _ => InitAsync()
                );
        }

        public async Task LaunchExtensionAsync(AppExtension appExtension)
        {
            // Il y a 2 types de packages, le bon et le mauvais
            // le bon, il est bon alors que le mauvais il est mauvais.
            if (false == appExtension.Package.Status.VerifyIsOK())
            {
                return;
            }

            var targetFolder = await appExtension.GetPublicFolderAsync();

            var insideFiles = (await targetFolder.GetFilesAsync()).ToList();

            var dN = appExtension.DisplayName;
            var desc = appExtension.Description;
            var pfn = appExtension.AppInfo.PackageFamilyName;


            //ap

        }

        public async Task AskRemoveExtensionAsync(AppExtension appExtension)
        {
            var pfn = appExtension.AppInfo.PackageFamilyName;

         var r = await   _catalog.RequestRemovePackageAsync(appExtension.Package.Id.FullName);
        }
    }
}
