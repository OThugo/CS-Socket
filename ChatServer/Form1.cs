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
using System.Threading;
using System.IO;
namespace ChatServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(txtServer.Text);
            //网络端点（ip地址和端口）
            IPEndPoint endPoint = new IPEndPoint(ip, int.Parse(txtPort.Text));

            //创建监听用的socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //绑定网络端点
                socket.Bind(endPoint);
                //开始监听
                socket.Listen(10);
                SetTxt("开始监听");
                btnStart.Enabled = false;

                //启动线程 监听客户端的连接
                Thread th = new Thread(Listen);
                //设置后台线程
                th.IsBackground = true;
                th.Start(socket);

            }
            catch (Exception ex)
            {
                SetTxt(ex.Message);
                btnStart.Enabled = true;
            }
        }
        Dictionary<string, Socket> dic = new Dictionary<string, Socket>();      
        void Listen(object o)
        {
            //监听用的socket
            Socket socket = o as Socket;
            while (true)
            {
                //当客户端连接成功，创建一个通信用的socket   阻塞窗体运行
                Socket connSocket = socket.Accept();
                //客户端的ip和端口
                string strIP = connSocket.RemoteEndPoint.ToString();
                SetTxt(strIP + " : 连接成功");
                //把客户端添加到下拉框
                cboUsers.Items.Add(strIP);
                //把通信用的socket添加到字典中
                dic.Add(strIP, connSocket);


                //接收消息
                Thread th = new Thread(RecMsg);
                th.IsBackground = true;
                th.Start(connSocket);
            }
        }
        //接收客户端的消息
        void RecMsg(object o)
        {
            Socket connSocket = o as Socket;
         
            //接收客户端发送过来的消息
            byte[] buffer = new byte[1024 * 1024 * 5];
            while (true)
            {
                //count 实际接收的字节个数 阻塞运行
                int count = connSocket.Receive(buffer);
                string strIP = connSocket.RemoteEndPoint.ToString();
                //当收到长度为0的字节数组，说明对方close了连接
                if (count == 0)
                {
                    connSocket.Close();
                    SetTxt(strIP + " : 断开连接");
                    break;
                }
              
                string msg = Encoding.UTF8.GetString(buffer, 0, count);
                SetTxt(strIP + " : " + msg);
            }
           
        }

        void SetTxt(string msg)
        {
            txtLog.AppendText(msg + "\r\n");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //向客户端发文字消息
            if (cboUsers.SelectedIndex >= 0)
            {
                string key = cboUsers.Text;

                byte[] buffer = Encoding.UTF8.GetBytes(txtMsg.Text);


                List<byte> list = new List<byte>();
                list.Add(0);//协议  0 文字
                list.AddRange(buffer);

                
                dic[key].Send(list.ToArray());
            }
            else
            {
                MessageBox.Show("请选择客户端");
            }
        }
        //选择文件
        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = ofd.FileName;
            }
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            //发送文件
            if (cboUsers.SelectedIndex >= 0)
            {
                string key = cboUsers.Text;
                using (FileStream fs = new FileStream(txtPath.Text,FileMode.Open))
                { 
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    

                    //[1][10][数据]
                    List<byte> list = new List<byte>();
                    list.Add(1); //协议  1  文件
                    list.AddRange(buffer);
                    dic[key].Send(list.ToArray());
                }


                
            }
            else
            {
                MessageBox.Show("请选择客户端");
            }
        }

        private void btnZD_Click(object sender, EventArgs e)
        {
            //发送文件
            if (cboUsers.SelectedIndex >= 0)
            {
                string key = cboUsers.Text;

                byte[] buffer = new byte[1];
                buffer[0] = 2; //协议  2 震动

                dic[key].Send(buffer);

            }
            else
            {
                MessageBox.Show("请选择客户端");
            }
        }
    }
}
