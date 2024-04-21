using ApiVault.DataModels;
using Cassandra;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class SignUpViewModel : ReactiveObject, IRoutableViewModel
    {
        // Routing interface needed properties.
        // Sets the view identifier to "signUp"
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = "signUp";

        // pre-compile validation Regex
        private static readonly Regex hasNumber = new Regex(@"[0-9]+", RegexOptions.Compiled);
        private static readonly Regex hasSpecialChar = new Regex(@"[!@#$%^&*()_+<>?]+", RegexOptions.Compiled);

        // New user variables for Data Binding
        private string? username;
        public string? Username
        {  get => username;
            set
            {
                this.RaiseAndSetIfChanged(ref username, value);
                UpdateCanSubmit();
            }
        }

        private string? email;
        public string? Email 
        {
            get => email;
            set
            {
                this.RaiseAndSetIfChanged(ref email, value);
                UpdateCanSubmit();
            }

        }

        private string? password;
        public string? Password
        {
            get => password;
            set
            {
                this.RaiseAndSetIfChanged(ref password, value);
                UpdateCanSubmit();
            }
        }

        private string? confirmpassword;
        public string? ConfirmPassword
        {
            get => confirmpassword;
            set
            { 
                this.RaiseAndSetIfChanged(ref confirmpassword, value); 
                UpdateCanSubmit(); 
            }
        }

        private string? phone;
        public string? Phone 
        {
            get => phone;
            set 
            { 
                this.RaiseAndSetIfChanged(ref phone, value);
                UpdateCanSubmit(); 
            }
        }

        private string? statusMessage;
        public string? StatusMessage
        {
            get => statusMessage;
            set => this.RaiseAndSetIfChanged(ref statusMessage, value);
        }

        public bool CanSubmit { get; set; } = false;

        /* - - - - - - - - - - HTTP Client - - - - - - - - - - */
        HttpClient httpClient = new HttpClient();

        private readonly string? astraDbId = Environment.GetEnvironmentVariable("ASTRA_DB_ID");
        private readonly string? astraDbRegion = Environment.GetEnvironmentVariable("ASTRA_DB_REGION");
        private readonly string? astraDbKeyspace = Environment.GetEnvironmentVariable("ASTRA_DB_KEYSPACE");
        private readonly string? astraDbApplicationToken = Environment.GetEnvironmentVariable("ASTRA_DB_APPLICATION_TOKEN");
        private readonly string _table = "users";

        /* - - - - - - - - - - - Commands - - - - - - - - - - - */
        // Command for Sign Up
        public ReactiveCommand<Unit, Unit> SignUpCommand { get; }

        /* - - - - - - - - - - - Constructors - - - - - - - - - - - */
        
        public SignUpViewModel()
        {
            SignUpCommand = ReactiveCommand.CreateFromTask(SignUp);
        }

        public SignUpViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        /* - - - - - - - - - - - Methods - - - - - - - - - - - */

        /*
         * Create new user account
         */
        private async Task SignUp()
        {
            // Check for empty fields
            if (Email != null && Username != null && Password != null && ConfirmPassword != null && Phone != null)
            {
                if (await verifyInput(Email, Username, Password, ConfirmPassword, Phone))
                {
                    // Insert user logic here
                    try
                    {
                        var success = await InsertUser(Email, Username, Password, Phone);
                        if (success)
                        {
                            // TODO: Clear fields after successfull sign up
                            Email = string.Empty;
                            Username = string.Empty;
                            Password = string.Empty;
                            ConfirmPassword = string.Empty;
                            Phone = string.Empty;

                            // Navigate to sign in or update UI accordingly
                            StatusMessage = "User created successfully";
                            Debug.Print("User created successfully");
                        }
                        else
                        {
                            StatusMessage = "Failed to create user";
                            Debug.Print("Failed to create user");
                        }
                    }

                    catch (Exception ex)
                    {
                        Debug.Print($"Sign Up exception: {ex}");
                    }
                }
            }
        }


        /*
         * Insert new user in the database
         */
        private async Task<bool> InsertUser(string email, string username, string password, string phone)
        {   
            // Encrypt password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            if (astraDbId != null && astraDbRegion != null && astraDbKeyspace != null && astraDbApplicationToken != null)
            {
                AstraDbService dbService = new AstraDbService(httpClient, astraDbId, astraDbRegion, astraDbKeyspace, astraDbApplicationToken);

                if (username is not null && hashedPassword is not null && email is not null && phone is not null)
                {
                    try
                    {
                        return await dbService.InsertUser(_table, email, username, hashedPassword, phone);
                    }

                    catch (Exception ex)
                    {
                        Debug.WriteLine($"InsertUser in Model: {ex}");
                    }
                }

                else
                {
                    Debug.WriteLine("Null values in insert user");
                    return false;
                }
            }

            return false;
        }

        // verify username, email, phone number, and password
        private async Task<bool> verifyInput(string Email, string Username, string Password, string ConfirmPassword, string Phone)
        {
            // Check for empty fields
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) ||
                string.IsNullOrEmpty(Phone))
            {
                StatusMessage = "One or more fields are empty";
                Debug.Print("One or more fields are empty");
                return false;
            }

            // check username length 
            if (Username.Length < 4)
            {
                StatusMessage = "Username must have at least 4 characters";
                Debug.Print("Username must have at least 4 characters");
                return false;
            }

            // check username availability
            if (await isUsernameAvailable(Username) == false)
            {
                StatusMessage = $"{Username} is already taken";
                Debug.Print("Username + \" is already taken\"");
                return false;
            }

            // Check for matching passwords
            if (Password != ConfirmPassword)
            {
                StatusMessage = "Passwords are not the same";
                Debug.Print("Passwords are not the same");
                return false;
            }

            // Check for special characters and numbers in passwords
            if (!IsValidPassword(Password))
            {
                StatusMessage = "Password needs at least one number and special character";
                Debug.Print("Password needs at least one number and special characters");
                return false;
            }

            // Check if password has right length ( 8 > 0)
            if (Password.Length < 8)
            {
                StatusMessage = "Password must have at least 8 characters";
                Debug.Print("Password must have at least 8 characters");
                return false;
            }

            // TODO: Check phone number formart
            if (Phone.Length < 7 ||  Phone.Length > 15) 
            {
                StatusMessage = "Invalid phone number";
                Debug.Print("Invalid phone number");
                return false;
            }

            // TODO: Check email format
            if (!IsValidEmail(Email))
            {
                StatusMessage = "Invalid email";
                Debug.Print("Invalid email");
                return false;
            }
            

            return true;
        }

        // Check if password has special characters and numbers
        private bool IsValidPassword(string password)
        { 
            return hasNumber.IsMatch(password) && hasSpecialChar.IsMatch(password);
        }

        // Check username availability
        private async Task<bool> isUsernameAvailable(string username)
        {
            if (astraDbId != null && astraDbRegion != null && astraDbKeyspace != null && astraDbApplicationToken != null)
            {
                AstraDbService dbService = new AstraDbService(httpClient, astraDbId, astraDbRegion, astraDbKeyspace, astraDbApplicationToken);

                return await dbService.isUsernameAvailable(username);
            }

            return false;
        }

        // Verify email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        // Enables user to submit once all form fields are filled
        private void UpdateCanSubmit()
        {
            CanSubmit = !string.IsNullOrWhiteSpace(Username)
                        && !string.IsNullOrWhiteSpace(Email)
                        && !string.IsNullOrWhiteSpace(Password)
                        && !string.IsNullOrWhiteSpace(ConfirmPassword)
                        && !string.IsNullOrWhiteSpace(Phone);

            this.RaisePropertyChanged(nameof(CanSubmit));
        }
    }
}
