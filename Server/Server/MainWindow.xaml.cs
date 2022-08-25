using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public String host="127.0.0.1";
        //public int port = 20001;
        public Socket server;
        public String host;
        public int port;
        public String portstr;
        public bool flag;
        public string username;
        public string message;
        List<Socket> sockets = new List<Socket>();
        public MainWindow()
        {
            InitializeComponent();
        }

        //start a socket server
        private void Start(object sender, RoutedEventArgs e)
        { 
            if (server == null)
            {
                host = host_ip.Text;
                port = int.Parse(host_port.Text);
                receive_msg.Text = "Server Start\n";
                IPAddress ip = IPAddress.Parse(host);
                //socket()
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //bind()
                server.Bind(new IPEndPoint(ip, port));
                //listen()
                flag = true;
                server.Listen(10);

                Thread thread = new Thread(Listen);
                thread.Start();
            }
        }

        //listen to socket client
        private void Listen()
        {
            while (flag)
            {
                try
                {
                    //accept()
                    Socket client = server.Accept();
                    sockets.Add(client);
                    Thread receive = new Thread(ReceiveMsg);
                    receive.Start(client);
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        //receive client message and send to client
        public void ReceiveMsg(object client)
        {
            bool first = true;
            Socket connection = (Socket)client;
            IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
            byte[] result = new byte[1024];
            //receive message from client
            int receive_num = connection.Receive(result);
            String receive_str = Encoding.ASCII.GetString(result, 0, receive_num);
            while (true)
            {
                try
                {
                    if (receive_num > 0 && first)
                    {
                        first = false;
                        Console.WriteLine("Server第一次收訊 " + receive_str);
                        ClientData clientData = JsonConvert.DeserializeObject<ClientData>(receive_str);
                        username = clientData.clientName;

                        ClientData clientData2 = new ClientData("", 0, "", "伺服器： " + username + "(" + clientIP + ")已進來聊天室\n", 1);
                        string jsonString = System.Text.Json.JsonSerializer.Serialize(clientData2);



                        //String send_str = receive_strs+"("+clientIP + "): " + receive_str;
                        //resend message to client
                        //connection.Send(Encoding.ASCII.GetBytes("You send: " + receive_str));
                        //connection.Send(Encoding.ASCII.GetBytes(receive_strs + "(" + clientIP + "):" + "send: " + receive_str));
                        receive_msg.Dispatcher.BeginInvoke(
                            new Action(() => { receive_msg.Text += username + " 已進來聊天室\n"; }), null);
                        for (int i = 0; i < sockets.Count; i++)
                        {
                            if (sockets[i] != null)
                            {
                                sockets[i].Send(Encoding.ASCII.GetBytes(jsonString));
                                Console.WriteLine("送出" + jsonString);   //print
                            }
                        }

                    }
                    else if (receive_num > 0 && !first)
                    {
                        result = new byte[1024];
                        receive_num = connection.Receive(result);  //receive message from client
                        receive_str = Encoding.ASCII.GetString(result, 0, receive_num);
                        if (receive_num > 0)
                        {
                            Console.WriteLine("Server第二次收訊 " + receive_str);
                            ClientData clientData = JsonConvert.DeserializeObject<ClientData>(receive_str);
                            username = clientData.clientName;
                            message = clientData.clientMsg;
                            int connect = clientData.clientConn;
                            if (connect == 1)
                            {
                                ClientData clientData2 = new ClientData("", 0, "", username + " : " + message + "\n", 1);
                                string jsonString = System.Text.Json.JsonSerializer.Serialize(clientData2);
                                ClientData clientData3 = new ClientData("", 0, "", "您傳送 : " + message + "\n", 1);
                                string jsonString2 = System.Text.Json.JsonSerializer.Serialize(clientData3);

                                for (int i = 0; i < sockets.Count; i++)   //resend message to client
                                {
                                    if (sockets[i] != null)
                                    {
                                        if (sockets[i].Equals(client))
                                        {
                                            sockets[i].Send(Encoding.ASCII.GetBytes(jsonString2));
                                            Console.WriteLine("送出" + jsonString2);   //print
                                        }
                                        else
                                        {
                                            sockets[i].Send(Encoding.ASCII.GetBytes(jsonString));
                                            Console.WriteLine("送出" + jsonString);   //print
                                        }
                                    }
                                }
                                receive_msg.Dispatcher.BeginInvoke(new Action(() => { receive_msg.Text += username + " : " + message + "\n"; }), null);
                            }
                            else
                            {
                                Console.WriteLine("收到client斷訊通知");   //print
                                ClientData clientData4 = new ClientData("", 0, "", message, 1);
                                string jsonString3 = System.Text.Json.JsonSerializer.Serialize(clientData4);
                                ClientData clientData5 = new ClientData("", 0, "", "您已離開聊天室...\n", 0);
                                string jsonString4 = System.Text.Json.JsonSerializer.Serialize(clientData5);

                                for (int i = 0; i < sockets.Count; i++)
                                {
                                    if (sockets[i] != null)
                                    {
                                        if (sockets[i].Equals(client))
                                        {
                                            sockets[i].Send(Encoding.ASCII.GetBytes(jsonString4));
                                            Console.WriteLine("送出" + jsonString4);   //print
                                            sockets[i] = null;
                                        }
                                        else
                                        {
                                            sockets[i].Send(Encoding.ASCII.GetBytes(jsonString3));
                                            Console.WriteLine("送出" + jsonString3);   //print
                                        }
                                    }
                                }
                                receive_msg.Dispatcher.BeginInvoke(new Action(() => { receive_msg.Text += message; }), null);
                            }

                        }
                    }

                }
                catch (Exception e)
                {
                    //exception close()
                    Console.WriteLine(e);
                    connection.Shutdown(SocketShutdown.Both);
                    connection.Close();
                    break;
                }
            }
        }

        //close() when close window
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
        private void Stop(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                flag = false;
                ClientData clientData = new ClientData("", 0, "", "伺服器已離線...\n", 0);
                string jsonString = System.Text.Json.JsonSerializer.Serialize(clientData);

                for (int i = 0; i < sockets.Count; i++)
                {
                    if (sockets[i] != null)
                    {
                        sockets[i].Send(Encoding.ASCII.GetBytes(jsonString));
                    }
                }
                receive_msg.Dispatcher.BeginInvoke(new Action(() => { receive_msg.Text += "伺服器已離線...\n"; }), null);
                //connection.Shutdown(SocketShutdown.Both);
                receive_msg.Text = "";
                sockets.Clear();
                server.Close();
                server = null;

                Console.WriteLine("伺服器斷線\n");
            }
        }
    }
}   
