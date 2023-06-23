using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberArk_Password_Retrival
{
   

    public class VaultDetailsResponse
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }

        public string Database { get; set; }
    }
}
