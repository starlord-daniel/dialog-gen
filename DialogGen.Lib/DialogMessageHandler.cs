using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DialogGen.Lib.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

public class DialogMessageHandler
{
    DialogModel _dialogModel;

    public DialogMessageHandler(DialogModel dialogModel)
    {
        this._dialogModel = dialogModel;
    }

    public async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        await SendMessageAsync(turnContext, cancellationToken, _dialogModel.InitMessage);
    }

    public async Task SendQnaMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken, QnaResponse qnaResponse)
    {
        var qnaResponseText = qnaResponse.Answers.First().Text;

        await SendMessageAsync(turnContext, cancellationToken, new Message {
            Text = qnaResponseText
        });
    }

    public async Task SendMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken, Message message)
    {
        try
        {
            var reply = turnContext.Activity.CreateReply();

            if(message.Options == null || message.Options.Length == 0)
            {
                reply.Text = message.Text;
            }
            else 
            {
                var replyButtons = PopulateButtonList(message.Options);

                // Create a HeroCard with options for the user to choose to interact with the bot.
                var card = new HeroCard
                {
                    Text = message.Text,
                    Buttons = replyButtons
                };

                reply.Attachments = new List<Attachment>() { card.ToAttachment() };
            }

            await turnContext.SendActivityAsync(reply);
        }
        catch(Exception e)
        {
            throw new Exception("[DialogMessageHandler] Error while sending a message", e);
        }
    }

    public async Task SendInitialMessage(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        try
        {
            var message = _dialogModel.InitMessage;
            await SendMessageAsync(turnContext, cancellationToken, message);
        }
        catch (System.Exception e) 
        {
            throw new Exception("[DialogMessageHandler] Please check the state of your initial message.",e);
        }
    }

    public async Task SendDefaultMessage(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        try
        {
            var message = _dialogModel.DefaultMessage;
            await SendMessageAsync(turnContext, cancellationToken, message);
        }
        catch (System.Exception e) 
        {
            throw new Exception("[DialogMessageHandler] Please check the state of your default message.",e);
        }   
    }

    public async Task SendAzureSearchResultAsync(ITurnContext turnContext, CancellationToken cancellationToken, 
        MessageMapping messageMapping, string azureSearchResultJson)
    {
        Activity replyActivity = CreateReplyFromAzureSearchMapping(turnContext, messageMapping, azureSearchResultJson);

        await turnContext.SendActivityAsync(replyActivity);
    }

    private Activity CreateReplyFromAzureSearchMapping(ITurnContext turnContext, MessageMapping messageMapping, string azureSearchResultJson)
    {
        try
        {
            // Parse Json into AzureSearchResult structure 
            JObject azureSearchResult = JObject.Parse(azureSearchResultJson);

            // select all returned values of the AzureSearchResult
            JArray searchResultArray = (JArray)azureSearchResult.SelectToken("value");

            var reply = turnContext.Activity.CreateReply();
            reply.Attachments = new List<Attachment>();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Text = messageMapping.Text;

            // Go through all search results
            foreach (JObject searchResult in searchResultArray.Children<JObject>())
            {
                // Create Image List
                var imageList = new List<CardImage>();

                // Get the value for the imageUrlPlaceholder from the mapping
                var imageUrlPlaceholder = messageMapping.Card.ImageUrl;
                
                // If the value exists, replace it with the value from the AzureSearchResult
                if(String.IsNullOrEmpty(imageUrlPlaceholder))
                {
                    // Replace all occurences of searchResult properties with their respective values
                    var imageUrl = MapSearchResultsToString(imageUrlPlaceholder, searchResult);
                    
                    // Add the image to the list
                    imageList.Add(
                        new CardImage{Url = imageUrl}
                    );
                }

                // Create Button List
                var buttonList = new List<CardAction>();
                
                foreach (var button in messageMapping.Card.Options)
                {
                    buttonList.Add(new CardAction(
                        Microsoft.Bot.Schema.ActionTypes.ImBack, 
                        title: button, 
                        value: button
                    ));
                }

                var card = new HeroCard
                {
                    Images = imageList,
                    Text = MapSearchResultsToString(messageMapping.Card.Text, searchResult),
                    Title = MapSearchResultsToString(messageMapping.Card.Title, searchResult),
                    Subtitle = MapSearchResultsToString(messageMapping.Card.Subtitle, searchResult),
                    Buttons = buttonList
                };

                // Add the card to our reply.
                reply.Attachments.Add(card.ToAttachment());                
            }

            return reply;
        }
        catch(Exception e)
        {
            throw new Exception("[DialogMessageHandler] Error while mapping AzureSearchResult to Message format.", e);
        }
    }
    private List<CardAction> PopulateButtonList(string[] optionCollection)
    {
        var optionActions = new List<CardAction>();

        foreach (var optionText in optionCollection)
        {
            optionActions.Add(new CardAction(Microsoft.Bot.Schema.ActionTypes.ImBack, title: optionText, value: optionText));
        }

        return optionActions;
    }

    private string MapSearchResultsToString(string placeholder, JObject searchResult)
    {
        var filledString = placeholder;

        foreach (var property in searchResult.Properties())
        {
            var key = property.Name;
            var value = property.Value.ToString();

            filledString.Replace("{" + key + "}", value);
        }

        return filledString;
    }
}