using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace 홍보_상태_모니터_프로그램
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //FormClosing += Form1_FormClosing;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            LoginForm loginForm = new LoginForm();
            loginForm.loginEventHandler += new EventHandler(LoginSuccess);

            switch (loginForm.ShowDialog())
            {
                case DialogResult.OK:
                    loginForm.Close();
                    break;
                case DialogResult.Cancel:
                    Dispose();
                    break;
            }

            dateTimePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            dateTimePicker2.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            /*
            if (Convert.ToInt16(DateTime.Now.ToString("HH")) >= 12)
            {
                dateTimePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 12:00:00";
                dateTimePicker2.Text = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 12:00:00";
            }
            else
            {
                dateTimePicker1.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 12:00:00";
                dateTimePicker2.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 12:00:00";
            }
            */
            loadingServerName();
        }


        private void LoginSuccess(string name)
        {
            MessageBox.Show(name + "님 반갑습니다.");
        }
        /*
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("정말로 종료하시겠습니까?", "종료", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
        */
        private String pw = "9994";
        private Boolean isStart = false;

        int delay = 60;
        bool sCheck = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sCheck)
            {
                delay--;
                delayTextBox.Text = delay.ToString();

                if (delay <= 0)
                {
                    systemMessageBox.Text = "";
                    systemMessageBox_AppendText("업데이트 시간 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Color.MediumVioletRed);
                    loadingServerStatusList();
                    systemMessageBox_AppendText("현재 홍보기의 상태를 업데이트 하였습니다.", Color.GreenYellow);
                    delay = Convert.ToInt16(delayBox.Text);
                }
            }
            else
            {
                timer1.Enabled = false;  // case실행중 Tick의 접근을 막겠다.
            }
        }

        private void allLoading()
        {
            systemMessageBox.Text = "";
            systemMessageBox_AppendText("업데이트 시간 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Color.MediumVioletRed);
            loadingList();
            loadingServerStatusList();
            loadingServerWritingList();
            systemMessageBox_AppendText("현재 홍보기의 상태를 업데이트 하였습니다.", Color.GreenYellow);
            delay = Convert.ToInt16(delayBox.Text);
        }
        private void loadingList()
        {
            completedataGridView.Rows.Clear();

            dateTimePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            dateTimePicker2.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            
            /*
            if (Convert.ToInt16(DateTime.Now.ToString("HH")) >= 12)
            {
                dateTimePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 12:00:00";
                dateTimePicker2.Text = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 12:00:00";
            }
            else
            {
                dateTimePicker1.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 12:00:00";
                dateTimePicker2.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 12:00:00";
            }
            */
            string loadingPopURL = "http://say4u.cafe24.com/poplin/loadingCompleteWriting.php?";

            loadingPopURL += "servername=";
            loadingPopURL += "&writingtimeGreater=" + dateTimePicker1.Text;
            loadingPopURL += "&writingtimeLess=" + dateTimePicker2.Text;
            loadingPopURL += "&type=0";
            systemMessageBox_AppendText("서버이름 : " + ServerNameComboBox.Text, Color.GreenYellow);
            systemMessageBox_AppendText("시작시간 : " + dateTimePicker1.Text, Color.GreenYellow);
            systemMessageBox_AppendText("종료시간 : " + dateTimePicker2.Text, Color.GreenYellow);
            try
            {
                //XElement 요소 Load
                XElement root = XElement.Load(loadingPopURL);

                int i = 1;
                foreach (XElement item in root.Descendants("items").Descendants("item"))
                {
                    string _servername = item.Attribute("servername").Value;
                    string popid = item.Attribute("poplinid").Value;
                    string writingtime = item.Attribute("writingtime").Value;
                    string ip = item.Attribute("ip").Value;
                    string completesubject = item.Attribute("subject").Value;
                    string status = item.Attribute("status").Value;
                    completedataGridView.Rows.Add(
                        i++,
                        _servername,
                        popid,
                        writingtime,
                        completesubject
                        );
                };

                systemMessageBox_AppendText("완료 갯수 : 총 " + (i - 1) + "개가 완료되었습니다", Color.GreenYellow);
                //ServerNameComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void loadingServerStatusList()
        {
            adServerStatusDataGridView.Rows.Clear();
            string loadingPopURL = "http://say4u.cafe24.com/poplin/loadingServerStatus.php?";
            loadingPopURL += "dbname=adstatus";

            try
            {
                //XElement 요소 Load
                XElement root = XElement.Load(loadingPopURL);
                int _serverNumber = 1;
                foreach (XElement item in root.Descendants("items").Descendants("item"))
                {
                    string pcname = item.Attribute("pcname").Value;
                    DateTime updatetime = Convert.ToDateTime(item.Attribute("updatetime").Value);
                    string ipAddress = item.Attribute("ipAddress").Value;
                    string servername = item.Attribute("servername").Value;
                    string adNumber = item.Attribute("adNumber").Value;
                    string version = item.Attribute("version").Value;
                    DateTime writetime = DateTime.Now;
                    if (item.Attribute("writingtime").Value.Length > 5)
                    {
                        writetime = Convert.ToDateTime(item.Attribute("writingtime").Value);
                    }

                    TimeSpan resultTime = DateTime.Now - updatetime;
                    TimeSpan wrTime = DateTime.Now - writetime;
                    adServerStatusDataGridView.Rows.Add(
                        _serverNumber++,
                        pcname,
                        updatetime,
                        ipAddress,
                        servername,
                        adNumber,
                        Convert.ToInt64(resultTime.TotalSeconds / 60),
                        Convert.ToInt64(wrTime.TotalSeconds / 60),
                        version
                        );
                };

                for (int r = 0; r < adServerStatusDataGridView.Rows.Count; r++)
                {
                    if (Convert.ToInt64(adServerStatusDataGridView[6, r].Value.ToString()) <= 30
                        && Convert.ToInt64(adServerStatusDataGridView[7, r].Value.ToString()) >= 30)
                    {
                        int _r = 0;
                        while (_r < 9)
                        {
                            adServerStatusDataGridView[_r, r].Style.BackColor = Color.AliceBlue;
                            adServerStatusDataGridView[_r, r].Style.ForeColor = Color.DarkSlateGray;
                            _r++;
                        }
                        continue;
                    }

                    if (Convert.ToInt64(adServerStatusDataGridView[6, r].Value.ToString()) <= 90
                        && Convert.ToInt64(adServerStatusDataGridView[7, r].Value.ToString()) >= 30)
                    {
                        int _r = 0;
                        while (_r < 9)
                        {
                            adServerStatusDataGridView[_r, r].Style.BackColor = Color.LightPink;
                            adServerStatusDataGridView[_r, r].Style.ForeColor = Color.DarkSlateGray;
                            _r++;
                        }
                        continue;
                    }

                    if (Convert.ToInt64(adServerStatusDataGridView[6, r].Value.ToString()) > 30
                        && Convert.ToInt64(adServerStatusDataGridView[6, r].Value.ToString()) < 1440)
                    {
                        int _r = 0;

                        adServerStatusDataGridView[4, r].Value = "##" + adServerStatusDataGridView[4, r].Value;
                        while (_r < 9)
                        {
                            adServerStatusDataGridView[_r, r].Style.BackColor = Color.DarkRed;
                            adServerStatusDataGridView[_r, r].Style.ForeColor = Color.Beige;
                            _r++;
                        }
                    }

                    if (Convert.ToInt64(adServerStatusDataGridView[6, r].Value.ToString()) > 30
                        && Convert.ToInt64(adServerStatusDataGridView[6, r].Value.ToString()) >= 1440)
                    {
                        int _r = 0;

                        adServerStatusDataGridView[4, r].Value = "##" + adServerStatusDataGridView[4, r].Value;
                        while (_r < 9)
                        {
                            adServerStatusDataGridView[_r, r].Style.BackColor = Color.Black;
                            adServerStatusDataGridView[_r, r].Style.ForeColor = Color.Beige;
                            _r++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }


        private void loadingServerWritingList()
        {
            ServerWritingList.Rows.Clear();
            string loadingPopURL = "http://say4u.cafe24.com/poplin/loadingServerWritingList.php?";
            loadingPopURL += "dbname=poplintext";

            try
            {
                //XElement 요소 Load
                XElement root = XElement.Load(loadingPopURL);

                foreach (XElement item in root.Descendants("items").Descendants("item"))
                {
                    string servername = item.Attribute("servername").Value;
                    string title = item.Attribute("title").Value;
                    string title2 = item.Attribute("title2").Value;
                    string title3 = item.Attribute("title3").Value;

                    ServerWritingList.Rows.Add(
                        servername,
                        title
                        );
                    ServerWritingList.Rows.Add(
    servername,
    title2
    );
                    ServerWritingList.Rows.Add(
    servername,
    title3
    );
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void loadingServerName()
        {
            string loadingPopURL = "http://say4u.cafe24.com/poplin/loadingServerName.php";
            try
            {
                ServerNameComboBox.Items.Add("모든 서버");
                //XElement 요소 Load
                XElement root = XElement.Load(loadingPopURL);
                String sname = "";
                foreach (XElement item in root.Descendants("items").Descendants("item"))
                {
                    string servername = item.Attribute("servername").Value;
                    if (!ServerNameComboBox.Items.Contains(servername))
                    {
                        sname += "홍보 서버 이름 : " + servername + "\r\n";
                        ServerNameComboBox.Items.Add(servername);
                    }
                };
                systemMessageBox_AppendText(sname, Color.DarkKhaki);
                ServerNameComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void loadingButtonClick()
        {
            completedataGridView.Rows.Clear();
            string loadingPopURL = "http://say4u.cafe24.com/poplin/loadingCompleteWriting.php?";

            string _servername = ServerNameComboBox.Text;
            if (ServerNameComboBox.Text.Contains("모든"))
            {
                _servername = "";
            }
            loadingPopURL += "servername=" + _servername;
            loadingPopURL += "&writingtimeGreater=" + dateTimePicker1.Text;
            loadingPopURL += "&writingtimeLess=" + dateTimePicker2.Text;
            loadingPopURL += "&type=0";
            systemMessageBox_AppendText("서버이름 : " + ServerNameComboBox.Text, Color.GreenYellow);
            systemMessageBox_AppendText("시작시간 : " + dateTimePicker1.Text, Color.GreenYellow);
            systemMessageBox_AppendText("종료시간 : " + dateTimePicker2.Text, Color.GreenYellow);
            try
            {
                //XElement 요소 Load
                XElement root = XElement.Load(loadingPopURL);

                int i = 1;
                foreach (XElement item in root.Descendants("items").Descendants("item"))
                {
                    _servername = item.Attribute("servername").Value;
                    string popid = item.Attribute("poplinid").Value;
                    string writingtime = item.Attribute("writingtime").Value;
                    string ip = item.Attribute("ip").Value;
                    string completesubject = item.Attribute("subject").Value;
                    string status = item.Attribute("status").Value;
                    completedataGridView.Rows.Add(
                        i++,
                        _servername,
                        popid,
                        writingtime,
                        completesubject
                        );
                };

                systemMessageBox_AppendText("완료 갯수 : 총 " + (i - 1) + "개가 완료되었습니다", Color.GreenYellow);
                //ServerNameComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void systemMessageBox_AppendText(String s, Color _color)
        {
            systemMessageBox.SelectionColor = _color;
            systemMessageBox.AppendText(s + "\r\n");
            systemMessageBox.SelectionStart = systemMessageBox.Text.Length;
            systemMessageBox.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!pwTextBox.Text.Contains(pw))
            {
                return;
            }
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                MessageBox.Show("종료날이 시작날보다 과거입니다.");
                return;
            }

            loadingButtonClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!pwTextBox.Text.Contains(pw))
            {
                return;
            }
            if (isStart)
            {
                sCheck = false;
                timer1.Enabled = false;
                isStart = false;
                return;
            }

            allLoading();
            isStart = true;

            sCheck = true;
            timer1.Enabled = true;
            // updateProgram();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loadingServerWritingList();
            ///updateProgram();
        }


        ArrayList socklist = new ArrayList(1000);
        ArrayList copylist = new ArrayList(1000);
        ArrayList broadlist = new ArrayList(1000);
        byte[] data = new byte[1024];
        string stringData;
        Boolean s_running;
        Boolean thread_alive = false;
        Socket main;
        IPEndPoint iep;
        int recv;
        //static int s_count = 0;

        private void serverstartbutton_Click(object sender, EventArgs e)
        {


            if (thread_alive == true) return;
            if (s_running == true) return;

            systemMessageBox_AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  Server start", Color.AliceBlue);
            s_running = true;
            main = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            main.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            iep = new IPEndPoint(IPAddress.Any, 10000);

            try
            {
                main.Bind(iep);
                main.Listen(128);
            }
            catch (SocketException ex3)
            {
                this.Invoke(new MethodInvoker(delegate () { systemMessageBox_AppendText(ex3.ToString(), Color.AliceBlue); }));
            }


            Thread work_thread = new Thread(new ThreadStart(main_work));

            work_thread.Start();
            thread_alive = true;

            timer1.Enabled = true;

        }


        private void main_work()
        {
            while (s_running == true)
            {

                if (main.Poll(1000000, SelectMode.SelectRead))
                {


                    Socket client1 = main.Accept();
                    client1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    IPEndPoint iep1 = (IPEndPoint)client1.RemoteEndPoint;
                    lock (this)
                    {
                        socklist.Add(client1);
                    }
                    String result = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  Connected to " + iep1.ToString();
                    //this.Invoke(new MethodInvoker(delegate() { textBox1.Text += iep1.ToString()+ "\r\n"; }));
                    this.Invoke(new MethodInvoker(delegate () { systemMessageBox_AppendText(result, Color.AliceBlue); }));

                    //Thread client_thread = new Thread(new ThreadStart(client_work));
                    //client_thread.Start();
                }
                if (socklist.Count > 0)
                {
                    // System.Threading.Thread.Sleep(1000);
                    lock (this)
                    {
                        copylist = new ArrayList(socklist);

                        try
                        {
                            if (copylist.Count < 1) continue;
                            Socket.Select(copylist, null, null, 1000);
                        }
                        catch (SocketException)
                        {


                        };
                    }

                    foreach (Socket client in copylist)
                    {
                        data = new byte[1024];
                        iep = (IPEndPoint)client.RemoteEndPoint;
                        String result = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  Disconnected to " + iep.ToString();

                        try
                        {
                            recv = client.Receive(data);
                        }
                        catch (SocketException)
                        {

                            client.Shutdown(SocketShutdown.Both);
                            //client.Disconnect(true);
                            client.Close();
                            lock (this)
                            {
                                socklist.Remove(client);
                            }
                            this.Invoke(new MethodInvoker(delegate () { systemMessageBox_AppendText(result, Color.AliceBlue); }));
                            continue;

                        }

                        stringData = Encoding.ASCII.GetString(data, 0, recv);


                        //if (recv == 0)
                        //   continue;
                        if (recv < 1)
                        {

                            try
                            {
                                client.Shutdown(SocketShutdown.Both);
                                //client.Disconnect(true);
                                client.Close();

                                lock (this)
                                {
                                    socklist.Remove(client);
                                }

                                this.Invoke(new MethodInvoker(delegate () { systemMessageBox_AppendText(result, Color.AliceBlue); }));
                            }
                            catch (Exception)
                            {



                            }



                            if (socklist.Count == 0)
                            {
                                //return;
                            }
                        }


                    }


                }

            }
            client_close_all();
            main.Close();
            s_running = false;
            this.Invoke(new MethodInvoker(delegate () { systemMessageBox_AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  Work_thread stop ", Color.AliceBlue); }));
            thread_alive = false;
            //s_count = 0;

        }


        private void client_close_all()
        {
            try
            {
                foreach (Socket client in socklist)
                {
                    if (client.Connected == true)
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();

                    }

                }
                socklist.Clear();
                copylist.Clear();
            }
            catch (SocketException)
            { }
        }

        private void DelayShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string loadingPopURL = "http://say4u.cafe24.com/poplin/";

            loadingPopURL += "loadingDelay.php?"
                 + "database=say4u&username=say4u&password=tkfkdgo1!&code=0x01&tablename=writedelay";
            DelayDataGridView.Rows.Clear();
            try
            {
                loadingPage(loadingPopURL, null, "GET", null, Encoding.GetEncoding("EUC-KR"));

                string[] strTexts = loadingPageHtml.Split(new Char[] { '\'' }, StringSplitOptions.None);
                DelayDataGridView.Rows.Add(
                    strTexts[1],
                    strTexts[3],
                    strTexts[5],
                    strTexts[7],
                    strTexts[9],
                    strTexts[11],
                    strTexts[13],
                    strTexts[15],
                    strTexts[17],
                    strTexts[19],
                    strTexts[21],
                    strTexts[23],
                    strTexts[25],
                    strTexts[27],
                    strTexts[29],
                    strTexts[31],
                    strTexts[33],
                    strTexts[35],
                    strTexts[37],
                    strTexts[39],
                    strTexts[41],
                    strTexts[43],
                    strTexts[45],
                    strTexts[47]
                    );
                systemMessageBox_AppendText("딜레이 불러오기에 성공하였습니다.", Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241))))));
            }
            catch (Exception ex)
            {
                systemMessageBox_AppendText("딜레이를 불어오는 도중 오류가 발생!", Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241))))));

                //textBoxComment.AppendText(ex.StackTrace);
                Console.WriteLine(ex.StackTrace);
            }
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
                /*
                _webRequest.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
                _webRequest.Timeout = 10000;
                _webRequest.ReadWriteTimeout = 10000;
                */
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
                /*
                using (_webResponse = (HttpWebResponse)_webRequest.GetResponse())
                {
                    Stream respPostStream = _webResponse.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                    h = readerPost.ReadToEnd();
                }
                */

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