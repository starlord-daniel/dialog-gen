using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DialogGen.Lib.Accessors;
using DialogGen.Lib.Model;
using DialogGen.Lib.Services;
using DialogGen.Lib.States;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DialogGen.Lib
{
    public class DialogGenerator
    {
        DialogModel dialogModel = new DialogModel();

        TopicState _topicState;

        UserProfile _userProfile;

        #region Constructors

        public DialogGenerator() 
        {

        }

        public DialogGenerator(string jsonDialogStructure) 
        {
            this.dialogModel = JsonConvert.DeserializeObject<DialogModel>(jsonDialogStructure);
        }

        #endregion

        #region Initialize Dialog Generator
        
        public void InitializeDialogGenerator(IServiceCollection services, string fileLocation)
        {
            services.AddSingleton<DialogGenerator>(sp =>
            {
                var dialogGenerator = new DialogGenerator();

                using (StreamReader r = new StreamReader(fileLocation))
                {
                    string json = r.ReadToEnd();
                    dialogGenerator.LoadDialogsJson(json);
                }

                return dialogGenerator;
            });

            services.AddSingleton<DialogLibAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null)
                {
                    throw new InvalidOperationException("UserState must be defined and added before adding user-scoped state accessors.");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding user-scoped state accessors.");
                }

                var accessors = new DialogLibAccessors(conversationState, userState)
                {
                    TopicState = conversationState.CreateProperty<TopicState>("TopicState"),
                    UserProfile = userState.CreateProperty<UserProfile>("UserProfile"),
                };

                return accessors;
            });
        }

        private void LoadDialogsJson(string jsonDialogStructure)
        {
            this.dialogModel = JsonConvert.DeserializeObject<DialogModel>(jsonDialogStructure);
        }

        #endregion


        /// <summary>
        /// Handles the bot conversation based on the users previously loaded dialogStrcuture, in JSON format
        /// </summary>
        public async Task HandleBotConversationsAsync(ITurnContext turnContext, CancellationToken cancellationToken, DialogLibAccessors accessors = null)
        {
            if(accessors != null)
            {
                // Get the topic state from the turn context.
                _topicState = await accessors?.TopicState?.GetAsync(turnContext, () => new TopicState());
            
                // Get the user profile (with feedback variables) from the turn context.
                _userProfile = await accessors?.UserProfile?.GetAsync(turnContext, () => new UserProfile());
            }

            if (turnContext.Activity.Type is ActivityTypes.Message)
            {   
                await HandleTriggersAsync(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate 
                && turnContext.Activity.MembersAdded.First().Name == "User")
            {
                await DialogMessageHandler.SendWelcomeMessageAsync(turnContext, cancellationToken, dialogModel);
            }

            if(accessors != null)
            {
                await accessors.TopicState.SetAsync(turnContext, _topicState);
                await accessors.ConversationState.SaveChangesAsync(turnContext);

                await accessors.UserProfile.SetAsync(turnContext, _userProfile);
                await accessors.UserState.SaveChangesAsync(turnContext);
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
                // check for dialog state matches (trigger state is optional, so check for null)
                if(trigger?.TriggerState?.Equals("default") == false 
                    && trigger?.TriggerState?.Equals(_topicState.TopicStateStrings["dialogState"]) == true)
                {
                    await PerformActionListAsync(turnContext, cancellationToken, trigger.TriggerActions.ToList());
                    return;
                }
                
                // Check for instant matches
                if(trigger?.Trigger?.ToLower() == userInput.ToLower())
                {
                    await PerformActionListAsync(turnContext, cancellationToken, trigger.TriggerActions.ToList());
                    return;
                }

                // check for special matches
                if(trigger?.Trigger?.Trim() == "")
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
                var userInput = turnContext.Activity.Text;

                if(action.Type == Model.ActionTypes.SendMessage)
                {
                    try
                    {
                        var message = this.dialogModel.Messages.Where(x => x.Id == action.MessageId).Select(x => x).First();
                        message.Text = message.Text.Replace("{input}", userInput);
                        
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
                        QnaResponse qnaResponse = await QnaMakerService.GetQnaResultAsync(
                            userInput,
                            dialogModel.QnaSettings.Host,
                            dialogModel.QnaSettings.Route,
                            dialogModel.QnaSettings.EndpointKey);

                        await DialogMessageHandler.SendQnaMessageAsync(turnContext, cancellationToken, qnaResponse);
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception("[DialogGenerator] Qna Message failed to send properly.", e);
                    }
                }
                else if(action.Type == Model.ActionTypes.SendLuisMessage)
                {
                    try
                    {
                        LuisResponse luisResponse = await LuisService.GetLuisResultAsync(
                            userInput,
                            dialogModel.LuisSettings.Region,
                            dialogModel.LuisSettings.AppId,
                            dialogModel.LuisSettings.SubscriptionKey,
                            dialogModel.LuisSettings.Threshold
                        );

                        // Send the message specified in the dialog structure
                        var message = this.dialogModel.Messages.Where(x => x.Id == luisResponse.TopScoringIntent.IntentIntent).First();
                        message.Text = ReplaceWithLuisEntities(message.Text, luisResponse);

                        await DialogMessageHandler.SendMessageAsync(turnContext, cancellationToken, message);
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception("[DialogGenerator] LUIS Message failed to send properly.", e);
                    }
                }
                else if(action.Type == Model.ActionTypes.StoreState)
                {
                    try
                    {
                        _topicState.TopicStateStrings["dialogState"] = action.Value;
                    }
                    catch (System.Exception e)
                    {
                        throw new Exception("[DialogGenerator] State could not be stored properly.", e);
                    }
                }
                else
                {
                    throw new NotImplementedException("You can't do anything else than the provided ActionTypes yet.");
                }
            }
        }

        private string ReplaceByPattern(string textToUpdate, string newText, string oldText = "{input}")
        {
            // Only replace input so far
            return textToUpdate.Replace(oldText, newText);
        }

        /// <summary>
        /// Replaces all entity types that were found in the message construct by their values from the LuisResponse
        /// </summary>
        /// <param name="textToUpdate">The message text that should be updated. This needs to include the entity types in the format {entityType}.</param>
        /// <param name="luisReponse">The LuisResponse that is used to replace the entity values from the message.</param>
        private string ReplaceWithLuisEntities(string textToUpdate, LuisResponse luisResponse)
        {
            var newText = textToUpdate;

            // TODO: What to do, when a entity is included in message, but not found in LuisResponse? 

            foreach (var entity in luisResponse.Entities)
            {
                newText = newText.Replace("{" + entity.Type + "}", entity.EntityEntity);
            }
            
            // Only replace input so far
            return newText;
        }
        
    }
}
