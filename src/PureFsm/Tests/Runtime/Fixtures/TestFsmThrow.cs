using System.Collections.Generic;

namespace PureFsm.Tests.Runtime
{
    class TestFsmThrow : Fsm<TestFsmThrow>
    {
        public TestFsmThrow(IEnumerable<IState<TestFsmThrow>> states) : base(states)
        {
            AddTransition<AThrow, BThrow>(0);
            AddTransition<AThrow, BThrow>(0);
            _ = Run<AThrow>();
        }
    }
}