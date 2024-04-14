using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.DataModels
{
    internal class ApiUser
    {
        public required string username { get; set; }
        public required string password { get; set; }
        public required string email { get; set; }
        public required string phone { get; set; }
    }
}
