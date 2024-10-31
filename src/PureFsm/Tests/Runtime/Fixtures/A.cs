using System.Threading;
using Cysharp.Threading.Tasks;

namespace PureFsm.Tests.Runtime
{
    class A : IState<TestFsm>
    {
        readonly Parameter _parameter;

        public A(Parameter parameter)
        {
            _parameter = parameter;
        }

        public async UniTask<int> EnterAsync(CancellationToken token)
        {
            await UniTask.Delay(50, cancellationToken: token);
            return (int)TestEvent.AtoB;
        }

        public async UniTask ExitAsync(CancellationToken token)
        {
            _parameter.ExitA = true;
        }
    }
}