using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.RemoteSystems;
using UWPWhatsNew.Common;
using UWPWhatsNew.Common.AsyncHelpers;

namespace UWPWhatsNew.Views.ConnectedApps
{
    public class ConnectedAppViewModel : BindableBase
    {
        private string _requestAccessError;

        public string RequestAccessError
        {
            get { return _requestAccessError; } set { SetProperty(ref _requestAccessError, value); }
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

        private readonly AsyncLock _listDevicesAsyncLock = new AsyncLock();
        private RemoteSystemWatcher _watcher;

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

                RemoteSystems = new ObservableCollection<RemoteSystem>();

                if (IsListingOnlyActiveDevices)
                {
                    var filter = new[]
                    {
                        new RemoteSystemStatusTypeFilter(RemoteSystemStatusType.Available)
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
                RemoteSystems[target] = args.RemoteSystem;
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
                RemoteSystems.RemoveAt(target);
            }
        }

        private void
            OnWatcherRemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        {
            RemoteSystems.Add(args.RemoteSystem);
        }
    }
}
