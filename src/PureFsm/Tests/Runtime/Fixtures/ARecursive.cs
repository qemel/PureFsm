using System.Threading;
using Cysharp.Threading.Tasks;

namespace PureFsm.Tests.Runtime
{
    class ARecursive : IState<TestFsmRecursive>
    {
        public async UniTask<int> EnterAsync(CancellationToken token)
        {
            await UniTask.Delay(50, cancellationToken: token);
            return 0;
        }
    }
}