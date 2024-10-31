using System.Collections.Generic;

namespace PureFsm.Tests.Runtime
{
    class TestFsmD : Fsm<TestFsmD>
    {
        public TestFsmD(IEnumerable<IState<TestFsmD>> states) : base(states)
        {
            _ = Run<D>();
        }
    }
}