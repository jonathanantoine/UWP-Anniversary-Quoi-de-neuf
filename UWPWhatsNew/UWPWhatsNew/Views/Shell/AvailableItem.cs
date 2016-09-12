using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPWhatsNew.Views.Shell
{
    public class AvailableItem<T> 
    {
        public AvailableItem(string label, T value,string extra ="")
        {
            Label = label;
            Value = value;
            Extra = extra;
        }

        public string Extra { get; set; }
        public string Label { get; set; }
        public T Value { get; set; }
    }
}
