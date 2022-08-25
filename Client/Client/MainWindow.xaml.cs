using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.Windows.Interop;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket client;
        public String host;
        public int port;
        public string portStr;
        public string username;
        public string message;
        public int connect;
        private byte[] result;
        public int receiveNumber;
        public string recStr;
        public MainWindow()
        {
            InitializeComponent();           
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                receive_msg.Text = "";            
                host = socket_ip.Text;
                port = int.Parse(socket_port.Text);
                username = name.Text;
                IPAddress ip = IPAddress.Parse(host);
                //socket()
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //connect()
                client.Connect(new IPEndPoint(ip, port));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                receive_msg.Text += "連線失敗\n";
            }
            //connect to socket server

        }

        //send()
        private void Send(object sender, RoutedEventArgs e)
        {
            if (connect == 1)
            {
                host = socket_ip.Text;
                port = int.Parse(socket_port.Text);
                username = name.Text;
                message = msg.Text;
                ClientData clientData = new ClientData(host, port, username, message, 1);
                string jsonString = System.Text.Json.JsonSerializer.Serialize(clientData);
                try
                {
                    Console.WriteLine("Client第二次傳訊 " + jsonString);
                    //send message to server
                    client.Send(Encoding.ASCII.GetBytes(jsonString));
                    msg.Text = "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    receive_msg.Text += "傳送失敗\n";
                }
            }
            String text = msg.Text;


        }

        //receive()
        public void ReceiveMessage()
        {
            ClientData clientData = new ClientData(host, port, username, "", 1);
            string jsonString = System.Text.Json.JsonSerializer.Serialize(clientData);
            client.Send(Encoding.ASCII.GetBytes(jsonString));
            Console.WriteLine("Client第一次傳訊 " + jsonString);   //print
            while (true)
            {
                try
                {
                    byte[] result = new byte[1024];
                    int receiveNumber = client.Receive(result);
                    String recStr = Encoding.ASCII.GetString(result, 0, receiveNumber);
                    if (receiveNumber > 0)
                    {
                        Console.WriteLine("Client持續收訊 " + recStr);   //print
                        ClientData clientData2 = JsonConvert.DeserializeObject<ClientData>(recStr);
                        message = clientData2.clientMsg;
                        connect = clientData2.clientConn;
                        receive_msg.Dispatcher.BeginInvoke(
                           new Action(() => { receive_msg.Text += message; }), null);
                        if (connect == 0)
                        {
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                            Console.WriteLine("client斷線");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //exception close()
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    //
                    break;
                }
            }

        }
        private void DisConnect(object sender, RoutedEventArgs e)
        {
            ClientData clientData = new ClientData("", 0, "", username + " 已離開聊天室...\n", 0);
            string jsonString = System.Text.Json.JsonSerializer.Serialize(clientData);
            client.Send(Encoding.ASCII.GetBytes(jsonString));
            //receive_msg.Text = "";
        }
    }
}
