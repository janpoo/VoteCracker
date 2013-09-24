using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;

using System.IO;
namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        string[] historyArr = null;

        //人员:IP:Port:MSG
        string[] proxyArr = null;
        string[] manArr = null;

        public Form1()
        {
            InitializeComponent();


          
        }

       


        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                string[] numPreStrs = new string[] { "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "151", "52", "153", "154", "155", "156", "157", "158", "159", "180", "181", "182", "183", "184", "185", "186", "187", "188", "189" };
                System.Random r =  new System.Random(System.DateTime.Now.Millisecond);
                string username ="janpoo"+ r.Next(1, 999999).ToString() ;
                string mobile = "138"+ r.Next(10000000, 99999999).ToString();
                string pwd = r.Next(1000, 9999).ToString();
                string sXmlMessage = "voteid=20&chkid=270&name=" + username + "&tel=" + mobile + "&yzmyzmm=" + pwd + "&firstecodedd=" + pwd + "&Submit3=" + pwd;//%CC%E1%BD%BB%CD%B6%C6%B1
                //把sXmlMessage发送到指定的DsmpUrl地址上
                Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                byte[] arrB = encode.GetBytes(sXmlMessage);

                string ip = (textBox1.Text.IndexOf(':') > 0) ? textBox1.Text.Split(':')[0] : textBox1.Text.Split(' ')[0];
                ip = ip.Trim();
                string port = (textBox1.Text.IndexOf(':') > 0) ? textBox1.Text.Split(':')[1] : textBox1.Text.Split(' ')[1];
                port = port.Trim();
                WebProxy proxyObject = new WebProxy(ip, int.Parse(port));
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("http://cmf.328f.cn/vote/vot.asp");
                myReq.Proxy = proxyObject;
                myReq.Method = "POST";
                myReq.ContentType = "application/x-www-form-urlencoded";
                myReq.ContentLength = arrB.Length;
                myReq.Headers.Add("SOURCE", "256.256.256."+r.Next(100, 255).ToString());
                myReq.Headers.Add("VIA", "256.256.256." + r.Next(100, 255).ToString());
                myReq.Headers.Add("X_FORWARDED_FOR", "256.256.256." + r.Next(100, 255).ToString());
                myReq.Headers.Add("REMOTE_ADDR", "256.256.256." + r.Next(100, 255).ToString());
                
                Stream outStream = myReq.GetRequestStream();
                outStream.Write(arrB, 0, arrB.Length);
                outStream.Close();


                //接收HTTP做出的响应
                WebResponse myResp = myReq.GetResponse();
                Stream ReceiveStream = myResp.GetResponseStream();
                StreamReader readStream = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                string str = null;
                while (count > 0)
                {
                    str += new String(read, 0, count);
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Close();
                myResp.Close();
                if(true)
                {
                    FileStream fs = new FileStream("c:\\proxy.txt", FileMode.Append);
                    //获得字节数组
                    byte[] data = new UTF8Encoding().GetBytes(textBox1.Text+"\r\n");
                    //开始写入
                    fs.Write(data, 0, data.Length);
                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();
                }
                MessageBox.Show(str);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            this.loadData();
            int votTimes;
            string msg = "";
            int success = 0;
            int fail = 0;
            try
            {
                votTimes = int.Parse(textBox3.Text);
                for (int i = 0; i < votTimes; i++)
                {
                    if (File.Exists("c:\\history.txt"))
                    {
                        this.historyArr = File.ReadAllLines("c:\\history.txt");
                    }

                    string m = getName();
                    string p = getProxy();
                    string ip = p.Split(':')[0];
                    string port = p.Split(':')[1];
                    if (m == "")
                    {
                        MessageBox.Show("没有设置人员名单");
                        return;
                    }

                    if (p == "")
                    {
                        MessageBox.Show("没有设置代理列表");
                        return;
                    }

                    string result = vote(m, ip, port);

                   
                    if (result.StartsWith( "投票成功"))
                    {
                        success++;
                    }
                    else
                    {
                        fail++;
                    }


                    string showmsg = m + ":" + p + ":" + result;
                    StreamWriter sw = new StreamWriter("c:\\history.txt",true);
                    sw.WriteLine(showmsg);
                    sw.Flush();
                    sw.Close();

                    listBox1.Items.Add(showmsg);
                }

            }
            catch (Exception ex)
            {
                listBox1.Items.Add("投票出错："+ex.Message);
            }

        }
        private void loadData()
        {
            if (File.Exists("c:\\proxy.txt"))
            {
                this.proxyArr = File.ReadAllLines("c:\\proxy.txt");
            }
            else
            {
                MessageBox.Show("c:\\proxy.txt不存在");
                this.Close();
            }
            /*
            if (File.Exists("c:\\man.txt"))
            {
                this.manArr = File.ReadAllLines("c:\\man.txt");

            }
            else
            {
                MessageBox.Show("c:\\man.txt不存在");
                this.Close();
            }


            if (File.Exists("c:\\history.txt"))
            {
                this.historyArr = File.ReadAllLines("c:\\history.txt");
            }
             * */
        }
        private string vote(string man,string ip,string port)
        {
            string result = "投票成功";
            try
            {
                string[] numPreStrs = new string[] { "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "151", "52", "153", "154", "155", "156", "157", "158", "159", "180", "181", "182", "183", "184", "185", "186", "187", "188", "189" };
                System.Random r = new System.Random(System.DateTime.Now.Millisecond);
                string username = man;
                string pre = numPreStrs[r.Next(0, numPreStrs.Length - 1)];
                string mobile = pre + r.Next(10000000, 99999999).ToString();
                string pwd = r.Next(1000, 9999).ToString();
                string sXmlMessage = "voteid=20&chkid=270&name=" + username + "&tel=" + mobile + "&yzmyzmm=" + pwd + "&firstecodedd=" + pwd + "&Submit3=" + pwd;//%CC%E1%BD%BB%CD%B6%C6%B1
               
                
                //把sXmlMessage发送到指定的DsmpUrl地址上
                Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                byte[] arrB = encode.GetBytes(sXmlMessage);

                WebProxy proxyObject = new WebProxy(ip, int.Parse(port));
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("http://cmf.328f.cn/vote/vot.asp");
                myReq.Proxy = proxyObject;
                myReq.Method = "POST";
                myReq.ContentType = "application/x-www-form-urlencoded";
                myReq.ContentLength = arrB.Length;
               /*
                myReq.Headers.Add("SOURCE", "256.256.256." + r.Next(100, 255).ToString());
                myReq.Headers.Add("VIA", "256.256.256." + r.Next(100, 255).ToString());
                myReq.Headers.Add("X_FORWARDED_FOR", "256.256.256." + r.Next(100, 255).ToString());
                myReq.Headers.Add("REMOTE_ADDR", "256.256.256." + r.Next(100, 255).ToString());
                */
                Stream outStream = myReq.GetRequestStream();
                outStream.Write(arrB, 0, arrB.Length);
                outStream.Close();


                //接收HTTP做出的响应
                WebResponse myResp = myReq.GetResponse();
                Stream ReceiveStream = myResp.GetResponseStream();
                StreamReader readStream = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                string str = null;
                while (count > 0)
                {
                    str += new String(read, 0, count);
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Close();
                myResp.Close();
                if (str.IndexOf("呵呵") >= 0)
                {
                    result = "投票失败(" + mobile + ")";
                }
                else
                {
                    result = "投票成功("+mobile+")" ;
                }
            }
            catch (Exception ex)
            {
                result = "投票出错[" + ex.Message + "]";
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        private string getName()
        {
            if (historyArr == null)
            {
                return manArr[0].Trim();
            }
            bool f = true;
            for (int i = 0; i < this.manArr.Count(); i++)
            {
                f = true;
                for (int j = 0; j < this.historyArr.Count(); j++)
                {
                    if (historyArr[j].Trim().IndexOf(manArr[i].Trim()) >= 0)
                    {
                        f = false;
                        break;
                    }
                }

                if (f)
                {
                    return manArr[i].Trim();
                }
            }
            return string.Empty;
        }


        private string getProxy()
        {
            if (historyArr == null)
            {
                return proxyArr[0].Trim();
            }

            bool f = true;
            for (int i = 0; i < this.proxyArr.Count(); i++)
            {
                for (int j = 0; j < this.historyArr.Count(); j++)
                {
                    f = true;
                    if (historyArr[j].Trim().IndexOf(proxyArr[i].Trim()) >= 0)
                    {
                        f = false;
                        break;
                    }
                }

                if (f)
                {
                    return proxyArr[i].Trim();
                }
            }
            return string.Empty;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.proxyArr = File.ReadAllLines("c:\\proxy.txt");
            for (int i = 0; i < this.proxyArr.Length; i++)
            {
                string p = this.proxyArr[i];
                if (p.Trim() == "") continue;
                string ip = p.Split(':')[0];
                string port = p.Split(':')[1];
                System.Threading.Thread.Sleep(100);
                System.Threading.Thread t = new System.Threading.Thread(this.readUrl);
                t.Start(p);
                
            }
        }


        private void readUrl(object o )
        {
            string p = o.ToString();
            string ip = p.Split(':')[0];
            string port = p.Split(':')[1];
            string url = "http://www.19lou.com/forum-94-thread-10191379159677285-1-1.html";
            try
            {
                

                WebProxy proxyObject = new WebProxy(ip, int.Parse(port));
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
                myReq.Proxy = proxyObject;
                myReq.Method = "GET";
                myReq.ContentType = "application/x-www-form-urlencoded";
                //myReq.ContentLength = arrB.Length;
                /*
                 myReq.Headers.Add("SOURCE", "256.256.256." + r.Next(100, 255).ToString());
                 myReq.Headers.Add("VIA", "256.256.256." + r.Next(100, 255).ToString());
                 myReq.Headers.Add("X_FORWARDED_FOR", "256.256.256." + r.Next(100, 255).ToString());
                 myReq.Headers.Add("REMOTE_ADDR", "256.256.256." + r.Next(100, 255).ToString());
                 */
                //Stream outStream = myReq.GetRequestStream();
               // outStream.Write(arrB, 0, arrB.Length);
                //outStream.Close();


                //接收HTTP做出的响应
                WebResponse myResp = myReq.GetResponse();
                Stream ReceiveStream = myResp.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                StreamReader readStream = new StreamReader(ReceiveStream, encode);
              
                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                string str = null;
                while (count > 0)
                {
                    str += new String(read, 0, count);
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Close();
                myResp.Close();
               // listBox1.Items.Add(str);
                
            }
            catch (Exception ex)
            {
                //result = "投票出错[" + ex.Message + "]";
                MessageBox.Show(ex.Message);
            }

            //return result;
        }


    }




}
