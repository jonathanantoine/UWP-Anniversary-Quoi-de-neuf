using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml.Controls;
using UWPWhatsNew.Common;
using UWPWhatsNew.Common.AsyncHelpers;

namespace UWPWhatsNew.Views.ConnectedApps
{
    public class ConnectedAppViewModel : BindableBase
    {
        private readonly AsyncLock _listDevicesAsyncLock = new AsyncLock();
        private RemoteSystemWatcher _watcher;

        public ConnectedAppViewModel()
        {
            ResetLaunchUri();
            ListDevicesAsync();
            AvailableImages = new List<string>
            {
                "ms-appx:///Assets/Images/unicorn.png",
             "http://www2.mes-coloriages-preferes.biz/colorino/Images/Large/Personnages-feeriques-Licorne-117432.png",
             "http://ideas.microsoft.fr/wp-content/uploads/2016/02/Logo-infinite-square.png",
             "http://iconshow.me/media/images/xmas/standard-new-year-icons/256/Snowflake-icon.png",
             "https://compass-ssl.microsoft.com/assets/a9/98/a9980443-667b-4bdf-a08d-0048ffa52ab7.png?n=Lumia-950-XL-catalogue-DSIM-white-offers.png",
             "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5f/Windows_logo_-_2012.svg/120px-Windows_logo_-_2012.svg.png",
             "https://i.imgur.com/lbniOdx.png",
             "http://pngimg.com/upload/cat_PNG1631.png",
             "http://labs.sebastian-fuss.de/money/images/HighRes/euro2.jpg",
             "http://www.hivingroom.com/boutique1/45-153-thickbox/pomme-a-glacons-rouge.jpg",
             "http://www.zerodollartips.com/wp-content/uploads/2014/11/how-to-update-driver-software-on-windows-10-pc.jpg",
             "http://assets2.howtospendit.ft-static.com/images/43/66/16/43661634-4f60-4a91-a445-d19d6c340b66_three_eighty.png",
             "https://upload.wikimedia.org/wikipedia/commons/4/48/Xbox_icon.png"

            };
        }

        #region remote app service app properties
        private string _launchRemoteAppServiceResult;
        public string LaunchRemoteAppServiceResult
        {
            get { return _launchRemoteAppServiceResult; }
            set { SetProperty(ref _launchRemoteAppServiceResult, value); }
        }

        private List<string> _availableImages;

        public List<string> AvailableImages
        {
            get { return _availableImages; }
            set { SetProperty(ref _availableImages, value); }
        }


        #endregion
        #region launch uri properties

        private string _targetLaunchHost;
        public string TargetLaunchHost { get { return _targetLaunchHost; } set { SetProperty(ref _targetLaunchHost, value); } }


        private string _targetLaunchUri;
        public string TargetLaunchUri
        {
            get { return _targetLaunchUri; }
            set { SetProperty(ref _targetLaunchUri, value); }
        }

        private string _launchRemoteUriResult = string.Empty;
        public string LaunchRemoteUriResult
        {
            get { return _launchRemoteUriResult; }
            set { SetProperty(ref _launchRemoteUriResult, value); }
        }
        #endregion

        #region Listing properties

        private string _requestAccessError;
        public string RequestAccessError
        {
            get { return _requestAccessError; }
            set { SetProperty(ref _requestAccessError, value); }
        }

        private RemoteSystem _selectedRemoteSystem;
        public RemoteSystem SelectedRemoteSystem
        {
            get { return _selectedRemoteSystem; }
            set { SetProperty(ref _selectedRemoteSystem, value); }
        }

        private ObservableCollection<RemoteSystem> _remoteSystems;
        public ObservableCollection<RemoteSystem> RemoteSystems
        {
            get { return _remoteSystems; }
            set { SetProperty(ref _remoteSystems, value); }
        }

        private bool _isListingOnlyActiveDevices;
        public bool IsListingOnlyActiveDevices
        {
            get { return _isListingOnlyActiveDevices; }
            set
            {
                if (SetProperty(ref _isListingOnlyActiveDevices, value))
                {
                    ListDevicesAsync();
                }
            }
        }
        #endregion

        #region Listing
        private async Task ListDevicesAsync()
        {
            using (await _listDevicesAsyncLock.LockAsync())
            {
                RequestAccessError = string.Empty;
                var result = await RemoteSystem.RequestAccessAsync();
                if (result != RemoteSystemAccessStatus.Allowed)
                {
                    RequestAccessError = "Impossible de lister les périphériques : " + result;
                    return;
                }

                if (_watcher != null)
                {
                    _watcher.RemoteSystemAdded -= OnWatcherRemoteSystemAdded;
                    _watcher.RemoteSystemRemoved -= OnWatcherRemoteSystemRemoved;
                    _watcher.RemoteSystemUpdated -= OnWatcher_RemoteSystemUpdated;
                }

                RemoteSystems = new ObservableCollection<RemoteSystem>();

                if (IsListingOnlyActiveDevices)
                {
                    var filter = new IRemoteSystemFilter[]
                    {
                        new RemoteSystemStatusTypeFilter(RemoteSystemStatusType.Available),
                        new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.Any)
                    };
                    _watcher = RemoteSystem.CreateWatcher(filter);
                }
                else
                {
                    var watcher = RemoteSystem.CreateWatcher();
                    watcher.RemoteSystemAdded += OnWatcherRemoteSystemAdded;
                    watcher.RemoteSystemRemoved += OnWatcherRemoteSystemRemoved;
                }



                _watcher.RemoteSystemUpdated += OnWatcher_RemoteSystemUpdated;

                _watcher.Start();
            }
        }

