using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServer.Server
{
    public class NetScene<Client> where Client : NetClient
    {
        public string Name { get; set; }
    }
}
