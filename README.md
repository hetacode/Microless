# Introduction 
Microless is an library to simplify maintanence of communication between microservices. Using this library you're getting ability to create state machine (simple implementation of saga pattern) which it handle sending and receiving messages from services connected to an message bus (prepared rabbitmq provider).
The library also give ability to create simple "function" implementation. Each coming message is handle by embedded mechanism of library, which recognize message type and then call proper function with implemented logic.
**Only for testing (no production ready)**

# Getting Started
The library is created for .Net Core 3 Preview 7. 
To start your journey with Microless:
1. You have to create all your projects in Asp.core template ("ASP.NET Core Empty" will be enough)
2. Install packages:
  - Install-Package Hetacode.Microless
  - Install-Package Hetacode.Microless.Abstractions
  - Install-Package Hetacode.Microless.RabbitMQ
3. In Startup.cs add above code:
  - In ConfigureServices method
```
// Register all needed dependencies
services.AddMicroless();
// Connect to message bus
services.AddMessageBus(config =>
{
// Register queue provider - <host> <username> <password> <virtual host>
  config.Provider = new RabbitMQProvider("localhost", "guest", "guest", "saga");
});
```
  - And then in Configure method:
```
app.UseMicroless();
app.UseMessageBus((steps, subscribe) =>
{
// Add messages listener - in this example, this one is listening on "Saga" queue and return <queue name - "Saga"> <message object> <transmitted headers>
  subscribe.AddReceiver("Saga", async (queueName, message, headers) =>
  {
    // <Put "message resolver" code here>
  });
});
```
4. In this step you have to define next purpose:
**For steps aggregator**
  - Create aggregator class. This class should implement IAggregator interface
  - Inject IAggregatorBuilder service
  - Register your steps like this (basic version - without handling errors and rollbacks):
```
public TestAggregator(IAggregatorBuilder states)
{
  // Init step - send first message
  _states = states.Init(c =>
  {
    var id = Guid.NewGuid();
    Console.WriteLine($"Init saga: {id}");
    // Send message - put message in "Service" queue
    c.SendMessage<MessageRequest>("Service", new MessageRequest());
  })
  // Response - message insert into "Saga" queue
  .Step<MessageResponse>((c, r) =>
  {
    Console.WriteLine($"Response1 saga: {c.CorrelationId}");
    c.SendMessage<Message1Request>("Service1", new Message1Request());
  })
  .Step<Message1Response>((c, r) =>
  {
    Console.WriteLine($"Response2 saga: {c.CorrelationId}");
    c.SendMessage<Message2Request>("Service2", new Message2Request());
  })
  // Last step - the best place to call some response service like signalr
  .Finish<Message2Response>((c, r) =>
  {
    Console.WriteLine($"Finish saga: {c.CorrelationId}");
  });
}

// This method run in any moment - it launch states mechanism
public void Run(IContext context)
{
_states.Call(context);
}
 ```
  - back to Startup.cs - it's time to connect steps resolver to messages consumer
So, just change this line <Put "message resolver" code here> to 
```
steps.Call(queueName, message, headers);
```
That's all!
**For the service side**
  - Create function - it's just an class with two async methods: "Run" and "Rollback"
> Important! Function class should be named in this pattern "<Name>Function"
  - Add an attribute:
```
[BindMessage(typeof(MessageRequest))]
```
The attribute bind your function with receiving message.
  - Run method is responsible for logic of function.
  - Rollback responds rollback logic
  - Both methods pass "context" parameter (look at "Context" section)
  - Like in "Aggregator" approach, please change <Put "message resolver" code here> in Startup.cs file.
But in this case use this line:
```
await steps.CallFunction(queueName, message, headers);
```
> For better naming, just change "steps" variable name in you want. 
#Context
Context is an argument passed to both aggregator steps and functions methods. This object contains methods to send messages, errors and rollbacks.
```
void SendMessage<T>(string name, T message, Dictionary<string, string> headers = null);

void SendError<T>(string name, T message, Dictionary<string, string> headers = null);

void SendRollback<T>(string name, T message);
```
There is also exists Headers property. This dictionary contains headers passing between services. 

# Build and Test
[![Build Status](https://hetacode.visualstudio.com/Microless/_apis/build/status/general-build?branchName=master)](https://hetacode.visualstudio.com/Microless/_build/latest?definitionId=5&branchName=master)
[![Build Status](https://hetacode.visualstudio.com/Microless/_apis/build/status/packages-build?branchName=master)](https://hetacode.visualstudio.com/Microless/_build/latest?definitionId=8&branchName=master)
