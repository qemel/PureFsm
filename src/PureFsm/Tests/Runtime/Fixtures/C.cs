using System.Threading;
using Cysharp.Threading.Tasks;

namespace PureFsm.Tests.Runtime
{
    class C : IState<TestFsm>
    {
        readonly Parameter _parameter;

        public C(Parameter parameter)
        {
            _parameter = parameter;
        }

        public async UniTask<int> EnterAsync(CancellationToken token)
        {
            _parameter.ChangedToC = true;
            await UniTask.Delay(50, cancellationToken: token);
            return (int)TestEvent.CtoA;
        }

        public async UniTask ExitAsync(CancellationToken token)
        {
            await UniTask.Delay(50, cancellationToken: token);
        }
    }
}