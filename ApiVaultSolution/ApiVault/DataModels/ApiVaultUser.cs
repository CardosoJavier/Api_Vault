using System;

namespace ApiVault.DataModels
{
    internal class ApiVaultUser
    {
        // User properties
        private string Username { get; set; } = String.Empty;
        private string Password { get; set; } = String.Empty;
        private string Email { get; set; } = String.Empty;
        private string Phone { get; set; } = String.Empty;
    }
}
