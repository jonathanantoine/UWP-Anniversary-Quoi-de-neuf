using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace UWPWhatsNew.Common
{
    public static class MessageService
    {
        public static void ClearAllPreviousToast()
        {
            ToastNotificationManager.History.Clear();
        }

        public static void DisplayToastMessage(string message)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            if (notifier.Setting != NotificationSetting.Enabled)
            {
                return;
            }


            var xmlContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            XmlNodeList toastTextAttributes = xmlContent.GetElementsByTagName("text");
            toastTextAttributes[0].InnerText = message;

            var notif = new ToastNotification(xmlContent);
            notifier.Show(notif);

        }
    }
}
