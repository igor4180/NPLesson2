using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class ClientContacts
    {
        public Socket Socket { get; set; }
        public string Name { get; set; }
        string _password;
        string _email;
        string _phone;
        public ClientContacts(Socket socket, string name, string password, string email, string phone)
        {
            Socket = socket;
            Name = name;
            _password = password;
            _email = email;
            _phone = phone;
        }

        public override string ToString()
        {
            return Name + "|" + _email + "|" + _password + "|" + _phone;
        }
    }
}
