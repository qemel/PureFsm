#if PURE_FSM_TEST_VCONTAINER
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;

namespace PureFsm.Tests.Runtime
{
    public sealed class VContainerTests
    {
        [UnityTest]
        public IEnumerator VContainerRegisterTest() => UniTask.ToCoroutine(
            async () =>
            {
                var lifetimeScope = LifetimeScope.Create(
                    builder =>
                    {
                        builder.Register<Parameter>(Lifetime.Singleton);
                        builder.Register<TestFsm>(Lifetime.Singleton);
                        builder.Register<A>(Lifetime.Singleton).As<IState<TestFsm>>();
                        builder.Register<B>(Lifetime.Singleton).As<IState<TestFsm>>();
                        builder.Register<C>(Lifetime.Singleton).As<IState<TestFsm>>();
                    }
                );


                await UniTask.Yield();
                await UniTask.Yield();

                var testFsm = lifetimeScope.Container.Resolve<TestFsm>();

                Assert.IsNotNull(testFsm);
                Assert.That(testFsm.StateCount == 3);
            }
        );

        [UnityTest]
        public IEnumerator VContainerRegisterSeparatelyTest() => UniTask.ToCoroutine(
            async () =>
            {
                var lifetimeScope = LifetimeScope.Create(
                    builder =>
                    {
                        builder.Register<Parameter>(Lifetime.Singleton);
                        builder.Register<TestFsm>(Lifetime.Singleton);
                        builder.Register<A>(Lifetime.Singleton).As<IState<TestFsm>>();
                        builder.Register<B>(Lifetime.Singleton).As<IState<TestFsm>>();
                        builder.Register<C>(Lifetime.Singleton).As<IState<TestFsm>>();

                        builder.Register<TestFsmEmpty>(Lifetime.Singleton);
                        builder.Register<AEmpty>(Lifetime.Singleton).As<IState<TestFsmEmpty>>();
                        builder.Register<BEmpty>(Lifetime.Singleton).As<IState<TestFsmEmpty>>();
                    }
                );


                await UniTask.Yield();
                await UniTask.Yield();

                var testFsm = lifetimeScope.Container.Resolve<TestFsm>();

                Assert.IsNotNull(testFsm);
                Assert.That(testFsm.StateCount == 3);

                var testFsmEmpty = lifetimeScope.Container.Resolve<TestFsmEmpty>();

                Assert.IsNotNull(testFsmEmpty);
                Assert.That(testFsmEmpty.StateCount == 2);
            }
        );
    }
}
#endif