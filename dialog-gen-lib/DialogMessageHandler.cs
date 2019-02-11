using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using dialog_gen_lib.model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

public static class DialogMessageHandler
{
    public static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken, DialogModel dialogModel)
    {
        try
        {
            var reply = turnContext.Activity.CreateReply();

            var replyButtons = PopulateButtonList(dialogModel.InitMessage.Options);

            // Create a HeroCard with options for the user to choose to interact with the bot.
            var card = new HeroCard
            {
                Text = dialogModel.InitMessage.Text,
                Buttons = replyButtons
            };

            // Add the card to our reply.
            reply.Attachments = new List<Attachment>() { card.ToAttachment() };
            
            await turnContext.SendActivityAsync(reply);
        }
        catch(Exception e)
        {
            throw new Exception("[DialogMessageHandler] Error while sending the welcome message", e);
        }
    }

    private static List<CardAction> PopulateButtonList(string[] optionCollection)
    {
        var optionActions = new List<CardAction>();

        foreach (var optionText in optionCollection)
        {
            optionActions.Add(new CardAction(ActionTypes.ImBack, title: optionText, value: optionText));
        }

        return optionActions;
    }
    
}