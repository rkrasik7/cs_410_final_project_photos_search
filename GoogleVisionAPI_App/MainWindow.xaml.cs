using GoogleVisionAPI_App.Lucene;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GoogleVisionAPI_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ImageEntity> ListImageEntityObj;        
        public MainWindow()
        {
            InitializeComponent();


            ListImageEntityObj = new RangeEnabledObservableCollection<ImageEntity>();

            this.Loaded += (s, o) =>
            {
                if (ListImageEntityObj != null)
                {
                    //if (ListImageEntityObj.Count > 0)
                    //{
                    lbImageGallery.DataContext = ListImageEntityObj;
                    //ListImageEntityObj = new ObservableCollection<ImageEntity>(ImageView.GetAllImageData().Result);
                    //}
                }

                DoAsync(() => LoadImages());                
            };
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


        public async void LoadImages()
        {
            object lockObj = new object();
            //object lockObj1 = new object();
            BindingOperations.EnableCollectionSynchronization(ListImageEntityObj, lockObj);
            await ImageView.GetAllImageData(ListImageEntityObj, pbStatus);
            //Parallel.ForEach(await ImageView.GetAllImageData(), (item) =>
            //{
            //    lock (lockObj1)
            //    {
            //        ListImageEntityObj.Add(item);
            //    }
            //});
             //= new ObservableCollection<ImageEntity>(await ImageView.GetAllImageData());
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var filteredList = new RangeEnabledObservableCollection<ImageEntity>();

            var list = LuceneService.Context.Search(SearchBox.Text);

            var listDic = list.ToDictionary(s => s.FileName) as Dictionary<string, LuceneDoc>;


            filteredList.InsertRange(ListImageEntityObj.Where(entity => listDic.ContainsKey(entity.CleanImageName))
                                                       .Select(s => 
                                                                {
                                                                    var item = listDic[s.CleanImageName];
                                                                    s.LuceneScore = item.Score;
                                                                    s.LuceneRanking = GetRanking(item.Score);
                                                                    return s;
                                                                })
                                                       .OrderByDescending(s => s.LuceneScore));
            lbImageGallery.DataContext = filteredList;
        }

        private void SearchClear_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            lbImageGallery.DataContext = ListImageEntityObj;
            //Remove ranking label
            ListImageEntityObj.Where(s => !String.IsNullOrEmpty(s.LuceneRanking)).Select(s => { s.LuceneRanking = string.Empty; s.LuceneScore = 0; return s; });
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SearchButton_Click(sender, null);
            }
        }


        private string GetRanking(double score)
        {
            var rank = Math.Round(score * 100, 0);
            return $"Lucene ranking: [{(rank > 100 ? 100 : rank)}%]";
        }



        //#region Uniform Grid Auto Column Adjust
        //public int TileColumns
        //{
        //    get { return (int)GetValue(TileColumnsProperty); }
        //    set { SetValue(TileColumnsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for TileColumns.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TileColumnsProperty =
        //    DependencyProperty.Register("TileColumns", typeof(int), typeof(ListBox), new PropertyMetadata(3));

        ////private void scroll_SizeChanged(object sender, SizeChangedEventArgs e)
        ////{
        ////    var aw = scroll.ActualWidth;
        ////    TileColumns = (int)aw / 154; // 154 is a Tile's width
        ////}



        //private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var aw = AppMainWindow.ActualWidth;
        //    TileColumns = (int)aw / 154; // 154 is a Tile's width
        //}

        //#endregion
    }
}
