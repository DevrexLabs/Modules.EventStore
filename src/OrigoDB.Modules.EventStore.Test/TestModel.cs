using System;
using OrigoDB.Core;

namespace OrigoDB.Modules.EventStore.Test
{
    [Serializable]
    public class TestModel : Model
    {
        public int State { get; set; }

        public int GetState()
        {
            return State;
        }

        public void SetState(int state)
        {
            State = state;
        }
    }
}