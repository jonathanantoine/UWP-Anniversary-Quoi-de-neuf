using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Lumia.Imaging;
using Lumia.Imaging.Adjustments;
using Lumia.Imaging.Artistic;

namespace UWPWhatsNewExtensionAppService
{
    public sealed class ImageSpecialEffectTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //Démarrage du traitement AppService
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (details == null)
            {
                return;
            }

            var def = taskInstance.GetDeferral();
            taskInstance.Canceled += (_, __) => { def.Complete(); };
            details.AppServiceConnection.RequestReceived += (_, message) =>
            {
                CreateImageEffectAsync(message, def);

            };
        }


        private static async Task CreateImageEffectAsync(AppServiceRequestReceivedEventArgs message, BackgroundTaskDeferral wholeTaskDeferral)
        {
            try
            {
                var messageDef = message.GetDeferral();
                try
                {
                    var targetFileToken = message.Request.Message["targetFileToken"].ToString();
                    var targetFile = await SharedStorageAccessManager.RedeemTokenForFileAsync(targetFileToken);
                    var outputFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Guid.NewGuid().ToString("N") + ".jpg");

                    //use the blureffect
                    using (var inputStream = await targetFile.OpenReadAsync())
                    {
                        var blureffect = new PosterizeEffect
                        {
                            ColorComponentValueCount = 12
                        };
                        inputStream.Seek(0);
                        blureffect.Source = new RandomAccessStreamImageSource(inputStream);


                        using (var jpegRenderer = new JpegRenderer(blureffect))
                        using (var stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            // Jpeg renderer gives the raw buffer that contains the filtered image.
                            IBuffer jpegBuffer = await jpegRenderer.RenderAsync();
                            await stream.WriteAsync(jpegBuffer);
                            await stream.FlushAsync();
                        }
                    }

                    var outputFileToken = SharedStorageAccessManager.AddFile(outputFile);
                    await message.Request.SendResponseAsync(new ValueSet { { "outputFileToken", outputFileToken } });

                }
                finally
                {
                    // fermeture du message
                    messageDef.Complete();
                }
            }
            finally
            {
                // On ne fera pas d'autres communications
                wholeTaskDeferral.Complete();
            }
        }
    }
}
