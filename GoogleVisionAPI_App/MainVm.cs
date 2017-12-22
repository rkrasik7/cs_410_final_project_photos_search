using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace GoogleVisionAPI_App
{
    public class MainVm : INotifyPropertyChanged
    {
        private RangeEnabledObservableCollection<ImageEntity> _listImageEntityObj;
        public RangeEnabledObservableCollection<ImageEntity> ListImageEntityObj
        {
            get { return _listImageEntityObj; }
            set { _listImageEntityObj = value; OnPropertyChanged("ListImageEntityObj"); }
        }

        private Int32 _progressValue;
        public Int32 ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropertyChanged("ProgressValue"); }
        }

        public MainVm()
        {            
            if (ListImageEntityObj == null)
            {
                //if (ListImageEntityObj.Count > 0)
                //{
                ListImageEntityObj = new RangeEnabledObservableCollection<ImageEntity>();
                ProgressValue = 0;
                //ListImageEntityObj = new ObservableCollection<ImageEntity>(ImageView.GetAllImageData().Result);
                //}
            }

            DoAsync(() => LoadImages());
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
            await ImageView.GetAllImageData(ListImageEntityObj, ProgressValue);
            //Parallel.ForEach(await ImageView.GetAllImageData(), (item) =>
            //{
            //    lock (lockObj1)
            //    {
            //        ListImageEntityObj.Add(item);
            //    }
            //});
            //= new ObservableCollection<ImageEntity>(await ImageView.GetAllImageData());
        }






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
    }
}
