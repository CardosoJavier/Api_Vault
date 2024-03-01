using ApiVault.DataModels;
using Cassandra;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class SignUpViewModel : ReactiveObject, IRoutableViewModel
    {
        // Routing interface needed properties.
        // Sets the view identifier to "login"
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = "signUp";

        // Constructor
        public SignUpViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        /*
         * Insert new user in the database
         */
        public static async Task InsertUser(string email, string username, string password, string phone)
        {
            // Stablish database connection
            var session = await AstraDbConnection.Connect();

            // New user query
            var inserteUser = session.Prepare("INSERT INTO apivault_space.Users (username, email, password, phone) VALUES (?, ?, ?, ?)");
            session.Execute(inserteUser.Bind(username, email, password, phone));
        }



    }
}
