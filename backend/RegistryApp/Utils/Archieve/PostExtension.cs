using System.Collections.Specialized;
using System.Text;


namespace RegistryApp.Utils.Archieve {
    public static class HttpExtension {
        public static string GenerateRedirectFormWithParams(NameValueCollection paramsList, string url) {
            StringBuilder sendForm = new();

            sendForm.Append("<html>");
            sendForm.Append("<body onload='formSubmitter()'>");
            sendForm.Append($"<form id='hidden-form' action='{url}' method='post'>");

            foreach (string key in paramsList)
                sendForm.Append($"<input type='hidden' name='{key}' value='{paramsList[key]}' />");

            sendForm.Append("</form>");

            sendForm.Append("<script>" +
                "window.addEventListener('load', () => {var formCard = document.getElementById('hidden-form');formCard.submit();});" +
                "</script>");

            sendForm.Append("</body></html>");

            return sendForm.ToString();
        }
    }
}