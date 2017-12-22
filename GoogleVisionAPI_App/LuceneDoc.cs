using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVisionAPI_App
{
    public class LuceneDoc
    {
        public string FileName { get; set; }
        public List<ImageInfo> ImageInfoList { get; set; }
        public double Score { get; set; }
    }
}
