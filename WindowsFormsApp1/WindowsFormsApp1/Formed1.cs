// Formed1.cs (CLIENT)
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WindowsFormsApp1 // Pastikan namespace ini sesuai
{
    public partial class Formed1 : Form
    {
        // Variabel UI tidak perlu dideklarasikan di sini karena sudah ada di Formed1.Designer.cs

        // Komponen Jaringan
        private Socket clientSocket;
        private byte[] buffer = new byte[1024];
        private const int PORT = 8888;
        private const string SERVER_IP = "127.0.0.1";

        public Formed1()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(SERVER_IP), PORT), ConnectCallback, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message);
            }
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket.EndConnect(AR);
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to server: " + ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int received = clientSocket.EndReceive(AR);
                if (received > 0)
                {
                    string positionData = Encoding.ASCII.GetString(buffer, 0, received);
                    this.Invoke((MethodInvoker)delegate {
                        UpdatePosition(positionData);
                    });
                }
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch
            {
                clientSocket.Close();
            }
        }

        private void UpdatePosition(string positionData)
        {
            try
            {
                string[] parts = positionData.Split(',');
                if (parts.Length == 2)
                {
                    int x = int.Parse(parts[0]);
                    int y = int.Parse(parts[1]);

                    // Kita asumsikan di file Designer ada PictureBox bernama 'pictureBox1'
                    if (this.pictureBox1 != null)
                    {
                        pictureBox1.Location = new Point(x, y);
                    }
                }
            }
            catch
            {
                // Abaikan jika format data salah
            }
        }
    }
}