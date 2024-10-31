using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PureFsm
{
    /// <summary>
    ///     ステートマシンの基底クラスです。
    /// </summary>
    /// <typeparam name="T">
    ///     Fsmを持つクラスの型です。Fsmを継承したクラス自身を指定してください。
    /// </typeparam>
    public class Fsm<T> : IDisposable where T : Fsm<T>
    {
        const int CancelEventId = -1;
        readonly HashSet<IState<T>> _states = new();
        readonly Dictionary<int, Dictionary<IState<T>, IState<T>>> _transitions = new();
        CancellationTokenSource _cts = new();

        IState<T> _currentState;

        protected Fsm(IEnumerable<IState<T>> states)
        {
            foreach (var state in states) _states.Add(state);
        }

        /// <summary>
        ///     ステートの個数
        /// </summary>
        public int StateCount => _states.Count;

        /// <summary>
        ///     遷移の個数
        /// </summary>
        public int TransitionCount => _transitions.Values.SelectMany(x => x).Count();

        /// <summary>
        ///     Fsmが動作中かどうか
        /// </summary>
        public bool IsRunning => _currentState != null;

        public void Dispose()
        {
            _cts?.Dispose();
        }

        public override string ToString()
        {
            var currentStateType = _currentState.GetType();
            var states = _states.Select(x => x.GetType().Name).ToArray();
            var transitions = _transitions
                              .SelectMany(
                                  x => x.Value.Select(
                                      y => $"{x.Key}: {y.Key.GetType().Name} -> {y.Value.GetType().Name}"
                                  )
                              )
                              .ToArray();

            return
                $"Current State: {currentStateType.Name}, " +
                $"States: {string.Join(", ", states)}, " +
                $"Transitions: {string.Join(", ", transitions)}";
        }

        /// <summary>
        ///     Fsm自体の停止をします。
        /// </summary>
        public void Stop()
        {
            _currentState = null;
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        ///     遷移を追加します。Fsmの初期化時に呼び出してください。
        /// </summary>
        protected void AddTransition<TFrom, TTo>(int eventId) where TFrom : IState<T> where TTo : IState<T>
        {
            if (eventId == CancelEventId)
                throw new InvalidOperationException("CancelEventId is reserved, please use another id");

            var fromState = typeof(TFrom);
            var toState = typeof(TTo);

            // イベントIDがなければ初期化
            if (!_transitions.ContainsKey(eventId))
                _transitions[eventId] = new Dictionary<IState<T>, IState<T>>();

            var from = _states.FirstOrDefault(x => x.GetType() == fromState);
            var to = _states.FirstOrDefault(x => x.GetType() == toState);

            if (from == null || to == null) return;

            if (_transitions[eventId].ContainsKey(from))
            {
                throw new InvalidOperationException(
                    $"Transition already exists: {fromState.Name} -> {toState.Name}, EventId: {eventId}"
                );
            }

            _transitions[eventId][from] = to;
        }

        /// <summary>
        ///     開始状態を設定します。
        /// </summary>
        protected async UniTask Run<TState>() where TState : IState<T>
        {
            _currentState = _states.FirstOrDefault(x => x.GetType() == typeof(TState));
            if (_currentState == null) throw new InvalidOperationException("State not found");

            while (!_cts.Token.IsCancellationRequested)
            {
                var eventId = await _currentState.EnterAsync(_cts.Token);

                if (eventId == CancelEventId)
                {
                    Stop();
                    return;
                }

                await _currentState.ExitAsync(_cts.Token);

                if (_transitions.ContainsKey(eventId) && _transitions[eventId].ContainsKey(_currentState))
                {
                    // 次の状態に遷移
                    var nextState = _transitions[eventId][_currentState];
                    _currentState = nextState;
                }

                await UniTask.Yield();
            }
        }
    }
}