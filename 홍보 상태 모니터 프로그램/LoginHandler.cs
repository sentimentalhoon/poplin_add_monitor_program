using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace 홍보_상태_모니터_프로그램
{
    class LoginHandler
    {

        private string name = "";
        public bool LoginCheck(string id, string password)
        {
            try
            {
                string loadingPopURL = string.Format("http://say4u.cafe24.com/poplin/auth.php?username={0}&password={1}", id, password);

                MessageBox.Show(loadingPopURL);
                loadingPage(loadingPopURL, null, "POST", null, Encoding.GetEncoding("EUC -KR"));
                if (!loadingPageHtml.Contains("환영"))
                {
                    return false;
                }
                name = id;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        public string GetUserName()
        {
            return name;
        }

        private HttpWebRequest _webRequest;
        private HttpWebResponse _webResponse;
        public static CookieContainer _cookieContainer;
        public static CookieCollection _cookieCollection;
        private Stream _responseStream;
        private StreamReader _streamReader;
        //private StreamWriter g;
        private string loadingPageHtml;

        public void _init()
        {
            _cookieContainer = new CookieContainer();
            _cookieCollection = new CookieCollection();
        }

        public string loadingPage(string _url, string _referer, string _method, string _params, Encoding _encoding)
        {
            try
            {
                if (_cookieContainer == null)
                {
                    _init();
                }

                _webRequest = (HttpWebRequest)WebRequest.Create(_url);
                _webRequest.Method = _method;
                _webRequest.ServicePoint.Expect100Continue = false;
                _webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                _webRequest.Accept = "text/html, application/xhtml+xml, */*";
                _webRequest.KeepAlive = true;
                _webRequest.ContentType = "application/x-www-form-urlencoded";        
                _webRequest.AllowAutoRedirect = true;

                if (_referer != null)
                {
                    _webRequest.Referer = _referer;
                }

                _webRequest.CookieContainer = _cookieContainer;

                if (_params != null)
                {
                    byte[] bytes = Encoding.Default.GetBytes(_params);
                    _webRequest.ContentLength = bytes.Length;
                    Stream requestStream = _webRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                _webResponse.Cookies = _webRequest.CookieContainer.GetCookies(_webRequest.RequestUri);
                _cookieCollection.Add(_webResponse.Cookies);
                _responseStream = _webResponse.GetResponseStream();
                _streamReader = new StreamReader(_responseStream, _encoding, true);
                loadingPageHtml = _streamReader.ReadToEnd();
            }
            catch (WebException exception)
            {
                return exception.Message;
            }
            catch (Exception exception2)
            {
                return exception2.Message;
            }
            finally
            {
                if (_responseStream != null)
                {
                    _responseStream.Close();
                }
                if (_streamReader != null)
                {
                    _streamReader.Close();
                }
            }
            return loadingPageHtml;
        }
    }
}
