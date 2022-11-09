using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace servidor
{
    class Program
    {
        static void Main(string[] args)
        {
            MyWebServer server = new MyWebServer();
            server.Startup();
        }
    }


    public class MyWebServer
    {
        private TcpListener myListener;
        private int port = 5050;

        public void Startup()
        {
            try
            {
                myListener = new TcpListener(port);
                myListener.Start();
                Thread th = new Thread(new ThreadStart(StartListen));
                th.Start();
                Console.WriteLine("localhost:" + port);
            }
            catch
            {
                Console.WriteLine("No se logro establecer servicio web en el puerto: " + port);
            }
        }

        public void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)
        {
            String sBuffer = "";
            if (sMIMEHeader.Length == 0) { sMIMEHeader = "text/html"; }
            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";
            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
            SendToBrowser(bSendData, ref mySocket);
            Console.WriteLine("Total Bytes : " + iTotBytes.ToString());
        }

        public void SendToBrowser(String sData, ref Socket mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }

        public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0;
            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                    {
                        Console.WriteLine("Socket Error cannot Send Packet");
                    }
                    else
                    {
                        Console.WriteLine("No. of bytes send {0}", numBytes);
                    }
                }
                else
                {
                    Console.WriteLine("Connection Dropped....");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occurred : {0} ", e);
            }
        }

        public void StartListen()
        {
            int iStartPos = 0;
            String sRequest;
            String sDirName;
            String sRequestedFile;
            String sErrorMessage;
            String sLocalDir;
            String sMyWebServerRoot = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "WEB" + Path.DirectorySeparatorChar;
            String sPhysicalFilePath = "";
            String sResponse = "";
            //serving = true;
            while (true)
            {
                try
                {
                    Socket mySocket = myListener.AcceptSocket();
                    Console.WriteLine("Socket Type " + mySocket.SocketType);
                    if (mySocket.Connected)
                    {
                        Console.WriteLine("\nClient Connected!!\n==================\nCLient IP {0}\n",mySocket.RemoteEndPoint);
                        Byte[] bReceive = new Byte[1024];
                        int i = mySocket.Receive(bReceive, bReceive.Length, 0);
                        string sBuffer = Encoding.ASCII.GetString(bReceive);
                        if (sBuffer.Substring(0, 3) != "GET")
                        {
                            Console.WriteLine("Solo metodo Get es soportado..");
                            mySocket.Close();
                            continue;
                        }
                        iStartPos = sBuffer.IndexOf("HTTP", 1);
                        string sHttpVersion = sBuffer.Substring(iStartPos, 8);
                        sRequest = sBuffer.Substring(0, iStartPos - 1);
                        sRequest.Replace("\\", "/");
                        if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
                        {
                            sRequest = sRequest + "/";
                        }
                        iStartPos = sRequest.LastIndexOf("/") + 1;
                        sRequestedFile = sRequest.Substring(iStartPos);
                        sDirName = sRequest.Substring(sRequest.IndexOf("/"), sRequest.LastIndexOf("/") - 3);
                        sLocalDir = sMyWebServerRoot;
                        Console.WriteLine("Directory Requested : " + sLocalDir);
                        if (sLocalDir.Length == 0)
                        {
                            sErrorMessage = "<H2>Error!! Directorio no existe</H2><Br>creado por Ricardo Antonio Valladares Renderos";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket);
                            mySocket.Close();
                            continue;
                        }
                        if (sRequestedFile.Length == 0)
                        {
                            sErrorMessage = "<H2>Error!! No hay archivo por default intente con <a href='http://localhost:" + port + "/index.html'>localhost:" + port + "/index.html</a></H2><Br>creado por Ricardo Antonio Valladares Renderos";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket);
                            mySocket.Close();
                            continue;
                        }
                        sPhysicalFilePath = sLocalDir + sRequestedFile;
                        Console.WriteLine("File Requested : " + sPhysicalFilePath);
                        if (File.Exists(sPhysicalFilePath) == false)
                        {
                            sErrorMessage = "<H2>404 Error! Archivo no existe...</H2><Br>creado por Ricardo Antonio Valladares Renderos";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket);
                            mySocket.Close();
                            continue;
                        }
                        else
                        {
                            int iTotBytes = 0;
                            sResponse = "";
                            FileStream fs = new FileStream(sPhysicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            BinaryReader reader = new BinaryReader(fs);
                            byte[] bytes = new byte[fs.Length];
                            int read;
                            while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);
                                iTotBytes = iTotBytes + read;
                            }
                            reader.Close();
                            fs.Close();
                            SendHeader(sHttpVersion, "", iTotBytes, " 200 OK", ref mySocket);
                            SendToBrowser(bytes, ref mySocket);
                        }
                        mySocket.Close();
                    }
                }
                catch { }

            }
        }
    }


}
