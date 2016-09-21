using System;
using System.Collections.Generic;
using UWPWhatsNew.Common;

namespace UWPWhatsNew.Views.Shell
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
                new AvailableItem<Type>("AccessText", typeof(AccessText.AccessTextPage),"/Assets/Images/IconAccessText.png"),
                new AvailableItem<Type>("Composition", typeof(Composition.CompositionPage),"/Assets/Images/IconGIF.png"),
                new AvailableItem<Type>("ConnectedAnimation", typeof(ConnectedAnimation.ConnectedAnimationPage),"/Assets/Images/IconAnimation.png"),
                new AvailableItem<Type>("Connected Apps", typeof(ConnectedApps.ConnectedAppPage),"/Assets/Images/IconConnected.png"),
                new AvailableItem<Type>("Single Process Model", typeof(ConnectedApps.ConnectedAppPage),"/Assets/Images/IconSingleProcess.png"),
                new AvailableItem<Type>("InkToolbar", typeof(InkToolbar.InkToolbarPage),"/Assets/Images/IconInk.png"),
                new AvailableItem<Type>("App Extensions", typeof(AppExtensions.AppExtensionsPage),"/Assets/Images/IconExtensibility.png")

            };
        }
    }
}
