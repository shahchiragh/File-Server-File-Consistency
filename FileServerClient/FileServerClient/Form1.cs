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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Permissions;

namespace FileServerClient
{
    public partial class fileSeverClientForm : Form
    {
        IPEndPoint ipEnd = null;
        Socket clientSock = null;
        public IPAddress ipAddress1 = null;
        public static string pullingPath = "E:\\FT";
        public static string clientDirectory = "E:\\Client2-FT1";

        public static string client1Path = "E:\\Client1-FT";
        public static string client2Path = "E:\\Client2-FT";
        public static string client3Path = "E:\\Client3-FT";

        public static string pushingFile = clientDirectory+"\\"+"ChiragShah.txt";

        static byte[] buffer = new byte[4096];
        // Create a new FileSystemWatcher and set its properties.
        FileSystemWatcher watcher;

        public fileSeverClientForm()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (portNumber.Text != "" )
            {
                messageText.AppendText(">> Trying to connect to the server...");
                IPHostEntry ipHostInfo = Dns.Resolve("localhost");
                ipAddress1 = ipHostInfo.AddressList[0];
                ipEnd = new IPEndPoint(ipAddress1, Int32.Parse(portNumber.Text));
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSock.Connect(ipEnd);
                messageText.AppendText("\n>> Connected to the server with local directory: " + clientDirectory);
                clientSock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSock);
                //uploadFile(pushingFile);
                checkFileInconsistency();

            }
            else
            {
                MessageBox.Show("Please enter port number to connect to server.");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            Socket ts = (Socket)result.AsyncState;
            ts.EndReceive(result);
            string serverMessage = Encoding.ASCII.GetString(buffer);
            result.AsyncWaitHandle.Close();
            //Clear the data, start asynchronous receiver
            buffer = new byte[buffer.Length];
            ts.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
            this.Invoke(new MethodInvoker(delegate
            {
                messageText.AppendText("\n>> Server says: "+ serverMessage);
            }));
            if (serverMessage.Contains("Invalidation Notice"))
            {
                stopWatcher();
                this.Invoke(new MethodInvoker(delegate
                {
                    messageText.AppendText("\n>> Need to Pull latest files from Server");
                    //pullConsistentFiles();
                }));
                File.Copy("E:\\FT\\ChiragShah.txt", pushingFile, true);
                this.Invoke(new MethodInvoker(delegate
                {
                    messageText.AppendText("\n>> Local Copy at " + clientDirectory+" is now updated and consitent.");
                }));

                checkFileInconsistency();
            }
            //if (serverMessage.Contains("Validated"))
            //{
            //    checkFileInconsistency();
            //}
        }


