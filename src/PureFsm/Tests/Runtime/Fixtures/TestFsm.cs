using System.Collections.Generic;

namespace PureFsm.Tests.Runtime
{
    class TestFsm : Fsm<TestFsm>
    {
        public TestFsm(IEnumerable<IState<TestFsm>> states) : base(states)
        {
            AddTransition<A, B>((int)TestEvent.AtoB);
            AddTransition<B, C>((int)TestEvent.BtoC);
            AddTransition<C, A>((int)TestEvent.CtoA);

            _ = Run<A>();
        }

        public void RunA() => _ = Run<A>();
        public void RunB() => _ = Run<B>();
    }
}