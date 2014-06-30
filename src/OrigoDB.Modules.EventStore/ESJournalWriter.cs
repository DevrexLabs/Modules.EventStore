using System;
using System.Runtime.Serialization;
using EventStore.ClientAPI;
using OrigoDB.Core;

namespace OrigoDB.Modules.EventStore
{
    public class ESJournalWriter : IJournalWriter
    {

        readonly IEventStoreConnection _connection;
        readonly IFormatter _formatter;
        readonly string _streamName;

        public ESJournalWriter(IEventStoreConnection conn, IFormatter formatter, string streamName)
        {
            _connection = conn;
            _formatter = formatter;
            _streamName = streamName;
        }

        public void Close()
        {
            _connection.Close();
        }

        public void Write(JournalEntry item)
        {
            var bytes = _formatter.ToByteArray(item);
            const bool isJson = false;
            var eventData = new EventData(Guid.NewGuid(), "OrigoDB.JournalEntry", isJson, bytes, null);
            _connection.AppendToStream(_streamName, ExpectedVersion.Any, eventData);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}