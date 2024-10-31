using System.Threading;
using Cysharp.Threading.Tasks;

namespace PureFsm
{
    /// <summary>
    ///     ステートのインターフェース
    /// </summary>
    /// <remarks>
    ///     Tはステートを持つFsmの型です。DIコンテナを使う場合は、Fsmの型を明確にするために使用します。
    /// </remarks>
    public interface IState<T> where T : Fsm<T>
    {
        /// <summary>
        ///     ステートに入ったときに呼ばれます。
        /// </summary>
        /// <returns>
        ///     次のEventIdを含めたUniTask
        /// </returns>
        UniTask<int> EnterAsync(CancellationToken token);

        /// <summary>
        ///     ステートから出るときに呼ばれます。
        /// </summary>
        UniTask ExitAsync(CancellationToken token) => UniTask.CompletedTask;
    }
}