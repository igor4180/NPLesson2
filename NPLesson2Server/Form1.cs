using System.Net;
using System.Net.Sockets;
using System.Text;
using Contacts;

namespace NPLesson2Server
{
    public partial class Form1 : Form
    {
        Socket server;
        IPEndPoint point;
        List<Socket> clientSockets = new List<Socket>();
        List<ClientContacts> contacts = new List<ClientContacts>();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
        }

        private void btn_startServer_Click(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.Bind(point);
                server.Listen(100);
                tmr_refreshConnection.Start();
            }
        }

        private void btn_stopServer_Click(object sender, EventArgs e)
        {
            try
            {
                if (server != null)
                    tmr_refreshConnection.Stop();
                server.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void tmr_refreshConnection_Tick(object sender, EventArgs e)
        {
            try
            {
                server.BeginAccept(ServerAcceptDelegate, server);
                byte[] buffer = new byte[1024];
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
                foreach (Socket client in clientSockets)
                {
                    Task<int> answer = client.ReceiveAsync(segment, SocketFlags.None);
                    if (answer.IsCompleted)
                    {
                         string text = Encoding.UTF8.GetString(segment);
                        IAsyncResult updateText = rtb_clients.BeginInvoke(RichTextBoxOutputDelegate, text);
                        rtb_clients.EndInvoke(updateText);
                        if (text.Contains("Contact"))
                        {
                            string[] contactStrings = text.Split("|");
                            contacts.Add(new ClientContacts(client, contactStrings[1], contactStrings[3], contactStrings[2], contactStrings[4]));                           
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                server.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void ServerAcceptDelegate(IAsyncResult result)
        {
            if (result != null)
            {
                Socket serv = (Socket)result.AsyncState;
                if (serv != null) { 
                    Socket clientsocket = serv.EndAccept(result);
                    clientSockets.Add(clientsocket);
                    clientsocket.Send(Encoding.UTF8.GetBytes("Успешное подключение."));

                    IAsyncResult updateText = rtb_clients.BeginInvoke(RichTextBoxOutputDelegate, clientsocket.RemoteEndPoint.ToString());
                    rtb_clients.EndInvoke(updateText);
                }

            }
        }

        void RichTextBoxOutputDelegate(object obj)
        {
            rtb_clients.Text += (string)obj;
        }

        private void btn_updateClientsList_Click(object sender, EventArgs e)
        {
            foreach (ClientContacts client in contacts)
            {
                rtb_clients.Text += client.ToString();
            }
        }
    }
}