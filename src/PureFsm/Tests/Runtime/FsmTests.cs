using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PureFsm.Tests.Runtime
{
    class FsmTests
    {
        [UnityTest]
        public IEnumerator FsmInformationTest() => UniTask.ToCoroutine(
            async () =>
            {
                var parameter = new Parameter();

                var fsm = new TestFsm(
                    new IState<TestFsm>[]
                    {
                        new A(parameter),
                        new B(parameter),
                        new C(parameter)
                    }
                );

                Debug.Log(fsm.ToString());

                Assert.IsTrue(fsm.StateCount == 3, "ステートの個数が正しくありません。");
                Assert.IsTrue(fsm.TransitionCount == 3, "遷移の個数が正しくありません。");
            }
        );

        [UnityTest]
        public IEnumerator FsmStateTransitionBasicTest() => UniTask.ToCoroutine(
            async () =>
            {
                var parameter = new Parameter();

                var fsm = new TestFsm(
                    new IState<TestFsm>[]
                    {
                        new A(parameter),
                        new B(parameter),
                        new C(parameter)
                    }
                );

                Debug.Log(fsm.ToString());
                Debug.Log(parameter.ToString());

                await UniTask.Delay(200);

                Debug.Log(fsm.ToString());
                Debug.Log(parameter.ToString());

                Assert.IsTrue(parameter.ExitA, "StateAが正しくExitしませんでした。");
                Assert.IsTrue(parameter.ChangedToB, "StateBへの遷移が正しく行われませんでした。");
            }
        );

        [UnityTest]
        public IEnumerator FsmCancelAndRunTest() => UniTask.ToCoroutine(
            async () =>
            {
                var parameter = new Parameter();

                var fsm = new TestFsm(
                    new IState<TestFsm>[]
                    {
                        new A(parameter),
                        new B(parameter),
                        new C(parameter)
                    }
                );

                Debug.Log(fsm.ToString());
                Debug.Log(parameter.ToString());

                await UniTask.Delay(30);

                fsm.Stop();

                fsm.RunB();

                await UniTask.Delay(200);

                Assert.IsTrue(!parameter.ExitA, "StateAが正しくCancelされませんでした。");
                Assert.IsTrue(parameter.ChangedToC, "StateCへの遷移が正しく行われませんでした。");
            }
        );

        [UnityTest]
        public IEnumerator CancelIndexTest() => UniTask.ToCoroutine(
            async () =>
            {
                var fsm = new TestFsmD(
                    new IState<TestFsmD>[]
                    {
                        new D()
                    }
                );

                await UniTask.Delay(200);

                Assert.IsTrue(!fsm.IsRunning, "Fsmが正しくCancelされませんでした。");
            }
        );

        [UnityTest]
        public IEnumerator SameStateTransitionTest() => UniTask.ToCoroutine(
            async () =>
            {
                var fsmRecursive = new TestFsmRecursive(
                    new IState<TestFsmRecursive>[]
                    {
                        new ARecursive()
                    }
                );

                await UniTask.Delay(200);

                Assert.IsTrue(fsmRecursive.IsRunning, "Fsmが正しく動作しませんでした。");
            }
        );

        [UnityTest]
        public IEnumerator SameTransitionThrowTest() => UniTask.ToCoroutine(
            async () =>
            {
                Assert.Throws(
                    typeof(InvalidOperationException),
                    () =>
                    {
                        var fsm = new TestFsmThrow(
                            new IState<TestFsmThrow>[]
                            {
                                new AThrow(),
                                new BThrow()
                            }
                        );
                    }
                );
            }
        );
    }
}