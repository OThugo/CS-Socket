using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
namespace ChatClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        Socket socket;
        private void btnStart_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(txtServer.Text);
            IPEndPoint endPoint = new IPEndPoint(ip, int.Parse(txtPort.Text));
            //客户端的socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //连接服务器
                socket.Connect(endPoint);

                SetTxt("连接成功");

                //接收消息
                Thread th = new Thread(RecMsg);
                th.IsBackground = true;
                th.Start();
            }
            catch (Exception ex)
            {
                SetTxt(ex.Message);
            }
        }
        //客户端接收服务器的消息
        void RecMsg()
        {
            byte[] buffer = new byte[1024*1024*5];
            while (true)
            {
                int count = socket.Receive(buffer);
                if (count == 0)
                {
                    socket.Close();
                    SetTxt("服务器关闭");
                    break;
                }
               
                //协议   0文字  1 文件  2 震动
                int flag = buffer[0];
                if (flag == 0)
                {
                    string msg = Encoding.UTF8.GetString(buffer,1, count-1);
                    SetTxt(msg);
                }
                else if (flag == 1)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        using (FileStream fs = new FileStream(sfd.FileName,FileMode.Create))
                        {
                            fs.Write(buffer, 1, count - 1);
                        }
                    }
                }
                else if (flag == 2)
                {
                    ZD();
                }
            }
        }
       
        int n = 1;
        void ZD()
        {
            this.TopMost = true;
            for (int i = 0; i < 20; i++)
            {
                this.Location = new Point(this.Location.X - 10 * n, this.Location.Y - 10 * n);
                n = n * -1;
                System.Threading.Thread.Sleep(40);
            }
        }
        void SetTxt(string msg)
        {
            txtLog.AppendText(msg + "\r\n");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //
            //if (socket != null && socket.Connected)
            //{
            //    socket.Shutdown(SocketShutdown.Both);
            //    socket.Close();
            //}
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //发送消息
            if (socket != null && socket.Connected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(txtMsg.Text);
                socket.Send(buffer);
            }
        }
    }
}
