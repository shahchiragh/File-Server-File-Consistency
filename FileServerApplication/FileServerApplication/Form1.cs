//Author:Shah Chirag Hareshkumar
//UID: 1001558824
using System;
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

namespace FileServerApplication
{
    public partial class fileServerForm : Form
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);  

        IPEndPoint ipEnd;
        Socket sock;
        //Socket clientSock;
        public static string receivedPath = "E:\\FT";

        public static string client1Path = "E:\\Client1-FT";
        public static string client2Path = "E:\\Client2-FT";
        public static string client3Path = "E:\\Client3-FT";

        BackgroundWorker backgroundWorker1;
        public int ListenState;
        public TcpListener ServerListener;
        public static int clientCount=0;
        private static List<TcpClient> _allClientSockets = new List<TcpClient>();

        public fileServerForm()
        {
            InitializeComponent();
            this.backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void startServer_Click(object sender, EventArgs e)
        {
            if (receivedPath.Length > 0)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            //StartListening();
        }

        private void stopServer_Click(object sender, EventArgs e)
        {
            try
            {
                ListenState = 0;
                //clientSock.Shutdown(SocketShutdown.Both);
                //clientSock.Close();
                ServerListener.Stop();

                _allClientSockets.Clear();
                //clientSock.Close();
                this.Invoke(new MethodInvoker(delegate
                {
                    serverOutput.AppendText("\n>> Stopping Server at Port Number : 5666\n");
                }));
                System.Windows.Forms.Application.ExitThread();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when a server closes a connection: " + ex.Message);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //StartServer();
            this.Invoke(new MethodInvoker(delegate
            {
                serverOutput.AppendText("\n>> Starting Server at Port Number : 5666");
            }));

           // ipEnd = new IPEndPoint(IPAddress.Any, 5658);
           // sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            ServerListener = new TcpListener(IPAddress.Any, 5666);
            ServerListener.Start();
            ListenState = 1;
            StartServer();
                //Thread startThreads = new Thread(StartServer);
                //startThreads.Start();
            //Console.WriteLine("Server is connected and listening..");
             //TcpServer1(5666);
      
        }

        private void StartServer()
        {
            try
            {
                while(ListenState==1)
                {
                    TcpClient clientSock = ServerListener.AcceptTcpClient();
                    _allClientSockets.Add(clientSock);
                    this.Invoke(new MethodInvoker(delegate
                    {
                        serverOutput.AppendText("\n>> Client: " + clientSock.Client.RemoteEndPoint.ToString() + " is connecting");
                    }));
                   // Console.WriteLine("Connections from:" + clientSock.RemoteEndPoint.ToString());
                    Thread clientSendReceiveThread = new Thread(new ParameterizedThreadStart(clientOperations));
                    clientSendReceiveThread.Start(clientSock);
                    //Console.WriteLine("Server is now accepting and starting new threads....");
                    //clientCount += 1;
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server stopped with exception : "+ex.Message);
            }
        }

        private void clientOperations(object obj)
        {
            //Console.WriteLine("Coming in client operations..");
            TcpClient clientSock = (TcpClient)obj;
            while (clientSock.Connected)
            {
                byte[] clientData = new byte[4096];
                int receivedBytesLen = 0;
                byte[] sendServerResponse = new byte[1024];
                try
                {
                    receivedBytesLen = clientSock.Client.Receive(clientData);
                }
                catch (Exception ex)
                {
                  MessageBox.Show("Error when a server receives a connection: "+ex.Message);
                }
                string operationName = Encoding.ASCII.GetString(clientData, 0, receivedBytesLen);

                if (operationName.Contains("PULL"))//means client wants the latest files..
                {
                    try
                    {
                        //File.Copy("E:\\FT\\ChiragShah.txt", client1Path + "\\" + "ChiragShah.txt", true);
                        //File.Copy("E:\\FT\\ChiragShah.txt", client2Path + "\\" + "ChiragShah.txt", true);
                        //File.Copy("E:\\FT\\ChiragShah.txt", client3Path + "\\" + "ChiragShah.txt", true);

                        this.Invoke(new MethodInvoker(delegate
                        {
                            serverOutput.AppendText("\n>> Updated all Clients with latest files from Server...\n");
                        }));

                        for (int i = 0; i < _allClientSockets.Count; i++)
                        {
                            //if (clientSock.Client.RemoteEndPoint.ToString() != _allClientSockets[i].Client.RemoteEndPoint.ToString())
                            //{
                            this.Invoke(new MethodInvoker(delegate
                            {
                                serverOutput.AppendText("\n>> Sending validated notice for Client: " + _allClientSockets[i].Client.RemoteEndPoint.ToString() + "\n");
                            }));
                            sendServerResponse = Encoding.ASCII.GetBytes("Validated");
                            _allClientSockets[i].Client.Send(sendServerResponse);
                            sendServerResponse = new byte[1024];
                            //}
                        }
                        clientData = new byte[4096];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while updating files at all clients:" + ex.Message);
                    }
                }
                else
                {
                    if (receivedBytesLen > 5)
                    {
                        try
                        {
                            this.Invoke(new MethodInvoker(delegate
                            {
                                serverOutput.AppendText("\n>> Receiving file from Client: " + clientSock.Client.RemoteEndPoint.ToString());
                            }));
                            int fileNameLen = BitConverter.ToInt32(clientData, 0);
                            string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

                            BinaryWriter bWrite = new BinaryWriter(File.Open(receivedPath + "/" + fileName, FileMode.Create));

                            bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                            //closing the write operations..
                            bWrite.Flush();
                            bWrite.Close();
                            this.Invoke(new MethodInvoker(delegate
                            {
                                serverOutput.AppendText("\n>> Client: " + clientSock.Client.RemoteEndPoint.ToString() + "'s file is updated at server.");
                            }));

                            for (int i = 0; i < _allClientSockets.Count; i++)
                            {
                                if (clientSock.Client.RemoteEndPoint.ToString() != _allClientSockets[i].Client.RemoteEndPoint.ToString())
                                {
                                this.Invoke(new MethodInvoker(delegate
                                {
                                    serverOutput.AppendText("\n>> Sending >--Invalidation Notice--< to Client: " + _allClientSockets[i].Client.RemoteEndPoint.ToString());
                                }));
                                sendServerResponse = Encoding.ASCII.GetBytes("Invalidation Notice");
                                _allClientSockets[i].Client.Send(sendServerResponse);
                                sendServerResponse = new byte[1024];
                                }
                            }
                            clientData = new byte[4096];
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error while uploading file at server:" + ex.Message);
                        }
                    }
                }
               
            }
        }
    
    }//end of class

     

}
