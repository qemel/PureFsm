using System.Threading;
using Cysharp.Threading.Tasks;

namespace PureFsm.Tests.Runtime
{
    class B : IState<TestFsm>
    {
        readonly Parameter _parameter;

        public B(Parameter parameter)
        {
            _parameter = parameter;
        }

        public async UniTask<int> EnterAsync(CancellationToken token)
        {
            _parameter.ChangedToB = true;
            await UniTask.Delay(50, cancellationToken: token);
            return (int)TestEvent.BtoC;
        }

        public async UniTask ExitAsync(CancellationToken token)
        {
            await UniTask.Delay(50, cancellationToken: token);
        }
    }
}