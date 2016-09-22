using System;
using System.Collections.ObjectModel;
using UWPWhatsNew.Common;

namespace UWPWhatsNew.Views.ConnectedAnimation
{
    public class ConnectedAnimationViewModel : BindableBase
    {
        public ConnectedAnimationViewModel()
        {
            Items = new ObservableCollection<Thumbnail>();

            Items.Add(new Thumbnail("Surface Pro 4", PREFIX_URL + "s1.jpg"));
            Items.Add(new Thumbnail("Ninja cat", PREFIX_URL + "ninjacat1.png"));
            Items.Add(new Thumbnail("Licorne multi color", PREFIX_URL + "u1.png"));
            Items.Add(new Thumbnail("Lumnia 1520", PREFIX_URL + "wp1.png"));
            Items.Add(new Thumbnail("Chat mignon", PREFIX_URL + "c1.png"));
            Items.Add(new Thumbnail("Visual studio", PREFIX_URL + "v1.png"));
            Items.Add(new Thumbnail("Surface Book", PREFIX_URL + "s2.jpg"));
            Items.Add(new Thumbnail("Licorne rose", PREFIX_URL + "u2.jpg"));
            Items.Add(new Thumbnail("Lumnia 950", PREFIX_URL + "wp2.jpg"));
            Items.Add(new Thumbnail("Hololens", PREFIX_URL + "s3.png"));
            Items.Add(new Thumbnail("Chat 2", PREFIX_URL + "c2.png"));
            Items.Add(new Thumbnail("Xamarin", PREFIX_URL + "v2.png"));
            Items.Add(new Thumbnail("Windows Phone Archos", PREFIX_URL + "wp3.png"));
            Items.Add(new Thumbnail("Licorne endormie", PREFIX_URL + "u3.png"));
            Items.Add(new Thumbnail("MS Band", PREFIX_URL + "s4.png"));
            Items.Add(new Thumbnail("Xbox One", PREFIX_URL + "s5.png"));
            Items.Add(new Thumbnail("Lumnia 930", PREFIX_URL + "wp4.png"));
            Items.Add(new Thumbnail("Notepad++", PREFIX_URL + "v3.jpg"));
            Items.Add(new Thumbnail("MS", PREFIX_URL + "ms.jpg"));
            Items.Add(new Thumbnail("Ninja cat", PREFIX_URL + "windowsCat.png"));
            Items.Add(new Thumbnail("Ninja cat", PREFIX_URL + "ninjacat2.jpg"));

        }

        public ObservableCollection<Thumbnail> Items
        {
            get; set;
        }

        private static readonly string PREFIX_URL = "ms-appx:///Assets/Images/ConnectedAnimation/";
    }

    public class Thumbnail
    {
        public Thumbnail()
        {

        }

        public Thumbnail(string name, string url)
        {
            Name = name;
            ImageUrl = url;
        }

        public string Name
        {
            get; set;
        }

        public string ImageUrl
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }
    }

}
