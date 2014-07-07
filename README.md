Modules.EventStore
==================


This is a storage module for OrigoDB, the in-memory database toolkit for NET/Mono, see http://devrexlabs.github.io


Example code for eventstore tcp running on localhost and port 1113:

```csharp
   var config = EngineConfiguration.Create();
   var endPoint = new IPEndPoint(IPAddress.LoopBack, 1113);
   config.SetCommandStoreFactory(cfg => new ESCommandStore(cfg, endPoint, "my-stream"));

   var db = Db.For<RedisModel>(config);
```
