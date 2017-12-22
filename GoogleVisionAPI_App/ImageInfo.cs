using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVisionAPI_App
{

    public enum AnnotationCategory
    {
        LabelAnnotations,
        WebEntities,
        LandmarkAnnotations,
        TextAnnotations,
    }

    public class ImageInfo
    {
        public string Description { get; set; }
        public double Score { get; set; }
        public AnnotationCategory AnnotationCategory { get; set; }

        public override string ToString()
        {
            return $"{Description} [{Math.Round(Score * 100, 0)}%]";
        }
    }
}
