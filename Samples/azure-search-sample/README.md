# Sample: Azure Search Dialog

## Search

The Azure Search service allows you to perform a search on a database without access to it from the bot. Bt calling the REST API, it is possible to send search queries and the results are returned in a format like this:

```json
{
    "@odata.context": "https://<yourname>.search.windows.net/indexes('your-index')/$metadata#docs(*)",
    "value": [
        {
            "@search.score": 2.6367974,
            "Property1": "Value1",
            "Property2": "Value2",
            "Property3": "Value3"
        }
    ]
}
```

The Dialog Generator library allows to automatically perform the requests to Azure Search and to map them to a specific card result. This mapping can look like this:

```json
"messageMapping": {
    "text": "This is your result",
    "card": {
        "imageUrl": "{Property1}",
        "title": "Result for {Property2}",
        "subtitle": "Your {Property2} was found in {Property3}",
        "options": [],
        "text": ""
    }
},
```

You can define the mapping by including the Azure Search property names in the strings of the specified card elements. These inclusions have to be surrounded by curly brackets. E.g. if you want to set the title to the value of Property1 of your result, type: 

```json
"title": "{Property1}"
```

You can include other text as well, which is rendered without alterations.

# Sample JSON

In this sample, to showcase 2 services working together, the Qna Maker service was also included. The important thing is, to change states based on the users choice at the start and change them back, once the respective dialog is finished.

```json
{
    "initMessage": {
        "id": "initMessage",
        "text": "Hello, I can help you to search for <include topic here>",
        "options": [
            "Search","FAQ"
        ]
    },
    "defaultMessage": {
        "id": "defaultMessage",
        "text": "I'm so sorry, I'm not worthy. Please try another question."
    },
    "actions": [
        {
            "trigger": "Search",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "searchMessageInit"
                },
                {
                    "type": "storeState",
                    "value": "searchInit"
                }
            ]
        },
        {
            "triggerState": "searchInit",
            "triggerActions": [
                {
                    "type": "sendAzureSearchMessage"
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
            "trigger": "FAQ",
            "triggerActions": [
                {
                    "type": "sendMessage",
                    "messageId": "faqMessageInit"
                },
                {
                    "type": "storeState",
                    "value": "faqInit"
                }
            ]
        },
        {
            "triggerState": "faqInit",
            "triggerActions": [
                {
                    "type": "sendQnaMessage"
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
            "id": "searchMessageInit",
            "text": "Search it is. What thing would you like to search for?"
        },
        {
            "id": "faqMessageInit",
            "text": "Ok, let's see if I can answer your questions. What do you want to know?"
        }
    ],
    "azureSearchSettings": {
        "messageMapping":{
            "text": "This is your result",
            "card": {
                "imageUrl": "{ImageUrl}",
                "title": "Result for {SearchResult}",
                "subtitle": "Your {SearchResult} was found in {SearchLocation}",
                "options": [],
                "text": ""
            }
        },
        "hostUrl": "https://<yoursearchname>.search.windows.net/indexes/<yourindex>/docs?api-version=2017-11-11",
        "endpointKey": "YOURENDP01NTK3Y5DG8S75G89F5SDGSD"
    },
    "qnaSettings": {
        "endpointKey": "YOUR-ENDPOINT-KEY",
        "host": "https://YOUR-RESOURCE-NAME.azurewebsites.net/qnamaker",
        "route": "/knowledgebases/YOUR-KNOWLEDGE-BASE-ID/generateAnswer"
    }
}
```