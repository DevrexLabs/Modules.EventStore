Modules.EventStore
==================

EventStore command logging for OrigoDB


Example for eventstore tcp running on localhost and port 1113:

```csharp
   var config = EngineConfiguration.Create();
   var endPoint = new IPEndPoint(IPAddress.LoopBack, 1113);
   config.SetCommandStoreFactory(cfg => new ESCommandStore(cfg, endPoint, "my-stream"));
```
