using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Xml.Linq;

namespace webjob
{

    class Post
    {
        public string Titulo
        {
            get;
            set;
        }

        public DateTime DataPublicacao
        {
            get;
            set;
        }
        public string Conteudo
        {
            get;
            set;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var xmlRss = RequestRssFeed();
            var post = ParseXml(xmlRss);
            EnviarEmail(post);
        }
        private static string RequestRssFeed()
        {
            var xmlRss = string.Empty;
            var url = "https://azure.microsoft.com/en-us/blog/feed/";
            var request = WebRequest.Create(url);
            request.Method = "GET";

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    xmlRss = sr.ReadToEnd();

                }
            }
            return xmlRss;
        }
        private static Post ParseXml(string xmlRss)
        {
            var xml = XElement.Parse(xmlRss);
            var ultimoPost = xml.Elements().First().Element("item");

            var post = new Post();
          
            post.Titulo = ultimoPost.Element("title").Value;
            post.DataPublicacao = DateTime.Parse(
                                       ultimoPost.Element("pubDate").Value);
            post.Conteudo = ultimoPost.Element("link").Value;

            return post;
        }
        private static void EnviarEmail(Post post)
        {
            var titulo =
                string.Concat(post.DataPublicacao, ":", post.Titulo);
            var email = new MailMessage("livroazurewebjob@gmail.com", "ademar.rohregger@gmail.com",
                titulo,
                post.Conteudo);
            using (SmtpClient smtp =
                                new SmtpClient("smtp.gmail.com", 587) )
            {
                smtp.Credentials =
                            new NetworkCredential("livroazurewebjob@gmail.com", "@livroazure1234");
                smtp.EnableSsl = true;
                smtp.Send(email);
            }
        }
    }

}
