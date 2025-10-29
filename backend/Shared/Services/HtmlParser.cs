using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;

namespace Shared.Services {

    public interface IHtmlParser {
        public string GetEmailPageAsString(string docName, string inputLink, Dictionary<string, string>? additionalData = null);
    }

    public class HtmlParser : IHtmlParser {
        private readonly IWebHostEnvironment _webHostEnv;
        private HtmlDocument _htmlDocument;

        public HtmlParser(IWebHostEnvironment webHostEnv) {
            _webHostEnv = webHostEnv;
            _htmlDocument = new HtmlDocument();
        }

        public string GetEmailPageAsString(string docName, string inputLink, Dictionary<string, string>? additionalData = null) {
            var htmlFilePath = Path.Combine(_webHostEnv.WebRootPath, "html", docName);
            var html = GetEmailPage(htmlFilePath, inputLink, additionalData) ??
                throw new Exception("Html template is not found in app environment");

            return html;
        }

        private string? GetEmailPage(string filepath, string inputLink, Dictionary<string, string>? additionalData) {
            _htmlDocument.Load(filepath);


            var linkNode = _htmlDocument.DocumentNode.SelectSingleNode("//a");
            if (linkNode != null) {
                linkNode.SetAttributeValue("href", inputLink);
            }


            if (additionalData != null) {
                foreach (var pair in additionalData) {
                    var element = _htmlDocument.GetElementbyId(pair.Key);
                    if (element != null) {
                        element.InnerHtml = HtmlEntity.Entitize(pair.Value);
                    }
                }
            }

            using var writer = new StringWriter();
            _htmlDocument.Save(writer);
            return writer.ToString();
        }
    }

}
