using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class ContactRegErr
    {
        public ContactRegErr()
        {
            c = new Contact();
        }

        public string Email { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Msg { get; set; }

        public Contact c;
    }
}