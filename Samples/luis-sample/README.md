# Sample: LUIS Dialog

The LUIS service can be used to parse more natural language that is send by the user. The basic outcomes are: Intent, which is the intention behind the sentence. E.g. the phrase: "What's the weather in Berlin?" Would return the predefined intent "weather" (or something similar, based on the definition). The other outcome can be entities, which are values of properties from the text. In the previous example, the entity "location" would have the value: Berlin.

## How to include LUIS in the dialog structure

To include LUIS to your dialog structure, you have to define the **luisSettings**. These include:
- region: The [datacenter region for the LUIS service](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-regions) (not the whole url! E.g. westus).
- appId: The id of your LUIS application
- subscriptionKey: the subscription key of your LUIS application
- threshold: The confidence threshold. This defines a level of certainty the result needs to achieve. If it doesn't, the default message is displayed.

To use LUIS, you need to use the triggerAction type: "sendLuisMessage". The trigger phrase can be empty for this. LUIS performs the call based on the users input and sends the message with the messageId, corresponding to the topIntent. For example, if your LUIS service has 2 intents: intent.greeting and intent.insult, the messages should look like this:

```json
"messages": [
        {
            "id": "intent.greeting",
            "text": "Oh hi, nice to meet you! Wanna play catch? Sike, I can't do that :-)"
        },
        {
            "id": "intent.insult",
            "text": "Oh, that's not really nice :-("
        }
    ],
````

In the case, the threshold is higher than the confidence or that the None intent was the best fit, the defaultMessage gets displayed.

You can also replace placeholders for entities with their value by including the entity name in curly brakets, like this:

```json
"messages": [
        {
            "id": "intent.greeting",
            "text": "Oh hi {entity.Name}, nice to meet you. I don't really have a name, only living beings get one - or Mars rovers, apparently ¯\\_(ツ)_/¯"
        }
]
```

## Sample JSON

```json
{
    "initMessage": {
        "id": "initMessage",
        "text": "Hello, try to ask me some questions about xyz. You can ask something like: zyx"
    },
    "defaultMessage": {
        "id": "defaultMessage",
        "text": "Sorry, but I can't answer your question yet. Please ask me another question."
    },
    "actions": [
        {
            "trigger": "",
            "triggerActions": [
                {
                    "type": "sendLuisMessage"
                }
            ]
        }
    ],
    "messages": [
        {
            "id": "intent.first",
            "text": "Answer to the first intent with entity {entityName1}."
        },
        {
            "id": "intent.second",
            "text": "Answer to the second intent with entity {entityName2}."
        }
    ],
    "luisSettings": {
        "region": "yourregion",
        "appId": "y0ur-lu1s-app-1d",
        "subscriptionKey": "yoursubscriptionkey",
        "threshold": 0.4
    }
}
```