using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Envision.Models
{
    public class CompanyRegErr
    {
        public CompanyRegErr()
        {
            c = new Company();
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string WebsiteURL { get; set; }
        public byte[] Logo { get; set; }
        public string About { get; set; }
        public string CompanyType { get; set; }

        public Company c;
    }
}