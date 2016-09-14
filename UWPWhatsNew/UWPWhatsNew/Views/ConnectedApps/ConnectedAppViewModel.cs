using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.System.RemoteSystems;
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
        }

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
                        new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.Cloud),
                    };
                    _watcher = RemoteSystem.CreateWatcher(filter);
                }
                else
                {
                    _watcher = RemoteSystem.CreateWatcher();
                }

                _watcher.RemoteSystemAdded += OnWatcherRemoteSystemAdded;
                _watcher.RemoteSystemRemoved += OnWatcherRemoteSystemRemoved;
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

                var request = new RemoteSystemConnectionRequest(target);
                var options = new RemoteLauncherOptions
                {
                    FallbackUri = new Uri("http://infinitesquare.com"),
                    PreferredAppIds = { "a7d1054a-8dc3-47e8-8d51-90d70a9f5a2f_n6jrw4wtwxjjj" }
                };

                var launchUriTask = Windows.System.RemoteLauncher
                    .LaunchUriAsync(request, new Uri(TargetLaunchUri), options);

                LaunchRemoteUriResult = "Appel en cours...";
                var timeout = Task.Delay(7000);
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
            Launcher.LaunchUriAsync(new Uri(TargetLaunchUri));
        }

        public void ResetLaunchUri()
        {
            TargetLaunchUri = "td2016whatsnew://snowfall";
        }
    }
}
