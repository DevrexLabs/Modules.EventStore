using System;
using OrigoDB.Core;

namespace OrigoDB.Modules.EventStore.Test
{
    [Serializable]
    public class TestCommand : Command<TestModel>
    {

        public override void Execute(TestModel model)
        {
            model.State++;
        }
    }
}
