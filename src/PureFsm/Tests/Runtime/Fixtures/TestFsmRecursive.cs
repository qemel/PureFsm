using System.Collections.Generic;

namespace PureFsm.Tests.Runtime
{
    class TestFsmRecursive : Fsm<TestFsmRecursive>
    {
        public TestFsmRecursive(IEnumerable<IState<TestFsmRecursive>> states) : base(states)
        {
            AddTransition<ARecursive, ARecursive>(0);
            _ = Run<ARecursive>();
        }
    }
}