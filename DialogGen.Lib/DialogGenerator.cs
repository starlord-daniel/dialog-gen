using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DialogGen.Lib.Model;
using DialogGen.Lib.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace DialogGen.Lib
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

        public Task LoadDialogsJsonAsync(string jsonDialogStructure)
        {
            this.dialogModel = JsonConvert.DeserializeObject<DialogModel>(jsonDialogStructure);
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the bot conversation based on the users previously loaded dialogStrcuture, in JSON format
        /// </summary>
        public async Task HandleBotConversationsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Type is ActivityTypes.Message)
            {   
                await HandleTriggersAsync(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate 
                && turnContext.Activity.MembersAdded.First().Name == "User")
            {
                await DialogMessageHandler.SendWelcomeMessageAsync(turnContext, cancellationToken, dialogModel);
            }
        }

        private async Task HandleTriggersAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // The list of all actions available
            List<Model.Action> triggerList = this.dialogModel.Actions.ToList();
            
            // The users input
            string userInput = turnContext.Activity.Text;

            foreach (var trigger in triggerList)
            {
                if(trigger.Trigger.ToLower() == userInput.ToLower())
                {
                    await PerformActionListAsync(turnContext, cancellationToken, trigger.TriggerActions.ToList());
                    return;
                }
            }

            // If there is no match, display the default message
            await DialogMessageHandler.SendMessageAsync(turnContext, cancellationToken, this.dialogModel.DefaultMessage);
        }

        private async Task PerformActionListAsync(ITurnContext turnContext, CancellationToken cancellationToken, 
            List<TriggerAction> actionList)
        {
            foreach (var action in actionList)
            {
                if(action.Type == Model.ActionTypes.SendMessage)
                {
                    try
                    {
                        var message = this.dialogModel.Messages.Where(x => x.Id == action.MessageId).Select(x => x).First();

                        await DialogMessageHandler.SendMessageAsync(turnContext, cancellationToken, message);
                    }
                    catch (System.Exception e) 
                    {
                        throw new Exception("[DialogGenerator] Please check, if your messageIds match the actions.",e);
                    }
                }
                else if(action.Type == Model.ActionTypes.SendInitMessage)
                {
                    try
                    {
                        var message = this.dialogModel.InitMessage;

                        await DialogMessageHandler.SendMessageAsync(turnContext, cancellationToken, message);
                    }
                    catch (System.Exception e) 
                    {
                        throw new Exception("[DialogGenerator] Please check the state of your initial message.",e);
                    }
                }
                else if(action.Type == Model.ActionTypes.SendQnaMessage)
                {
                    try
                    {
                        var userInput = turnContext.Activity.Text;

                        QnaResponse qnaResponse = await QnaMakerService.GetQnaResultAsync(userInput);

                        await DialogMessageHandler.SendQnaMessageAsync(turnContext, cancellationToken, qnaResponse);
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception("[DialogGenerator] Qna Message failed to send properly.", e);
                    }
                }
                else
                {
                    throw new NotImplementedException("You can't do anything else than the provided ActionTypes yet.");
                }
            }
        }
        
    }
}
