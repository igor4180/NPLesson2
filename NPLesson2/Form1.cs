using System.Net;
using System.Net.Sockets;
using System.Text;
using Contacts;
namespace NPLesson2
{
    public partial class Form1 : Form
    {
        IPEndPoint point;
        ClientContacts contact;
        ClientServerCommand command;
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_connectServer_Click(object sender, EventArgs e)
        {
            command.ConnectServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80), contact.Socket);
            Thread thread = Thread.CurrentThread;
            thread.Join(500);
            if (command.ServerIsConnected())
                rtb_chat.Text = "Подключение успешно\n";
            /*try
            {
                
                client.BeginConnect(point, (IAsyncResult result) =>
                {
                    server = (Socket)result.AsyncState;
                    if (server.Connected)
                    {
                        byte[] buffer = new byte[1024];
                        int answerServer = server.Receive(buffer);
                        while (answerServer > 0)
                        {
                            IAsyncResult updateText = rtb_chat.BeginInvoke(RichTextBoxOutputDelegate, Encoding.UTF8.GetString(buffer));
                            rtb_chat.EndInvoke(updateText);
                            answerServer = server.Receive(buffer);
                        }
                    }
                    client.EndConnect(result);

                }, client);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }*/

        }

        private void btn_disconnectServer_Click(object sender, EventArgs e)
        {

        }

        private void btn_choiceContact_Click(object sender, EventArgs e)
        {

        }

        private void btn_sendMessage_Click(object sender, EventArgs e)
        {
            if (command.ServerIsConnected())
                command.SendMessage("Contact|"+contact.ToString());
            /*try
            {
                client = contact.Socket;
                point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
                client.BeginConnect(point, (IAsyncResult result) =>
                {
                    server = (Socket)result.AsyncState;
                    if (server.Connected)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(tb_message.Text);
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
                        client.SendAsync(segment, SocketFlags.None);
                    }
                    client.EndConnect(result);

                }, client);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }*/

        }

        private void tb_message_TextChanged(object sender, EventArgs e)
        {

        }

        private void tmr_refreshConnection_Tick(object sender, EventArgs e)
        {

        }
        void RichTextBoxOutputDelegate(object obj)
        {
            rtb_chat.Text += (string)obj;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            contact = new ClientContacts(
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP),
            "Ilya"+DateTime.Now.Millisecond.ToString(),
            "123456",
            "iluxailuxa@gmail.com",
            "+79287604894"
            );
            command = new ClientServerCommand();
        }
    }


    class ClientServerCommand 
    {
        Socket client { get; set; }
        Socket server { get; set; }

        string _answer = "";
        public bool ConnectServer(IPEndPoint point, Socket clientSocket)
        {
            try
            {       
                client = clientSocket;
                client.BeginConnect(point, (IAsyncResult result) =>
                {
                    server = (Socket)result.AsyncState;
                    if (server.Connected)
                    {
                         
                        byte[] buffer = new byte[1024];
                        int answerServer = server.Receive(buffer);
                        while (answerServer > 0)
                        {
                            _answer += Encoding.UTF8.GetString(buffer);
                            answerServer = server.Receive(buffer);
                        }                        
                    }
                }, client);    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return _answer == "Успешное подключение." ? true : false;
        }
        public void SendMessage(string message)
        {
            if (server.Connected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
                client.SendAsync(segment, SocketFlags.None);
            }
        }
        public bool ServerIsConnected()
        {
            return server != null ? true : false;
        }
    }
}