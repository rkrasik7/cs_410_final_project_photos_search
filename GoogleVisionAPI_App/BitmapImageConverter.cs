using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using GoogleVisionAPI_App.Lucene;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;

namespace GoogleVisionAPI_App
{
    public class BitmapImageConverter : IValueConverter
    {
        private static BitmapImageVisionConverter visionConverter = new BitmapImageVisionConverter();
        StorageClient storage = StorageClient.Create(GetGoogleCredential());
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            object result = null;
            
            //DoAsync(() =>
            //{
                if (value is Google.Apis.Storage.v1.Data.Object)
                    result = GetBitmap((Google.Apis.Storage.v1.Data.Object)value);
            //});


            //return GetBitmap((MemoryStream)parameter);

            //if (value is string)
            //    return GetBitmap(new Uri((string)value, UriKind.Absolute));

            //if (value is Uri)
            //    return GetBitmap((Uri)value);
            return result;

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private void DoAsync(Action action)
        {
            var frame = new DispatcherFrame();
            new Thread((ThreadStart)(() =>
            {
                action();
                frame.Continue = false;
            })).Start();
            Dispatcher.PushFrame(frame);
        }


        public BitmapImage GetBitmap(Google.Apis.Storage.v1.Data.Object obj)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    //obj.MediaLink = obj.MediaLink + "=s32-c";
                    //obj.SelfLink = obj.SelfLink + "=s32-c";
                    //storage.Service.
                    storage.DownloadObject(obj, stream);
                    stream?.Seek(0, SeekOrigin.Begin);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.DecodePixelHeight = 230;
                    bitmap.DecodePixelWidth = 230;
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    //No need to wait for this to finish
                    //ImageInfo(bitmap, obj);

                    return bitmap;


                }catch(Exception ex)
                {
                    return new BitmapImage();
                }
            }
        }


        private static GoogleCredential GetGoogleCredential()
        {
            string filePath = "cool-photos-search-57852b03daf7.json";
            GoogleCredential credential = null;
            using (var jsonStream = new FileStream(filePath, FileMode.Open,
                FileAccess.Read, FileShare.Read))
            {
                credential = GoogleCredential.FromStream(jsonStream);
            }

            return credential;
        }

        public async Task ImageInfo(BitmapImage image, Google.Apis.Storage.v1.Data.Object obj)
        {
            //// Instantiates a client                
            //var client = ImageAnnotatorClient.Create();

            var name = obj.Name.Replace("Visualhunt.com/", "");

            var client = ImageAnnotatorClient.Create();

            AnnotateImageRequest request = new AnnotateImageRequest
            {
                Features =
                    {
                        new Feature { Type = Feature.Types.Type.LabelDetection },
                        new Feature { Type = Feature.Types.Type.WebDetection },
                        new Feature { Type = Feature.Types.Type.FaceDetection },
                        // By default, no limits are put on the number of results per annotation.
                        // Use the MaxResults property to specify a limit.
                        new Feature { Type = Feature.Types.Type.LandmarkDetection, MaxResults = 5 },
                        new Feature { Type = Feature.Types.Type.TextDetection },
                        new Feature { Type = Feature.Types.Type.ImageProperties, MaxResults = 5  },
                    }
            };

            var img = new Image();
            //image.StreamSource.Seek(0, SeekOrigin.Begin);
            var memstream = image.StreamSource as MemoryStream;
            img.Content = Google.Protobuf.ByteString.FromStream(new MemoryStream(memstream.ToArray()));

            request.Image = img;


            var result = client.Annotate(request);
            
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append(",");
            if (result.LabelAnnotations != null)
                stringBuilder.AppendLine(String.Join(", ", result.LabelAnnotations.Select(s => $"{s.Description}")));


            stringBuilder.AppendLine();
            if (result.WebDetection != null && result.WebDetection.WebEntities != null)
                stringBuilder.AppendLine(String.Join(", ", result.WebDetection.WebEntities.Select(s => $"{s.Description}")));


            stringBuilder.AppendLine();
            if (result.LandmarkAnnotations != null)
                stringBuilder.AppendLine(String.Join(", ", result.LandmarkAnnotations.Select(s => $"{s.Description}")));


            stringBuilder.AppendLine();
            if (result.TextAnnotations != null)
                stringBuilder.AppendLine(String.Join(", ", result.TextAnnotations.Select(s => $"{s.Description}")));

            stringBuilder.AppendLine();
            if (result.ImagePropertiesAnnotation != null && result.ImagePropertiesAnnotation.DominantColors != null
                && result.ImagePropertiesAnnotation.DominantColors.Colors != null)
                stringBuilder.AppendLine(String.Join(", ", result.ImagePropertiesAnnotation.DominantColors.Colors.Select(s => {
                    System.Drawing.Color thisColor = System.Drawing.Color.FromArgb(
                                                                            int.Parse(s.Color.Alpha?.ToString() ?? "0"),
                                                                            int.Parse(s.Color.Red.ToString()),
                                                                            int.Parse(s.Color.Green.ToString()),
                                                                            int.Parse(s.Color.Blue.ToString())
                                                                            );                    
                    return $"{HexConverter(thisColor)}";
                })));

            
            //LuceneService.Context.AddIndexesAsync(name, stringBuilder.ToString(), memstream.ToArray());
        }

        private String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
