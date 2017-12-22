using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace GoogleVisionAPI_App
{
    public class ImageEntityVm : ViewModelBase
    {
        #region Property

        private string _imageName;
        private string _imagePath;
        private string _imageContent;

        public String ImageName
        {
            get { return _imageName; }
            set
            {
                if (this._imageName != value)
                {
                    this._imageName = value;
                    base.NotifyChanged("ImageName");                    
                }
            }
        }
        

        public String CleanName
        {
            get { return ImageName.Replace("Visualhunt.com/", ""); }
        }

        public String ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (this._imagePath != value)
                {
                    this._imagePath = value;
                    base.NotifyChanged("ImagePath");
                }
            }
        }


        public String ImageContent
        {
            get { return _imageContent; }
            set
            {
                if (this._imageContent != value)
                {
                    this._imageContent = value;
                    base.NotifyChanged("ImageContent");
                }
            }
        }

        private BitmapImage _data;
        public BitmapImage Data
        {
            get { return _data; }
            set
            {
                if (this._data != value)
                {
                    this._data = value;
                    base.NotifyChanged("Data");
                }
            }
        }

        private Google.Apis.Storage.v1.Data.Object _imageObj;

        public Google.Apis.Storage.v1.Data.Object ImageObj
        {
            get { return _imageObj; }
            set
            {
                _imageObj = value;
                //StartLoadingImage(_imageObj);
            }
        }

        StorageClient storage = StorageClient.Create(GetGoogleCredential());
        public void StartLoadingImage()
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    this.ImageObj.MediaLink = ImageObj.MediaLink + "=s32-c";
                    this.ImageObj.SelfLink = ImageObj.SelfLink + "=s32-c";
                    //storage.Service.
                    storage.DownloadObject(ImageObj, stream);
                    stream?.Seek(0, SeekOrigin.Begin);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    //bitmap.DecodePixelHeight = 230;
                    bitmap.DecodePixelWidth = 230;
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; //BitmapCacheOption.OnLoad;
                    //bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    //No need to wait for this to finish
                    //ImageInfo(bitmap, obj);

                    Data = bitmap;


                }
                catch (Exception ex)
                {
                    Data = new BitmapImage();
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


        #endregion
    }
}
