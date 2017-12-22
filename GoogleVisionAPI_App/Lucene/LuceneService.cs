
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;

namespace GoogleVisionAPI_App.Lucene
{
    public interface ILuceneService
    {
        //void BuildIndex(IEnumerable<SampleDataFileRow> dataToIndex);
        //IEnumerable<SampleDataFileRow> Search(string searchTerm);
    }

    public class LuceneService : ILuceneService
    {

        public static string FileNameField = "filename";
        public static string ContentField = "content";
        public static string ImageContentField = "imagecontent";

        private static LuceneService _service;

        //Build single object
        public static LuceneService Context
        {
            get
            {
                if (_service == null)
                {
                    _service = new LuceneService();
                    _service.InitializeLucene();
                }
                return _service;
            }
        }


        // Note there are many different types of Analyzer that may be used with Lucene, the exact one you use
        // will depend on your requirements
        private Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
        private Directory luceneIndexDirectory;
        private IndexWriter writer;
        private string indexPath = System.AppDomain.CurrentDomain.BaseDirectory + "app_data\\Lucene";

        private LuceneService()
        {
            //InitializeLucene();
        }

        private void InitializeLucene()
        {
            if (System.IO.Directory.Exists(indexPath))
            {
                System.IO.Directory.Delete(indexPath, true);
            }

            luceneIndexDirectory = FSDirectory.Open(indexPath);
            writer = new IndexWriter(luceneIndexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public async Task AddIndexesAsync(string filename, List<ImageInfo> list, byte[] imageContent)
        {
            AddIndexes(filename, list, imageContent);
        }


        public void AddIndexes(string filename, List<ImageInfo> list, byte[] imageContent)
        {
            //var document = new Document();
            var luceneDoc = new LuceneDoc() { FileName = filename, ImageInfoList = list };
            
            var doc = luceneDoc.ToDocument();
            
            //document.Add(new Field(FileNameField, filename.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));           
            //document.Add(new Field(ImageContentField, System.Convert.ToBase64String(imageContent), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));

            writer.AddDocument(doc);

            writer.Commit();
            //writer.Flush(true, true, true);
        }

        public void Flush()
        {
            writer.Flush(true, true, true);
        }


        public IEnumerable<LuceneDoc> Search(string searchTerm)
        {
            IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);
            QueryParser parser = new QueryParser(Version.LUCENE_30, "ImageInfoList.Description", analyzer);
            TopFieldCollector fieldCollector = TopFieldCollector.Create(Sort.RELEVANCE, 1000, true, true, true, true);

            Query query = parser.Parse(searchTerm);
            searcher.Search(query, fieldCollector);

            var results = fieldCollector.TopDocs().ScoreDocs;
            List<string> fileNames = new List<string>();
            foreach (ScoreDoc scoreDoc in results)
            {
                var doc = searcher.Doc(scoreDoc.Doc);

                var luceneDoc = doc.ToObject<LuceneDoc>();
                luceneDoc.Score = scoreDoc.Score;
                yield return luceneDoc;
                //fileNames.Add(luceneDoc.FileName);
            }

            //return fileNames;
            //List<SampleDataFileRow> results = new List<SampleDataFileRow>();
            //SampleDataFileRow sampleDataFileRow = null;

            //for (int i = 0; i < hitsFound.Length(); i++)
            //{
            //    sampleDataFileRow = new SampleDataFileRow();
            //    Document doc = hitsFound.Doc(i);
            //    sampleDataFileRow.LineNumber = int.Parse(doc.Get("LineNumber"));
            //    sampleDataFileRow.LineText = doc.Get("LineText");
            //    float score = hitsFound.Score(i);
            //    sampleDataFileRow.Score = score;
            //    results.Add(sampleDataFileRow);
            //}

            //return results.OrderByDescending(x => x.Score).ToList();
            //return null;
        }



        //public IEnumerable<System.Tuple<string, string, string>> Search(string searchTerm, int test)
        //{
        //    IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);
        //    QueryParser parser = new QueryParser(Version.LUCENE_30, ContentField, analyzer);
        //    TopFieldCollector fieldCollector = TopFieldCollector.Create(Sort.RELEVANCE, 1000, true, false, false, false);

        //    Query query = parser.Parse(searchTerm);
        //    searcher.Search(query, fieldCollector);

        //    var results = fieldCollector.TopDocs();
        //    List<System.Tuple<string, string, string>> fileNames = new List<System.Tuple<string, string, string>>();
        //    foreach (ScoreDoc scoreDoc in results.ScoreDocs)
        //    {
        //        var doc = searcher.Doc(scoreDoc.Doc);
        //        fileNames.Add(new System.Tuple<string, string, string>(doc.Get(FileNameField), doc.Get(ContentField), doc.Get(ImageContentField)));
        //    }

        //    return fileNames;
        //    //List<SampleDataFileRow> results = new List<SampleDataFileRow>();
        //    //SampleDataFileRow sampleDataFileRow = null;

        //    //for (int i = 0; i < hitsFound.Length(); i++)
        //    //{
        //    //    sampleDataFileRow = new SampleDataFileRow();
        //    //    Document doc = hitsFound.Doc(i);
        //    //    sampleDataFileRow.LineNumber = int.Parse(doc.Get("LineNumber"));
        //    //    sampleDataFileRow.LineText = doc.Get("LineText");
        //    //    float score = hitsFound.Score(i);
        //    //    sampleDataFileRow.Score = score;
        //    //    results.Add(sampleDataFileRow);
        //    //}

        //    //return results.OrderByDescending(x => x.Score).ToList();
        //    //return null;
        //}
    }
}
