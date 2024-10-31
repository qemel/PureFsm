using System.Threading;
using Cysharp.Threading.Tasks;

namespace PureFsm.Tests.Runtime
{
    class AEmpty : IState<TestFsmEmpty>
    {
        public async UniTask<int> EnterAsync(CancellationToken token) => 10;
    }
}