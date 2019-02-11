using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DialogGen.Lib.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

public static class DialogMessageHandler
{
    public static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken, DialogModel dialogModel)
    {
        await SendMessageAsync(turnContext, cancellationToken, dialogModel.InitMessage);
    }

    public static async Task SendMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken, Message message)
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

    private static List<CardAction> PopulateButtonList(string[] optionCollection)
    {
        var optionActions = new List<CardAction>();

        foreach (var optionText in optionCollection)
        {
            optionActions.Add(new CardAction(Microsoft.Bot.Schema.ActionTypes.ImBack, title: optionText, value: optionText));
        }

        return optionActions;
    }
}