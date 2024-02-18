using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.DataModels
{
    internal class ApiVaultUser
    {
        // User properties
        private string Name { get; set; } = String.Empty;
        private string Username { get; set; } = String.Empty;
        private string Password { get; set; } = String.Empty;
        private string Email { get; set; } = String.Empty;
        private string Phone { get; set; } = String.Empty;
    }
}
