using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ScreenCaptureService
{
    internal class ImageServer
    {
        private Thread? _serverThread = null;
        private Timer? _timer = null;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private bool _connectionTaken = false;
        
        public void Start(int port)
        {
            CancellationToken cancelToken = _tokenSource.Token;

            IPAddress ipAddress = new IPAddress(0);

            TcpListener listener = new TcpListener(ipAddress, port);
            _serverThread = new Thread(() => ServerHandler(listener, cancelToken));
            _serverThread.Start();

            Console.WriteLine($"TCP Server started. Listening on port {port}");
        }

        public void Stop()
        {
            if(_serverThread != null)
            {
                Console.WriteLine("Stopping TCP Server.");

                _tokenSource.Cancel();
                _serverThread = null;
            }
        }

        async void ServerHandler(TcpListener listener, CancellationToken cancelToken)
        {
            listener.Start();

            while (true)
            {
                byte[] bytesFrom = new byte[65536];
                string dataFromClient;
                int timerInterval;

                cancelToken.ThrowIfCancellationRequested();

                Console.WriteLine("Waiting for connection.");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                _connectionTaken = true;

                NetworkStream stream = client.GetStream();

                try
                {
                    await stream.ReadAsync(bytesFrom, 0, (int)client.ReceiveBufferSize);

                }
                catch (IOException)
                {
                    Console.WriteLine("Connection to client lost.");
                }
                


                dataFromClient = Encoding.UTF8.GetString(bytesFrom);

                timerInterval = int.Parse(dataFromClient);

                Console.WriteLine($"Client sent following data: {dataFromClient}");


                _timer = new Timer(SendLastScreenshot, stream, 0, timerInterval);

                while (_connectionTaken)
                {
                    continue;
                }
            }
        }

        private byte[] ImageToByte(Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }

        private void SendLastScreenshot(object? state)
        {
            try
            {
                NetworkStream stream = state as NetworkStream; 

                lock (ImageResource.lastScreenshotLock)
                {
                    if (ImageResource.lastScreenshot != null)
                    {
                        byte[] responseBytes = ImageToByte(ImageResource.lastScreenshot);

                        int headerLength = responseBytes.Length;

                        byte[] byteArrHeaderLength = BitConverter.GetBytes(headerLength);

                        stream.Write(byteArrHeaderLength, 0, byteArrHeaderLength.Length);
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }

                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Client disconnected.");
                _timer?.Change(Timeout.Infinite, 0);
                _connectionTaken = false;

                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error. Aborting connection with client.");
                _timer?.Change(Timeout.Infinite, 0);
                _connectionTaken = false;

                return;
            }

        }
    }
}
