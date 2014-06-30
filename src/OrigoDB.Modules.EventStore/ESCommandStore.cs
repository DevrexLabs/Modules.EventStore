using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using OrigoDB.Core;
using OrigoDB.Core.Storage;

namespace OrigoDB.Modules.EventStore
{
    public class ESCommandStore : CommandStore
    {
        
        //Number of events per read from the event store
        private const int ReadChunkSize = 200;

        readonly string _streamName;
        private readonly IPEndPoint _endPoint;

        public ESCommandStore(EngineConfiguration config, IPEndPoint endPoint, string streamName): base(config)
        {
            _endPoint = endPoint;
            _formatter = config.CreateFormatter(FormatterUsage.Journal);
            _streamName = streamName;
        }

        public override IEnumerable<JournalEntry> GetJournalEntriesBeforeOrAt(DateTime pointInTime)
        {
            return GetJournalEntriesFrom(0).TakeWhile(entry => entry.Created <= pointInTime);
        }

        public override IEnumerable<JournalEntry> GetJournalEntriesFrom(ulong entryId)
        {
            if (entryId > Int32.MaxValue) throw new InvalidOperationException("EventStore storage event id overflow");

            using (var connection = EventStoreConnection.Create(_endPoint))
            {
                connection.Connect();
                StreamEventsSlice currentSlice;
                var nextSliceStart = (int) entryId;
                do
                {
                    currentSlice = connection.ReadStreamEventsForward(_streamName, nextSliceStart, ReadChunkSize, false);
                    //if (currentSlice.Status != SliceReadStatus.StreamNotFound) throw new Exception("Can't read stream: " + currentSlice.Status);
                    nextSliceStart = currentSlice.NextEventNumber;
                    foreach (var resolvedEvent in currentSlice.Events)
                    {
                        yield return _formatter.FromByteArray<JournalEntry>(resolvedEvent.Event.Data);
                    }

                } while (!currentSlice.IsEndOfStream);
            }
        }

        protected override IJournalWriter CreateStoreSpecificJournalWriter()
        {
            var connection = EventStoreConnection.Create(_endPoint);
            connection.Connect();
            var formatter = _config.CreateFormatter(FormatterUsage.Journal);
            return new ESJournalWriter(connection, formatter, _streamName);
        }

        public override Stream CreateJournalWriterStream(ulong firstEntryId = 1)
        {
            throw new NotImplementedException();
        }
    }
}