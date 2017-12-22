using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace GoogleVisionAPI_App
{
    public class ImageEntity : INotifyPropertyChanged
    {
        #region Property

        //private string _imageName;
        //private string _imagePath;
        //private Stream _data;
        //private Google.Apis.Storage.v1.Data.Object _imageObj;
        //private AnnotateImageRequest _annotateRequest;
        private string _imageText;

        public String ImageName
        {
            //get { return _imageName; }
            //set { _imageName = value; OnPropertyChanged("ImageName"); }
            get;
            set;
        }

        public String CleanImageName
        {
            get
            {
                return ImageName.Replace("Visualhunt.com/", "");
            }
        }

        public String ImagePath
        {
            //get { return _imagePath; }
            //set { _imagePath = value; OnPropertyChanged("ImagePath"); }
            get;
            set;
        }

        public Stream Data
        {
            //get { return _data; }
            //set { _data = value; OnPropertyChanged("Data"); }
            get;
            set;
        }


        public Google.Apis.Storage.v1.Data.Object ImageObj
        {
            //get { return _imageObj; }
            //set { _imageObj = value; OnPropertyChanged("ImageObj"); }
            get;
            set;
        }


        public AnnotateImageRequest AnnotateRequest
        {
            //get { return _annotateRequest; }
            //set { _annotateRequest = value; OnPropertyChanged("AnnotateRequest"); }
            get;
            set;
        }


        private double _luceneScore;
        public double LuceneScore
        {
            get { return _luceneScore; }
            set { _luceneScore = value; OnPropertyChanged("LuceneScore"); }
        }

        private string _luceneRanking;
        public string LuceneRanking
        {
            get { return _luceneRanking; }
            set { _luceneRanking = value; OnPropertyChanged("LuceneRanking"); }
        }


        public string ImageText
        {
            get
            {
                return _imageText;
            }
            set
            {
                _imageText = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ImageText");
            }
        }


        public int ProgressValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
