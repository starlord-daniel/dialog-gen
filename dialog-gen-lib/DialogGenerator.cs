using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dialog_gen_lib.model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace dialog_gen_lib
{
    public class DialogGenerator
    {
        DialogModel dialogModel = new DialogModel();

        public DialogGenerator() 
        {

        }

        public DialogGenerator(string jsonDialogStructure) 
        {
            this.dialogModel = JsonConvert.DeserializeObject<DialogModel>(jsonDialogStructure);
        }

        /// <summary>
        /// Handles the bot conversation based on the users previously loaded dialogStrcuture, in JSON format
        /// </summary>
        public async Task HandleBotConversationsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Type is ActivityTypes.Message)
            {   
                await turnContext.SendActivityAsync("test");
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate 
                && turnContext.Activity.MembersAdded.First().Name == "User")
            {
                await DialogMessageHandler.SendWelcomeMessageAsync(turnContext, cancellationToken, dialogModel);
            }
        }

        public Task LoadDialogsJsonAsync(string jsonDialogStructure)
        {
            this.dialogModel = JsonConvert.DeserializeObject<DialogModel>(jsonDialogStructure);
            
            return Task.CompletedTask;
        }

        
    }
}
