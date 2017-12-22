using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Async;
using System.Windows.Controls;
using System.Windows.Threading;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Vision.V1;
using System.Threading;
using GoogleVisionAPI_App.Lucene;

namespace GoogleVisionAPI_App
{
    public static class ImageView
    {
        #region Property
        //private static long ? _ImageCount;
        //public static long ? ImageCount
        //{
        //    get 
        //    {
        //        try
        //        {
        //            XDocument XDoc = XDocument.Load("ImageData.xml");

        //            _ImageCount = XDoc.Descendants("Image").Count();

        //            return _ImageCount;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(ex.Message); 
        //        }
        //    }
        //}
        #endregion

        private static ObservableCollection<ImageEntity> _collection;
        private static ProgressBar _progressBar;
        #region Methods
        /// <summary>
        /// Get Image From XML Data
        /// </summary>
        /// <returns></returns>
        public async static Task<List<ImageEntity>> GetAllImageData(ObservableCollection<ImageEntity> collection, ProgressBar progressBar)
        {
            try
            {
                _progressBar = progressBar;
                //assign a reference object for accessing in this class
                _collection = collection;
                // Load  XML Document
                //XDocument XDoc = XDocument.Load("ImageData.xml");

                // Query for retriving all Images data from XML
                // Using Linq Query
                //var Query = (from Q in XDoc.Descendants("Image")
                //            select new ImageEntity
                //            {
                //                ImageName=Q.Element("ImageName").Value,
                //                ImagePath=Q.Element("ImagePath").Value
                //            }).ToList<ImageEntity>();

                // Using Lambda Expression
                //var Query = XDoc.Descendants("Image").Select(LE => new ImageEntity() 
                //{ ImageName=LE.Element("ImageName").Value, ImagePath=LE.Element("ImagePath").Value }).ToList<ImageEntity>();
                var items = await GetBucket("cool-photos-search");
                //return Query;
                return items;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        static object lockObj1 = new object();

        public async static Task<List<ImageEntity>> GetBucket(string projectId)
        {
            var images = new List<ImageEntity>();
            // Explicitly use service account credentials by specifying the private key
            // file.
            GoogleCredential credential = GetGoogleCredential();
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var itemsList = storage.ListObjectsAsync("sample_photos_2017");

            //for (int i = 0; i < 1000; i++)
            //{

            //int i = 0;
            //while (i <= await itemsList.Count())
            //{
            //    var firstBatch = itemsList.Skip(i).Take(10);
            //DoAsync(async () =>
            //{
            int count = 0;
            var total = itemsList.Count().Result;
            var client = ImageAnnotatorClient.Create();
            var skip = 0;
            var page = 10;
            var requestCount = 0;
            // use (ca) half of the kernels:
            int degreeOfParallelism = Environment.ProcessorCount > 1 ?
                      Environment.ProcessorCount / 2 : 1;
            //var dateTimeStarted = DateTime.Now;
            while (skip < total)
            {

                //make the thread sleep when process 600 request because only 600 requests per minute are allowed.
                if (requestCount == 600)
                {
                    requestCount = 0;
                    System.Threading.Thread.Sleep(40000);
                }

                //we have to use localskip because of async threads
                var localskip = skip;
                skip = skip + page;
                requestCount = requestCount + page;

                await itemsList.Skip(localskip).Take(page).ForEachAsync(item =>
                {
                    var image = new ImageEntity() { ImageName = item.Name, ImagePath = item.SelfLink, ImageObj = item };
                    lock (lockObj1)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            _collection.Add(image);
                            //image.AnnotateRequest = CreateRequet(image.ImageObj);
                            //images.Add(image);
                            //image.AnnotateRequest = CreateRequet(image.ImageObj);
                            //image.ImageText = GetInfo(client.Annotate(image.AnnotateRequest));
                            //_progressBar.Value = (skip / total) * 100; //Int64.Parse($"{count++ / total}");
                        }, DispatcherPriority.Background);
                    }
                });

                var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = degreeOfParallelism };
                Parallel.ForEach(_collection.Skip(localskip).Take(page), parallelOptions, async (item) =>
                //foreach (var item in _collection)
                {
                    //Tuple<string, string> info = null;
                    List<ImageInfo> list = null;
                    try
                    {

                        item.AnnotateRequest = CreateRequet(item.ImageObj);
                        var result = await client.AnnotateAsync(item.AnnotateRequest);
                        list = GetInfo(result, item.CleanImageName);
                        var builder = new StringBuilder();
                        //builder.AppendLine("Google Vision Labels:");
                        builder.AppendLine(string.Join(", ", list.Where(s =>
                                                                            s.AnnotationCategory == AnnotationCategory.LabelAnnotations)
                                                                    .Select(s => s.ToString())));
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            item.ImageText = builder.ToString(); //info.Item1;
                        }, DispatcherPriority.Background);
                    }
                    catch
                    {
                        //ignore it as we can't handle it.
                    }
                    if (list != null)
                        await LuceneService.Context.AddIndexesAsync(item.CleanImageName, list/*info.Item2*/, null);

                });

                LuceneService.Context.Flush();
                App.Current.Dispatcher.Invoke(() =>
                {

                    _progressBar.Value = ((double)skip / (double)total) * 100;

                }, DispatcherPriority.Background);
            }
            //});

            //images.All(s => { s.AnnotateRequest = CreateRequet(s.ImageObj); return true; });


            //int i =0;
            //while (i < total)
            //{
            //    BatchAnnotateImagesRequest batch = new BatchAnnotateImagesRequest();
            //    batch.Requests.AddRange(images.Skip(i).Take(15).Select(s => s.AnnotateRequest));

            //    var client = ImageAnnotatorClient.Create();
            //    var result = client.BatchAnnotateImages(batch);

            //    i = i + 15;
            //}

            // i += 10;
            //}
            //}
            return images;
        }


        //private static void DoAsync(Action action)
        //{
        //    var frame = new DispatcherFrame();
        //    new Thread((ThreadStart)(() =>
        //    {
        //        action();
        //        frame.Continue = false;
        //    })).Start();
        //    Dispatcher.PushFrame(frame);
        //}

        private static AnnotateImageRequest CreateRequet(Google.Apis.Storage.v1.Data.Object imageObj)
        {
            UrlSigner urlSigner = UrlSigner.FromServiceAccountCredential(GetGoogleCredential().UnderlyingCredential as ServiceAccountCredential);
            string url = urlSigner.Sign(
                "sample_photos_2017",
                imageObj.Name,
                TimeSpan.FromMinutes(5),
                System.Net.Http.HttpMethod.Get);

            AnnotateImageRequest request = new AnnotateImageRequest
            {
                Image = Google.Cloud.Vision.V1.Image.FromUri(url),
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

            return request;
        }


        private static List<ImageInfo> GetInfo(AnnotateImageResponse result, string name)
        {            
            var infoList = new List<ImageInfo>();

            //var labels = client.DetectLabels(img);
            //var stringBuilderIndex = new StringBuilder();
            //var stringBuilderDisplay = new StringBuilder();


            //stringBuilderIndex.AppendLine(name);
            //stringBuilderDisplay.AppendLine(name);

            //stringBuilderDisplay.AppendLine("Google Vision Labels:");

            //stringBuilder.AppendLine(String.Join(",", labels.Select(s => s.Description)));
            if (result.LabelAnnotations != null)
            {

                infoList.AddRange(result.LabelAnnotations
                                    .Select(s =>
                                                new ImageInfo()
                                                {
                                                    Description = s.Description,
                                                    Score = s.Score,
                                                    AnnotationCategory = AnnotationCategory.LabelAnnotations
                                                }
                                        )
                                 );

                //stringBuilderIndex.AppendLine(String.Join(", ", result.LabelAnnotations.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));
                //stringBuilderDisplay.AppendLine(String.Join(", ", result.LabelAnnotations.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));
            }

            //stringBuilderIndex.AppendLine();
            //stringBuilderIndex.AppendLine("Web Info:");
            //var webinfos = client.DetectWebInformation(img);
            //stringBuilder.AppendLine(String.Join(",", webinfos.WebEntities.Select(s => s.Description)));
            if (result.WebDetection != null && result.WebDetection.WebEntities != null)
            {
                infoList.AddRange(result.WebDetection.WebEntities
                                    .Select(s =>
                                                new ImageInfo()
                                                {
                                                    Description = s.Description,
                                                    Score = s.Score,
                                                    AnnotationCategory = AnnotationCategory.WebEntities
                                                }
                                        )
                                 );
                //stringBuilderIndex.AppendLine(String.Join(", ", result.WebDetection.WebEntities.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));
            }


            //stringBuilderIndex.AppendLine();
            //stringBuilderIndex.AppendLine("LandMark:");
            //var landmarks = client.DetectLandmarks(img);
            //stringBuilder.AppendLine(String.Join(",", landmarks.Select(s => s.Description)));
            if (result.LandmarkAnnotations != null)
            {
                infoList.AddRange(result.LandmarkAnnotations
                                    .Select(s =>
                                                new ImageInfo()
                                                {
                                                    Description = s.Description,
                                                    Score = s.Score,
                                                    AnnotationCategory = AnnotationCategory.LandmarkAnnotations
                                                }
                                        )
                                 );

                //stringBuilderIndex.AppendLine(String.Join(", ", result.LandmarkAnnotations.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));
            }

            //stringBuilderIndex.AppendLine();
            //stringBuilderIndex.AppendLine("Text:");
            //var texts = client.DetectText(img);
            //stringBuilder.AppendLine(String.Join(",", texts.Select(s => s.Description)));
            if (result.TextAnnotations != null)
            {
                infoList.AddRange(result.TextAnnotations
                                    .Select(s =>
                                                new ImageInfo()
                                                {
                                                    Description = s.Description,
                                                    Score = s.Score,
                                                    AnnotationCategory = AnnotationCategory.TextAnnotations
                                                }
                                        )
                                 );
                //stringBuilderIndex.AppendLine(String.Join(", ", result.TextAnnotations.Select(s => $"{s.Description} [{Math.Round(s.Score * 100, 0)}%]")));
            }
            //stringBuilderIndex.AppendLine();
            //stringBuilderIndex.AppendLine("Color:");

            //var properties = client.DetectImageProperties(img);
            //stringBuilder.AppendLine(String.Join(",", properties.DominantColors.Colors.Select(s =>
            //{
            //    System.Drawing.Color thisColor = System.Drawing.Color.FromArgb(
            //                                                                int.Parse(s.Color.Alpha?.ToString() ?? "0"),
            //                                                                int.Parse(s.Color.Red.ToString()),
            //                                                                int.Parse(s.Color.Green.ToString()),
            //                                                                int.Parse(s.Color.Blue.ToString())
            //                                                                );
            //    return HexConverter(thisColor);

            //})));

            //if (result.ImagePropertiesAnnotation != null && result.ImagePropertiesAnnotation.DominantColors != null
            //    && result.ImagePropertiesAnnotation.DominantColors.Colors != null)
            //    stringBuilder.AppendLine(String.Join(", ", result.ImagePropertiesAnnotation.DominantColors.Colors.Select(s =>
            //    {
            //        System.Drawing.Color thisColor = System.Drawing.Color.FromArgb(
            //                                                                int.Parse(s.Color.Alpha?.ToString() ?? "0"),
            //                                                                int.Parse(s.Color.Red.ToString()),
            //                                                                int.Parse(s.Color.Green.ToString()),
            //                                                                int.Parse(s.Color.Blue.ToString())
            //                                                                );
            //        ;
            //        return $"{HexConverter(thisColor)} [{Math.Round(s.Score * 100, 0)}%]";
            //    })));



            //return stringBuilder.ToString();
            return infoList;//new Tuple<string, string>(stringBuilderDisplay.ToString(), stringBuilderIndex.ToString());
        }


        //private static void AddAnnotation(List<ImageInfo> list, EntityAnnotation annotation, AnnotationCategory category)
        //{

        //}


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

        #endregion
    }
}
