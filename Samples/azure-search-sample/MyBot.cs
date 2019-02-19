using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Options;
using Microsoft.Bot.Builder.Integration;
using System.Collections.Generic;
using System.IO;

// For use of DialogGen library
using DialogGen.Lib;
using DialogGen.Lib.Accessors;
using DialogGen.Lib.States;

namespace azure_search_sample
{
    public class MyBot : IBot
    {
        #region Bot Variable Setup

        private readonly DialogLibAccessors _accessors;
        private readonly DialogGenerator _dialogGenerator;

        public MyBot(DialogLibAccessors accessors, DialogGenerator dialogGenerator)
        {
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
            _dialogGenerator = dialogGenerator ?? throw new System.ArgumentNullException(nameof(dialogGenerator));
        }

        #endregion

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle the dialog that is build in JSON
            await _dialogGenerator.HandleBotConversationsAsync(turnContext, cancellationToken);
        }
    }
}