        private void uploadFile(string fileName)
        {
            try
            {
                string filePath = "";
                fileName = fileName.Replace("\\", "/");
                while (fileName.IndexOf("/") > -1)
                {
                    filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                }

                byte[] fileNameByte = new byte[4096];
                try
                {
                    fileNameByte = Encoding.ASCII.GetBytes(fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Idhar jata he bc.." + ex.Message);
                }
                //Console.WriteLine("getting" + fileNameByte);
                if (fileNameByte.Length > 850 * 1024)
                {
                    messageText.Text = "\nFile size is more than 850kb, please try with small file.";
                    return;
                }
                //messageText.AppendText("Buffering ...\n");

                byte[] fileData = new byte[4096];
                try
                {
                    //fileData = File.ReadAllBytes(filePath + fileName);
                    using (System.IO.FileStream fs = System.IO.File.Open(filePath + fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                    {
                        int numBytesToRead = Convert.ToInt32(fs.Length);
                        fileData = new byte[(numBytesToRead)];
                        fs.Read(fileData, 0, numBytesToRead);
                    }
                }
                catch (Exception ex)
                {
                      MessageBox.Show("I am stucker manr.."+ex.Message);
                }

                byte[] clientData = new byte[2 + 4 + fileNameByte.Length + fileData.Length];
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

                try
                {
                    fileNameLen.CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Some kind of error.."+ex.Message);
                }

                //messageText.AppendText("\n File being pushedsending...\n");
                clientSock.Send(clientData);
                messageText.AppendText("\n>> File updated at the server!\n");

                clientData = new byte[4096];
                fileNameByte = new byte[4096];
                fileData = new byte[4096];
                fileNameLen = new byte[4096];

            }
            catch (Exception ex)
            {
                if (ex.Message == "No connection could be made because the target machine actively refused it")
                    messageText.Text = "File Sending fail. Because server not running.\n";
                else
                    messageText.Text = "File Sending fail.\n" + ex.Message;
            }

        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            messageText.AppendText("\n>> Disconnecting client from the server...\n");
            clientSock.Disconnect(false);
        }

        #region Old Functions
        private void uploadFile_Click(object sender, EventArgs e)
        {
            FileDialog fDg = new OpenFileDialog();

            if (fDg.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(fDg.FileName);
                uploadFile(fDg.FileName);
            }
        }

        private void checkFile_Click(object sender, EventArgs e)
        {
            try
            {
                //now here we make request to server to send us the files which it has..
                byte[] clientData = new byte[1024];
                string opertionName = "GET";
                clientData = Encoding.ASCII.GetBytes(opertionName);
                clientSock.Send(clientData);
                byte[] serverData = new byte[1024];
                int receivedBytesLen = clientSock.Receive(serverData);
                string responseName = Encoding.ASCII.GetString(serverData, 0, receivedBytesLen);
                // Console.WriteLine("I am getting some response..:" + responseName);
                messageText.AppendText("---> Available files on server are: <---\n");
                string[] fileNames = responseName.Split(':');
                foreach (string name in fileNames)
                {
                    messageText.AppendText(name + "\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while checking files at server.." + ex.Message);
            }

        }

        private void downloadFile_Click(object sender, EventArgs e)
        {
            if (downloadFileTB.Text != "")
            {
                try
                {
                    string filename = "ChiragShah.txt";
                    byte[] clientData = new byte[1024];
                    string opertionName = "DOWNLOAD" + filename;
                    clientData = Encoding.ASCII.GetBytes(opertionName);
                    clientSock.Send(clientData);

                    byte[] requestedData = new byte[1024 * 5000];
                    int receivedBytesLen = clientSock.Receive(clientData);

                    int fileNameLen = BitConverter.ToInt32(clientData, 0);
                    string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

                    BinaryWriter bWrite = new BinaryWriter(File.Open(client1Path + "/" + fileName, FileMode.Create));
                    bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                    BinaryWriter bWrite2 = new BinaryWriter(File.Open(client1Path + "/" + fileName, FileMode.Create));
                    bWrite2.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                    BinaryWriter bWrite3 = new BinaryWriter(File.Open(client1Path + "/" + fileName, FileMode.Create));
                    bWrite3.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                    //closing the write operations..
                    bWrite.Close();
                    messageText.AppendText("Downloaded file to " + pullingPath + "\n");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while downloading file at server." + ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Enter File Name which you want to download from available files..");
                try
                {
                    byte[] clientData = new byte[1024];
                    string opertionName = "CHECK";
                    clientData = Encoding.ASCII.GetBytes(opertionName);
                    clientSock.Send(clientData);
                    byte[] serverData = new byte[1024];
                    int receivedBytesLen = clientSock.Receive(serverData);
                    string responseName = Encoding.ASCII.GetString(serverData, 0, receivedBytesLen);
                    // Console.WriteLine("I am getting some response..:" + responseName);
                    messageText.AppendText("---> Available files on server are: <---\n");
                    string[] fileNames = responseName.Split(':');
                    foreach (string name in fileNames)
                    {
                        messageText.AppendText(name + "\n");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while showing available files at server." + ex.Message);
                }
            }
        } 
        #endregion

        //This function check the inconsitent files at the client level and updates server if server detects changes..
        bool let = false;
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void checkFileInconsistency()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = clientDirectory;
            /* Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories. */
           // watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.NotifyFilter =  NotifyFilters.LastWrite | NotifyFilters.FileName ;
            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            //Just monitoring the change events..
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }
        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed...
            try
            {
                 this.Invoke(new MethodInvoker(delegate
                {
                    if (let == false)
                    {
                        messageText.AppendText("\n>> File:" + e.FullPath + " has been " + e.ChangeType.ToString().ToLower());
                        messageText.AppendText("\n>> Pushing changed file to server.");
                        uploadFile(pushingFile);
                        let = true;
                    }
                    else
                    {
                        let = false;
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception when triggered onchange :"+ex.Message);
            }
        }

        public void stopWatcher()
        {
           // watcher.EnableRaisingEvents = false;
            watcher.Dispose();

        }

        private void pullConsistentFiles()
        {
            try
            {
                byte[] clientData = new byte[1024];
                clientData = Encoding.ASCII.GetBytes("PULL");
                clientSock.Send(clientData);
                clientData = new byte[1024];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating files at Clients." + ex.Message);
            }
        }

    }


}

