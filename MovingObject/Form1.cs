// MovingObject -> Form1.cs (SERVER - VERSI BENAR SESUAI KODE ASLI)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
// Ditambahkan untuk jaringan
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MovingObject
{
    public partial class Form1 : Form
    {
        // Variabel dari kode aslimu untuk menggambar
        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        int slide = 10;

        // ======================================================
        // BAGIAN JARINGAN DITAMBAHKAN DI SINI
        // ======================================================
        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();
        private const int PORT = 8888;
        // ======================================================

        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 50;
            timer1.Enabled = true;

            // Memulai server saat form dijalankan
            StartServer();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            back();

            // Menggerakkan object Rectangle
            rect.X += slide;

            // Memaksa form untuk menggambar ulang (memanggil Form1_Paint)
            Invalidate();

            // ======================================================
            // MODIFIKASI: Kirim posisi Rectangle ke semua client
            // ======================================================
            BroadcastPosition(rect.X, rect.Y);
            // ======================================================
        }

        private void back()
        {
            if (rect.X >= this.Width - rect.Width * 2)
                slide = -10;
            else
            if (rect.X <= rect.Width / 2)
                slide = 10;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Menggambar object Rectangle
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }

        // ======================================================
        // SEMUA FUNGSI JARINGAN DITAMBAHKAN DI SINI
        // ======================================================
        private void StartServer()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
                serverSocket.Listen(10);
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server Error: " + ex.Message);
            }
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(AR);
                clientSockets.Add(clientSocket);
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Accept Error: " + ex.Message);
            }
        }

        private void BroadcastPosition(int x, int y)
        {
            byte[] data = Encoding.ASCII.GetBytes($"{x},{y}");
            for (int i = clientSockets.Count - 1; i >= 0; i--)
            {
                try
                {
                    clientSockets[i].BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, clientSockets[i]);
                }
                catch
                {
                    clientSockets[i].Close();
                    clientSockets.RemoveAt(i);
                }
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            try
            {
                Socket client = (Socket)AR.AsyncState;
                client.EndSend(AR);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send Error: " + ex.Message);
            }
        }
        // ======================================================
    }
}