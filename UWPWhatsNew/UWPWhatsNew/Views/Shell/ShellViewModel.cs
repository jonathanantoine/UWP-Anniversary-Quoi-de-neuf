using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPWhatsNew.Common;
using UWPWhatsNew.Views.Shell;

namespace UWPWhatsNew.Views
{
    public class ShellViewModel : BindableBase
    {
        private List<AvailableItem<Type>> _availableSamples;
        public List<AvailableItem<Type>> AvailableSamples
        {
            get { return _availableSamples; }
            set { SetProperty(ref _availableSamples, value); }
        }

        public ShellViewModel()
        {
            AvailableSamples = new List<AvailableItem<Type>>
            {
                new AvailableItem<Type>("Animated GIF", typeof(AnimatedGif.AnimatedGifPage),"/Assets/Images/IconGIF.png"),
                new AvailableItem<Type>("Connected Apps", typeof(ConnectedApps.ConnectedAppPage),"/Assets/Images/IconConnected.png"),
                new AvailableItem<Type>("Connected Apps", typeof(ConnectedApps.ConnectedAppPage),"/Assets/Images/IconSingleProcess.png"),
                new AvailableItem<Type>("Connected Apps", typeof(ConnectedApps.ConnectedAppPage),"/Assets/Images/IconInk.png")

            };
        }
    }
}
