using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace YiTao.Web.Common
{
    /// <summary>
    /// 全文检索
    /// </summary>
    public class Searcher
    {
        private static string indexPath = @"\Lucene";
        /// <summary>
        /// 创建索引
        /// 如已存在则会先删除在添加
        /// </summary>
        /// <param name="shangPin">商品类</param>
        public static void Add(ShangPin shangPin)
        {
            //索引的文件存放的路径        
            //FSDirectory是用于对文件系统目录的操作的类
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
            //检查目录是否存在
            bool isUpdate = IndexReader.IndexExists(directory);
            if (isUpdate)
            {
                //目录存在则判断目录是否被锁定，被锁定就解锁
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }

            //IndexWriter主要用于写索引
            //方法签名：public IndexWriter(Directory d,Analyzer a,boolean create,IndexWriter.MaxFieldLength mfl)
            //第一个参数是 (Directory d)：索引的目录(前面的FSDirectory类的对象)
            //第二个参数是 (Analyzer a)：分析器(这里我们用盘古分词的分析器)
            //第三个参数是 (boolean create)：是否创建目录
            //第四个参数是 (IndexWriter.MaxFieldLength):最大长度
            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isUpdate, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.DeleteDocuments(new Term("ShangPinIndex", shangPin.ShangPinIndex));
            //Document文档对象
            Document document = new Document();
            //将数据中的内容写入索引
            document.Add(new Field("ShangPinIndex", shangPin.ShangPinIndex, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("ShangPinId", shangPin.ShangPinId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Type", shangPin.Type.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Name", shangPin.Name.ToLower(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            writer.AddDocument(document);
            //要记得关闭
            writer.Dispose();
            directory.Dispose();
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="shangPin">商品类</param>
        public static void Del(ShangPin shangPin)
        {
            //索引的文件存放的路径        
            //FSDirectory是用于对文件系统目录的操作的类
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
            //检查目录是否存在
            bool isUpdate = IndexReader.IndexExists(directory);
            if (isUpdate)
            {
                //目录存在则判断目录是否被锁定，被锁定就解锁
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }

            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isUpdate, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.DeleteDocuments(new Term("ShangPinIndex", shangPin.ShangPinIndex));            
            //要记得关闭
            writer.Dispose();
            directory.Dispose();
        }

        /// <summary>
        /// 搜索方法
        /// </summary>
        /// <param name="name">商品名</param>
        /// <returns>商品列表</returns>
        public static List<ShangPin> Search(string name)
        {
            List<ShangPin> list = new List<ShangPin>();
            name = name.ToLower();
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            PanGuAnalyzer analyzer = new PanGuAnalyzer();
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Name", analyzer);
            Query query = parser.Parse(name.ToLower());
            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.Create(1000, true);
            searcher.Search(query, null, collector);//根据query查询条件进行查询，查询结果放入collector容器
            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.TopDocs(0, collector.TotalHits).ScoreDocs;
            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].Doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document               
                ShangPin shangPin = new ShangPin ();
                shangPin.ShangPinId = Convert.ToInt32(doc.Get("ShangPinId"));
                shangPin.Type = (EnumShangPinType)Enum.Parse(typeof(EnumShangPinType), doc.Get("Type"));
                shangPin.Name = doc.Get("Name");
                list.Add(shangPin); //获得文档对象指定内容
            }
            reader.Dispose();
            return list;
        }
    }
}