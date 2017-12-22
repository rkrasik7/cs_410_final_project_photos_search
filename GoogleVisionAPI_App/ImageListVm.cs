using GoogleVisionAPI_App.Lucene;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVisionAPI_App
{
    public class ImageListVm : ViewModelBase
    {
        private string searchTerm;
        private RangeEnabledObservableCollection<ImageEntityVm> _listImageEntityObj;
        public RangeEnabledObservableCollection<ImageEntityVm> ListImageEntityObj
        {
            get { return _listImageEntityObj; }
            set
            {
                if (this._listImageEntityObj != value)
                {
                    this._listImageEntityObj = value;
                    base.NotifyChanged("ListImageEntityObj");
                }
            }
        }

        public ImageListVm()
        {
            SearchCommand = new ReactiveCommand<object, object>((x) => !IsBusy);
            UILoadedCommand = new ReactiveCommand<object, object>((x) => !IsBusy);
            ListImageEntityObj = new RangeEnabledObservableCollection<ImageEntityVm>();
            LoadImages();
            WireupViewModelStreams();
        }

        public ReactiveCommand<object, object> SearchCommand { get; private set; }
        public ReactiveCommand<object, object> UILoadedCommand { get; private set; }
        public bool IsBusy { get; set; }

        public async void LoadImages()
        {
            //object lockObj = new object();
            //object lockObj1 = new object();
            //BindingOperations.EnableCollectionSynchronization(ListImageEntityObj, lockObj);
            var list = await ImageView.GetAllImageData();
            ListImageEntityObj.InsertRange(list.Select(s => new ImageEntityVm { ImageName = s.ImageName, ImagePath = s.ImagePath, ImageObj = s.ImageObj }).ToList<ImageEntityVm>());
            //Parallel.ForEach(await ImageView.GetAllImageData(), (item) =>
            //{
            //    lock (lockObj1)
            //    {
            //        ListImageEntityObj.Add(item);
            //    }
            //});
            //= new ObservableCollection<ImageEntity>(await ImageView.GetAllImageData());
            //UILoadedCommand.Execute(null);
        }


        private void WireupViewModelStreams()
        {
            this.AddDisposable(this.SearchCommand.CommandExecutedStream.Subscribe(_ => DoLuceneSearch()));
            this.AddDisposable(this.UILoadedCommand.CommandExecutedStream.Subscribe(_ => UILoaded()));
        }

        private async void UILoaded()
        {
            Parallel.ForEach(ListImageEntityObj, item =>
            {
                item.StartLoadingImage();
            });
        }

        private void DoLuceneSearch()
        {
            var filteredList = new RangeEnabledObservableCollection<ImageEntityVm>();

            var list = LuceneService.Context.Search(SearchTerm);

            filteredList.InsertRange(ListImageEntityObj.Where(entity => list.Contains(entity.ImageName)));
            //lbImageGallery.DataContext = filteredList;
        }

        public string SearchTerm
        {
            get { return searchTerm; }
            set
            {
                if (this.searchTerm != value)
                {
                    this.searchTerm = value;
                    base.NotifyChanged("SearchTerm");
                }
            }
        }

    }
}
