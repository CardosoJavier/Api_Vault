using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class ConfirmDeleteViewModel : ReactiveObject
    {
        public string Message { get; }

        // Commands
        public ReactiveCommand<Unit, bool> ConfirmCommand { get; }
        public ReactiveCommand<Unit, bool> CancelCommand { get; }

        public ConfirmDeleteViewModel(string message)
        {
            Message = message;

            ConfirmCommand = ReactiveCommand.Create(() => true);
            CancelCommand = ReactiveCommand.Create(() => false);
        }
    }

}
