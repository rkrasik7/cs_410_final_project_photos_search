using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GoogleVisionAPI_App
{
    public class BitmapImageVisionConverter : IValueConverter
    {
        static GoogleCredential credential = GetGoogleCredential();
        StorageClient storage = StorageClient.Create(credential);
        UrlSigner urlSigner = UrlSigner.FromServiceAccountCredential(credential.UnderlyingCredential as ServiceAccountCredential);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;

            DoAsync(() =>
            {
                if (value is Google.Apis.Storage.v1.Data.Object/*BitmapImage*/)
                    result = GetBitmapInfo(/*(BitmapImage)*/(Google.Apis.Storage.v1.Data.Object)value);
            });

            //if (value is Google.Apis.Storage.v1.Data.Object/*BitmapImage*/)
            //    return GetBitmapInfo(/*(BitmapImage)*/(Google.Apis.Storage.v1.Data.Object)value);
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



        public string GetBitmapInfo(/*BitmapImage image*/Google.Apis.Storage.v1.Data.Object obj)
        {
            using (var stream = new MemoryStream())
            {
                //obj.MediaLink = obj.MediaLink + "=s32-c";
                //obj.SelfLink = obj.SelfLink + "=s32-c";

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


                ////string url = urlSigner.Sign(
                ////    "sample_photos_2017",
                ////    obj.Name,
                ////    TimeSpan.FromMinutes(5),
                ////    System.Net.Http.HttpMethod.Get);


                // Instantiates a client                
                var client = ImageAnnotatorClient.Create();

                AnnotateImageRequest request = new AnnotateImageRequest
                {
                    //Image = Google.Cloud.Vision.V1.Image.FromUri(url),
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


                // Load the image file into memory
                //var image = bitmap;
                // Performs label detection on the image file
                var img = new Image();
                //image.StreamSource.Seek(0, SeekOrigin.Begin);
                var memstream = bitmap.StreamSource as MemoryStream;
                img.Content = Google.Protobuf.ByteString.FromStream(new MemoryStream(memstream.ToArray()));

                request.Image = img;

                var result = client.Annotate(request);

                
                //var labels = client.DetectLabels(img);
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Label Annotations:");

                //stringBuilder.AppendLine(String.Join(",", labels.Select(s => s.Description)));
                if(result.LabelAnnotations != null)
                    stringBuilder.AppendLine(String.Join(", ", result.LabelAnnotations.Select(s => $"{s.Description} [{Math.Round(s.Score * 100,0)}%]")));


                //stringBuilder.AppendLine();
                //stringBuilder.AppendLine("Web Info:");
                ////var webinfos = client.DetectWebInformation(img);
                ////stringBuilder.AppendLine(String.Join(",", webinfos.WebEntities.Select(s => s.Description)));
                //if (result.WebDetection != null && result.WebDetection.WebEntities != null)
                //    stringBuilder.AppendLine(String.Join(", ", result.WebDetection.WebEntities.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));


                //stringBuilder.AppendLine();
                //stringBuilder.AppendLine("LandMark:");
                ////var landmarks = client.DetectLandmarks(img);
                ////stringBuilder.AppendLine(String.Join(",", landmarks.Select(s => s.Description)));
                //if (result.LandmarkAnnotations != null)
                //    stringBuilder.AppendLine(String.Join(", ", result.LandmarkAnnotations.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));


                //stringBuilder.AppendLine();
                //stringBuilder.AppendLine("Text:");
                ////var texts = client.DetectText(img);
                ////stringBuilder.AppendLine(String.Join(",", texts.Select(s => s.Description)));
                //if (result.TextAnnotations != null)
                //    stringBuilder.AppendLine(String.Join(", ", result.TextAnnotations.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));

                //stringBuilder.AppendLine();
                //stringBuilder.AppendLine("Color:");

                ////var properties = client.DetectImageProperties(img);
                ////stringBuilder.AppendLine(String.Join(",", properties.DominantColors.Colors.Select(s =>
                ////{
                ////    System.Drawing.Color thisColor = System.Drawing.Color.FromArgb(
                ////                                                                int.Parse(s.Color.Alpha?.ToString() ?? "0"),
                ////                                                                int.Parse(s.Color.Red.ToString()),
                ////                                                                int.Parse(s.Color.Green.ToString()),
                ////                                                                int.Parse(s.Color.Blue.ToString())
                ////                                                                );
                ////    return HexConverter(thisColor);

                ////})));
                
                //if (result.ImagePropertiesAnnotation != null && result.ImagePropertiesAnnotation.DominantColors != null 
                //    && result.ImagePropertiesAnnotation.DominantColors.Colors != null)
                //    stringBuilder.AppendLine(String.Join(", ", result.ImagePropertiesAnnotation.DominantColors.Colors.Select(s => {
                //        System.Drawing.Color thisColor = System.Drawing.Color.FromArgb(
                //                                                                int.Parse(s.Color.Alpha?.ToString() ?? "0"),
                //                                                                int.Parse(s.Color.Red.ToString()),
                //                                                                int.Parse(s.Color.Green.ToString()),
                //                                                                int.Parse(s.Color.Blue.ToString())
                //                                                                );
                //             ;
                //        return $"{HexConverter(thisColor)} [{Math.Round(s.Score * 100, 0)}%]";
                //    })));



                return stringBuilder.ToString();
            }
        }


        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }


        private static string BuildString(EntityAnnotation entity)
        {
            return $"{entity.Description} [{entity.Score * 100}]";
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
    }
}
