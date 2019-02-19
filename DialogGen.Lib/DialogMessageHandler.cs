using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DialogGen.Lib.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

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

    private List<CardAction> PopulateButtonList(string[] optionCollection)
    {
        var optionActions = new List<CardAction>();

        foreach (var optionText in optionCollection)
        {
            optionActions.Add(new CardAction(Microsoft.Bot.Schema.ActionTypes.ImBack, title: optionText, value: optionText));
        }

        return optionActions;
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
}