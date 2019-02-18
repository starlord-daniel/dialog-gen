# dialog-gen
A library to generate dialogs based on a given structure file (JSON)

# How to guide

To use the dialog generator library, clone the repo (or donwload the code). Make sure to add a reference to the DialogGen.Lib in your own bot project. You can take a look at the samples to get an idea for the usage.

// If there is enough demand, this library will be published as a Nuget package for Dotnet Core.

## Setup your bot

To prepare for using the DialogGen library, you need to create a bot as showcased in the samples. The minimal configuration every bot need to have includes:

1. **Startup.cs - Configure Services**

    In the Startup.cs file of the project, make sure to include the code to setup the DialogGenerator. In the method called *"ConfigureServices"* you'll have to include this code after the services.AddSingleton(configuration) statement:

    ```csharp
    services.AddBot<MyBot>(options =>
    {
        // Create a storage to persist our values
        IStorage dataStore = new MemoryStorage();

        // Store ConverstationState (for current conversation) and UserState (for tracking user interaction)
        var conversationState = new ConversationState(dataStore);
        var userState = new UserState(dataStore);
        
        options.CredentialProvider = new ConfigurationCredentialProvider(configuration);               
        
        // Add the State to the bot
        options.State.Add(conversationState);
        options.State.Add(userState);
    });

    // Create the dialog generator instance
    DialogGenerator dialogGenerator = new DialogGenerator();

    // Set up the dialog generator with an absolute path to your file. 
    dialogGenerator.InitializeDialogGenerator(services, Path.GetFullPath("folder/your-dialog-structure.json"));
    ```

    Please make sure to rename the "MyBot" reference in services.AddBot accordingly, when you change name the main bot class.

2. **MyBot.cs - Add the library and accessors**

    To make use of the dialog library, you have to include the following code. The example assumes, that your bot class is called **MyBot**

    ```csharp
    public class MyBot : IBot
    {
        private readonly DialogLibAccessors _accessors;
        private readonly DialogGenerator _dialogGenerator;

        public MyBot(DialogLibAccessors accessors, DialogGenerator dialogGenerator)
        {
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
            _dialogGenerator = dialogGenerator ?? throw new System.ArgumentNullException(nameof(dialogGenerator));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle the dialog that is build in JSON
            await _dialogGenerator.HandleBotConversationsAsync(turnContext, cancellationToken);
        }
    }
    ```

## Dialog Structure document

The dialog structure consists of 2 main components: Messages and Actions. For more sophisticated dialogs, additional components will be introduced. The documentation shall be included in the specific sample.

### Messages

A message consists of an id, text and (optional) options. The text is displayed when an action triggers the message with the specified id. The options are displayed as buttons below the message text.

#### Special messages

The initMessage and default message are of the type message, but provide additional functionality. If the initMessage is specified, an initial message is sent to the user. The default message is used every time the triggers did not match the user input.

### Actions

Actions consist of a trigger and corresponding triggerActions. A trigger is a word or phrase that is checked against the user input. If the input matches, the triggerActions are executed. 
Each triggerAction consists of a type and messageId. The type defines the type of message to be send. The messageId is a reference to a message, that should be sent. An action can have multiple triggerActions, which are processed in order of the structure document (JSON).

### Sample dialog structure

This is a sample dialog structure, taken from the simple-prompt sample:

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
        }
    ],
    "defaultMessage": {
        "id": "defaultMessage",
        "text": "Sorry, but I'm just a humble bot, that only responds to the given options."
    }
}
```

# Participation

Everyone is welcome to participate to the project. Every major change (e.g. new service included into dialogs) needs to have proper documentation (README) and its own sample.