        private void OnWatcher_RemoteSystemUpdated(RemoteSystemWatcher sender, RemoteSystemUpdatedEventArgs args)
        {
            int target = -1;
            for (int i = 0; i < RemoteSystems.Count; i++)
            {
                if (RemoteSystems[i].Id == args.RemoteSystem.Id)
                {
                    target = i;
                    break;
                }
            }

            if (target >= 0)
            {
                DispatcherHelper.UIDispatcher.RunIdleAsync(_ =>
                {
                    var original = RemoteSystems[target];
                    RemoteSystems[target] = args.RemoteSystem;
                    if (SelectedRemoteSystem == original)
                    {
                        SelectedRemoteSystem = args.RemoteSystem;
                    }
                });

            }
        }

        private void OnWatcherRemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
        {
            int target = -1;
            for (int i = 0; i < RemoteSystems.Count; i++)
            {
                if (RemoteSystems[i].Id == args.RemoteSystemId)
                {
                    target = i;
                    break;
                }
            }

            if (target >= 0)
            {
                DispatcherHelper.UIDispatcher.RunIdleAsync(_ =>
                {
                    RemoteSystems.RemoveAt(target);
                });
            }
        }

        private void OnWatcherRemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        {
            DispatcherHelper.UIDispatcher.RunIdleAsync(_ =>
            {
                RemoteSystems.Add(args.RemoteSystem);
            });
        }
        #endregion

        #region LaunchUri

        private readonly AsyncLock _launchUriAsyncLock = new AsyncLock();
        public async Task LaunchUriAsync()
        {
            using (await _launchUriAsyncLock.LockAsync())
            {
                LaunchRemoteUriResult = string.Empty;

                var target = SelectedRemoteSystem;
                if (target == null)
                {
                    LaunchRemoteUriResult = "Il faut sélectionner une cible...";
                    return;
                }

                if (!string.IsNullOrEmpty(TargetLaunchHost))
                {
                    // construct a HostName object
                    Windows.Networking.HostName deviceHost = new Windows.Networking.HostName(TargetLaunchHost);

                    // create a RemoteSystem object with the HostName
                    var remotesys = await RemoteSystem.FindByHostNameAsync(deviceHost);
                    if (remotesys != null)
                    {
                        LaunchRemoteUriResult = "Cible trouvée";

                        target = remotesys;
                    }
                }
                var targetRs = RemoteSystems
                    .FirstOrDefault(r => r.Status == RemoteSystemStatus.Available);
                var request = new RemoteSystemConnectionRequest(targetRs);
                var uri = new Uri(TargetLaunchUri);

                var launchUriTask = RemoteLauncher.LaunchUriAsync(request, uri);

                LaunchRemoteUriResult = "Appel en cours...";
                var timeout = Task.Delay(13000);
                await Task.WhenAny(timeout, launchUriTask.AsTask());

                if (timeout.IsCompleted)
                {
                    LaunchRemoteUriResult = "Timeout.... ";
                }
                else
                {
                    var result = await launchUriTask;

                    LaunchRemoteUriResult = "Appel effectué : " + result;
                }

            }
        }

        public async Task LaunchUriLocalAsync()
        {
            await Launcher.LaunchUriAsync(new Uri(TargetLaunchUri));
        }

        public void ResetLaunchUri()
        {
            TargetLaunchUri = "td2016whatsnew://snowfall";
        }

        #endregion

        #region Remote AppService
        private readonly AsyncLock _launchAppServiceAsyncLock = new AsyncLock();
        public async Task LauncAppServiceAsync(object sender, ItemClickEventArgs e)
        {
            using (await _launchAppServiceAsyncLock.LockAsync())
            {
                LaunchRemoteAppServiceResult = string.Empty;

                var target = SelectedRemoteSystem;
                if (target == null)
                {
                    LaunchRemoteAppServiceResult = "Il faut sélectionner une cible...";
                    return;
                }

                var connection = new AppServiceConnection
                {
                    AppServiceName = "com.infinitesquare.CustomRain",
                    PackageFamilyName = Package.Current.Id.FamilyName
                };

                // Create a remote system connection request for the given remote device
                var connectionRequest = new RemoteSystemConnectionRequest(target);

                // "open" the AppServiceConnection using the remote request
                var openRemoteTask = connection.OpenRemoteAsync(connectionRequest);

                LaunchRemoteAppServiceResult = "Connexion en cours...";
                var timeout = Task.Delay(13000);
                await Task.WhenAny(timeout, openRemoteTask.AsTask());

                if (timeout.IsCompleted)
                {
                    LaunchRemoteAppServiceResult = "Timeout...";
                    return;
                }

                var result = await openRemoteTask;
                if (result == AppServiceConnectionStatus.Success)
                {
                    LaunchRemoteAppServiceResult = "Connecté, envoi du message";

                    var inputs = new ValueSet();

                    // min_value and max_value vars are obtained somewhere else in the program
                    inputs.Add("targetUri", e.ClickedItem.ToString());

                    // send input and receive output in a variable
                    var response = await connection.SendMessageAsync(inputs);
                    if (response.Status != AppServiceResponseStatus.Success)
                    {
                        LaunchRemoteAppServiceResult = "Message envoyé ";
                    }
                    else
                    {
                        LaunchRemoteAppServiceResult = "Echec de l'envoi : " + result;
                    }
                }
                else
                {
                    LaunchRemoteAppServiceResult = "Echec de connexion : " + result;
                }
            }
        }
        #endregion
    }
}
