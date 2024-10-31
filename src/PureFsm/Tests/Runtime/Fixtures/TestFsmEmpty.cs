using System.Collections.Generic;

namespace PureFsm.Tests.Runtime
{
    class TestFsmEmpty : Fsm<TestFsmEmpty>
    {
        public TestFsmEmpty(IEnumerable<IState<TestFsmEmpty>> states) : base(states)
        {
            _ = Run<AEmpty>();
        }
    }
}