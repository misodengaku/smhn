using System;
using System.Xml.Linq;

namespace SmhnLib
{
    public class NewsItemClass
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri Link { get; set; }
        //public Uri CommentsLink { get; set; }
        public DateTime Date { get; set; }
        public int CommentsCount { get; set; }

        public NewsItemClass(XElement element)
        {
            XNamespace fb = "http://rssnamespace.org/feedburner/ext/1.0";
            XNamespace slash = "http://purl.org/rss/1.0/modules/slash/";
            Title = (string)element.Element("title");
            Description = (string)element.Element("encoded");
            Link = new Uri((string)element.Element(fb + "origLink"));
            Date = DateTime.Parse((string)element.Element("pubDate"));
            CommentsCount = int.Parse(element.Element(slash + "comments").Value);
        }
    }
}
