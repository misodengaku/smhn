using System;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace SmhnLib
{
    [DataContract]
    public class NewsItemClass
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Uri Link { get; set; }
        //public Uri CommentsLink { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public int CommentsCount { get; set; }
        [DataMember]
        public string ImageSource { get; set; }

        public NewsItemClass(XElement element)
        {
            XNamespace fb = "http://rssnamespace.org/feedburner/ext/1.0";
            XNamespace slash = "http://purl.org/rss/1.0/modules/slash/";
            XNamespace content = "http://purl.org/rss/1.0/modules/content/";
            Title = (string)element.Element("title");

            string[] del = {"<br>", "<br/>", "<br />" };

            Description = ((string)element.Element(content + "encoded")).Split(del, StringSplitOptions.RemoveEmptyEntries)[0];
            Link = new Uri((string)element.Element(fb + "origLink"));
            Date = DateTime.Parse((string)element.Element("pubDate"));
            CommentsCount = int.Parse(element.Element(slash + "comments").Value);
            //ImageSource = (string)element.Element(content + "encoded").Element("img").Value;
        }
    }
}
