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
using dialog_gen_lib;
using System.IO;

namespace bot_cs
{
    public class MyBot : IBot
    {
        DialogGenerator dialogGenerator = new DialogGenerator();

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // load the JSON dialog
            await InitDialogGenerator("generator/simpleDialog.json");

            // Handle the dialog that is build in JSON
            await dialogGenerator.HandleBotConversationsAsync(turnContext, cancellationToken);
        }

        private async Task InitDialogGenerator(string fileLocation)
        {
            using (StreamReader r = new StreamReader(fileLocation))
            {
                string json = await r.ReadToEndAsync();
                await dialogGenerator.LoadDialogsJsonAsync(json);
            }
                        
        }

    }
}