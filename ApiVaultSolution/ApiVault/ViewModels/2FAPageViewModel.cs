using ApiVault.Services;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
namespace ApiVault.ViewModels
{
    internal class _2FAPageViewModel: ViewModelBase
    {

        public event EventHandler? LoginSuccessful;
        private readonly IUserSessionService _userSessionService;
        private string? _2faCode;
        private readonly string? _SID = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        private readonly string? _AUTH = Environment.GetEnvironmentVariable("TWILIO_AUTH");
        private readonly string? _SERVICE_ID = Environment.GetEnvironmentVariable("TWILIO_SERVICE_ID");


        private string? user2FAInput;
        public string? User2FAInput
        {
            get => user2FAInput;
            set
            {
                this.RaiseAndSetIfChanged(ref user2FAInput, value);
            }
        }

        private string? statusMessage;
        public string? StatusMessage
        {
            get => statusMessage;
            set => this.RaiseAndSetIfChanged(ref statusMessage, value);
        }

        public ReactiveCommand<Unit, Unit> VerifyCode { get; }

        public _2FAPageViewModel(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
            VerifyCode = ReactiveCommand.CreateFromTask(Verify2FACode);
            Send2FACode();
        }

        // Send text message to user for 2FA
        private void Send2FACode()
        {

            var accountSid = _SID;
            var authToken = _AUTH;
            string fullPhone = string.Concat("+1", _userSessionService.Phone);

            TwilioClient.Init(accountSid, authToken);

            var verification = VerificationResource.Create(
                to: fullPhone,
                channel: "sms",
                pathServiceSid: _SERVICE_ID
            );

        }


        private async Task Verify2FACode()
        {
            Debug.WriteLine($"User input: {User2FAInput}");

            try
            {
                var accountSid = _SID;
                var authToken = _AUTH;

                TwilioClient.Init(accountSid, authToken);

                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: $"+1{_userSessionService.Phone}",
                    code: User2FAInput,
                    pathServiceSid: _SERVICE_ID
                );

                if (verificationCheck.Status == "approved")
                {
                    Debug.WriteLine("2FA code is correct.");
                    OnLoginSuccess();
                }

                else
                {
                    StatusMessage = "2FA code is incorrect.";
                    Debug.WriteLine("2FA code is incorrect.");
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while verifying 2FA code: {ex.Message}");
                StatusMessage = "2FA code is incorrect.";
            }
        }

        private void OnLoginSuccess()
        {
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }

    }
}
