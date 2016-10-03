using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml.Media.Imaging;
using UWPWhatsNew.Common;

namespace UWPWhatsNew.Views.AppExtensions
{
    public class AppExtensionsViewModel : BindableBase
    {

        private BitmapImage _outputImage;

        public BitmapImage OutputImage
        {
            get { return _outputImage; }
            set { SetProperty(ref _outputImage, value); }
        }

        private bool _isAppExtensionListEmpty;

        public bool IsAppExtensionListEmpty
        {
            get { return _isAppExtensionListEmpty; }
            set { SetProperty(ref _isAppExtensionListEmpty, value); }
        }


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
            await Task.Delay(1000);
            var allInstalled = await _catalog.FindAllAsync();
            AppExtensions = allInstalled.ToList();

            IsAppExtensionListEmpty = !AppExtensions.Any();
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

            // création d'une connexion
            var connection = new AppServiceConnection
            {
                AppServiceName = "com.infinitesquare.UWPWhatsNewExtension",

                // ciblage de l'extension
                PackageFamilyName = appExtension.Package.Id.FamilyName
            };

            var cameraCaptureUI = new CameraCaptureUI { PhotoSettings = { CroppedAspectRatio = new Size(1, 1), Format = CameraCaptureUIPhotoFormat.Jpeg } };
            var file = await cameraCaptureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file == null)
            {
                return;
            }

            var token = SharedStorageAccessManager.AddFile(file);

            // ouverture de la connexion
            var result = await connection.OpenAsync();

            if (result != AppServiceConnectionStatus.Success)
            {
                return;
            }

            var inputs = new ValueSet { { "targetFileToken", token } };

            // send input and receive output in a variable
            var response = await connection.SendMessageAsync(inputs);

            var outputFileToken = response.Message["outputFileToken"].ToString();

            var outputFile = await SharedStorageAccessManager.RedeemTokenForFileAsync(outputFileToken);

            var copied = await outputFile.CopyAsync(ApplicationData.Current.TemporaryFolder, outputFile.Name);

            OutputImage = new BitmapImage(new Uri(copied.Path));
        }

        public async Task AskRemoveExtensionAsync(AppExtension appExtension)
        {
            var pfn = appExtension.AppInfo.PackageFamilyName;

            var r = await _catalog.RequestRemovePackageAsync(appExtension.Package.Id.FullName);
        }
    }
}
