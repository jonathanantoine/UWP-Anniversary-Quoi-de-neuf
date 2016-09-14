using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPWhatsNew.Common;

namespace UWPWhatsNew.Views.ConnectedAnimation
{
    public class ConnectedAnimationViewModel : BindableBase
    {
        public ConnectedAnimationViewModel()
        {
            Items = new ObservableCollection<Thumbnail>();
            Items.Add(new Thumbnail("Chat 1", PREFIX_URL + "1.jpg", "Un chat tout mignon"));
            Items.Add(new Thumbnail("Chat 2", PREFIX_URL + "2.jpg", "Un chat tout mignon"));
            Items.Add(new Thumbnail("Chat 3", PREFIX_URL + "3.jpg", "Un chat tout mignon"));
            Items.Add(new Thumbnail("Chat 4", PREFIX_URL + "4.jpg", "Un chat tout mignon"));
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

        public Thumbnail(string name, string url, string description)
        {
            Name = name;
            ImageUrl = url;
            Description = description;
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
