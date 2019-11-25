# Endev-Framework : C# Port 

The _[Endev-Framework](https://github.com/TobiHatti/EndevFramework)_ provides several easy to use components you can include into your projects to simplify and accelerate your development-experience.

# Modules
The different modules are completely independend on each other, so the use of just a single module in your project is possible instead of loading in a single, fully featured framework into your project, unneccecaraly bloating up the entire project.

## <i class="fas fa-server"></i> Endev-Network-Core
[&#9654; Source Code](https://github.com/TobiHatti/EndevFramework/tree/master/NetworkCore) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[&#9654; Releases & Downloads](https://github.com/TobiHatti/EndevFramework/releases) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[&#9654; NuGet-Install](#)

__[ALPHA VERSION 1.2 Rev3] Send-Stability: 99,999% (± 0,001%)__

The Endev-Network-Core allows you to communicate between application via the TCP-Protocol.
The Module allows __one server__ and __multiple clients__. 
The communication-system is based on __instructions__, to allow a wide range of functionalities. 
To provide a secure communication between users, confidential data gets automaticaly encryptet using RSA-Encryption. 
The module is easily expandable and custom instructions or authentication-checks can quickly be added or modified.


__KEY FEATURES__
- Server-Client concept
- No client-limitation
- RSA-Encryption
- Semi-Automatic setup
- Reliable error handling
- "_TurnKey-Solution_"

__EXAMPLE__
```csharp
// Initialize the server on port 2225
NetComServer server = new NetComServer(2225);

// Select where to show the debug-information
server.SetDebugOutput(DebugOutput.ToConsole);

// Select a method to authenticate clients
server.SetAuthenticationTool(MyCustomSQLAuthentication);

// Start the server
server.Start();

// Create an instruction to show a messagebox on the receivers screen
var instruction = new InstructionLibraryEssentials.SimpleMessageBox
	(server, server.ConnectedClients["SampleUsername"], "Hello world!");

// Send the instruction
server.Send(instruction);
```

[&#9654; See full example and documentation](https://github.com/TobiHatti/EndevFramework/blob/master/NetworkCore/README.md)

## Endev-WinForms-Core
[&#9654; Source Code](https://github.com/TobiHatti/EndevFramework/tree/master/WinFormsCore) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[&#9654; Releases & Downloads](https://github.com/TobiHatti/EndevFramework/releases) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[&#9654; NuGet-Install](#)

__[WIP]__

_This module is not ready for use yet!_




## Endev-Graphics-Core
[&#9654; Source Code](https://github.com/TobiHatti/EndevFramework/tree/master/GraphicsCore) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[&#9654; Releases & Downloads](https://github.com/TobiHatti/EndevFramework/releases) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[&#9654; NuGet-Install](#)

__[WIP]__

_This module is not ready for use yet!_
