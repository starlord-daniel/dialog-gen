# Sample: Simple Dialog Bot

This sample showcases a simple way to set up a more sophisticated dialog structure.

## What you can do with it

The simple dialog bot allows to store a dialog state value, that helps to keep track of the current position in the dialog structure. 

It initially asks to choose between one of 2 options: Name or Age. Both of these options trigger a short dialog, which displays a message and then uses the **"storeState"** action type to save the dialog state.

A new concept, the **triggerState** is introduced, which replaces the **trigger** option for the specific actions: A triggerState is triggered when the dialog state is changed to its value. It then behaves like any other action and performs all the actions in the **triggerActions** list. 

**Please note, that in the last action of a dialog (in this sample, e.g. the nameAsked action), the state needs to be reset to the value "default"**

Another new inclusion to this sample are replacements in messages. This can be observed in the message: **nameFinalMessage1**

```json
...
{
    "id": "nameFinalMessage1",
    "text": "Thank you, {input}."
},
...
```

It is possible to include the user input in the message by using the notation: **{input}** inside of the string. 

Note: Only the exact notation {input} is supported right now. Something like **{userInput}** or **[input]** would not work.


## Sample dialog structure JSON

```json
{
    "initMessage": {
        "id": "initMessage",
        "text": "Hello, I'm a simple dialog bot. Start the conversation by choosing one of the options.",
        "options": [
            "Name",
            "Age"
        ]
    },
    "actions": [
        {
            "trigger": "Name",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "nameMessage1"
                },
                {
                    "type": "storeState",
                    "value": "nameAsked"
                }
            ]
        },
        {
            "trigger": "Age",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "ageMessage1"
                },
                {
                    "type": "storeState",
                    "value": "ageAsked"
                }
            ]
        },
        {
            "triggerState": "nameAsked",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "nameFinalMessage1"
                },
                {
                    "type": "storeState",
                    "value": "default"
                },
                {
                    "type": "sendInitMessage"
                }
            ]
        },
        {
            "triggerState": "ageAsked",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "ageFinalMessage1"
                },
                {
                    "type": "storeState",
                    "value": "default"
                },
                {
                    "type": "sendInitMessage"
                }
            ]
        }
    ],
    "messages": [
        {
            "id": "nameMessage1",
            "text": "Hi, what's your name?"
        },
        {
            "id": "ageMessage1",
            "text": "Hi, how old are you?"
        },
        {
            "id": "nameFinalMessage1",
            "text": "Thank you, {input}."
        },
        {
            "id": "ageFinalMessage1",
            "text": "Thanks, I got that you are {input} years old."
        }
    ],
    "defaultMessage": {
        "id": "defaultMessage",
        "text": "Sorry, but I'm just a humble bot, that only responds to the given options."
    }
}
```