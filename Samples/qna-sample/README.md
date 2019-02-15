# Sample: Qna Maker Dialog


To use Qna Maker with your dialog, add this to your dialog structure json file:

```json
"qnaSettings": {
    "endpointKey": "YOUR-ENDPOINT-KEY",
    "host": "https://YOUR-RESOURCE-NAME.azurewebsites.net/qnamaker",
    "route": "/knowledgebases/YOUR-KNOWLEDGE-BASE-ID/generateAnswer"
}
```

Also, to trigger the Qna, add the trigger to actions without a value for "trigger" as follows:

```json
"actions": [
    {
        "trigger": "",
        "triggerActions": [
            {
                "type": "sendQnaMessage"
            }
        ]
    }
],
```

**Note**: The type is case sensitive and can't be changed.