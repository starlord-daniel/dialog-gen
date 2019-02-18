# Sample: Simple Prompt Dialog

This sample is a good starting point for learning more about the library. It introduces the action, trigger and message structure of the JSON. 

## What you can do with it

The simple prompt allows to create a JSON that handles simple conversations, that follow a call and response mechanism. Only if the user input matches one of the text in the trigger variables, the including actions are performed. 

Every **message** in this sample consists of at minimum 2 components: The **id** and **text** fields. These specify the id, that is referenced in the calling action and the text, that will be displayed to the user. The **options** list can be used to render buttons with the message, that a user can choose from. This helps to guide the user, but is completely optional.

An **action** in this sample includes 2 components: a **trigger** - which is a phrase that needs to match the user input in order to perform the **triggerActions** - which is a list of predefined actions the bot can perform. This sample includes the options **"sendMessage"** and **"sendInitialMessage"**.

## Sample dialog structure JSON

```json
{
    "initMessage": {
        "id": "initMessage",
        "text": "Hello, I'm a simple prompt bot to showcase the dialog generator library ;-)",
        "options": [
            "Welcome",
            "Bye"
        ]
    },
    "actions": [
        {
            "trigger": "Welcome",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "welcomeMessage1"
                },
                {
                    "type": "sendInitMessage",
                    "messageId": "initMessage"
                }
            ]
        },
        {
            "trigger": "Bye",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "byeMessage1"
                },
                {
                    "type": "sendInitMessage",
                    "messageId": "initMessage"
                }
            ]
        },
        {
            "trigger": "Bye again!",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "byeMessage2"
                },
                {
                    "type": "sendInitMessage",
                    "messageId": "initMessage"
                }
            ]
        }
    ],
    "messages": [
        {
            "id": "welcomeMessage1",
            "text": "Hi, welcome to you, too!"
        },
        {
            "id": "byeMessage1",
            "text": "Goodbye! I hope to read from you soon.",
            "options": [
                "Bye again!"
            ]
        },
        {
            "id": "byeMessage2",
            "text": "Goodbye again :-)."
        }
    ],
    "defaultMessage": {
        "id": "defaultMessage",
        "text": "Sorry, but I'm just a humble bot, that only responds to the given options."
    }
}
```