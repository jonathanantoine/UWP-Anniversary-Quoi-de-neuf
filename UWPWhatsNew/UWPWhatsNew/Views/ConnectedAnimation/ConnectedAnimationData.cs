using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPWhatsNew.Views.ConnectedAnimation
{
    public static class ConnectedAnimationData
    {
        static ConnectedAnimationData()
        {
            AnimationIsEnabled = true;
        }

        public static bool AnimationIsEnabled { get; set; }

    }
}
