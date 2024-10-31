## PureFsm

PureFsmは、Unityで使える軽量なステートマシンライブラリです。

### 特長

- PureC#で実装されているため、Pureなステートマシンが記述できます。
- UniTaskに対応しているため、非同期処理を簡単に記述できます。
- DIコンテナとの連携が可能です。各Stateやステートマシンに静的な依存性を注入することが出来ます。

### インストール

> [!WARNING]
> UniTaskが必須なので、先にUniTaskをインストールしてください。

その後Unity Package Managerで以下のURLを追加してください。

```
https://github.com/qemel/PureFsm.git
```

### Overview

```csharp
using PureFsm;

public class SampleFsm : Fsm<SampleFsm>
{
    public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states)
    {
        AddTransition<IdleState, WalkState>((int)EventId.Walk);
        AddTransition<WalkState, IdleState>((int)EventId.Idle);
        
        _ = Run<IdleState>();
    }
}

public class IdleState : IState<SampleFsm>
{
    public UniTask<int> EnterAsync(CancellationToken token)
    {
        Debug.Log("Enter IdleState");
        return (int)EventId.Walk;
    }
}

public class WalkState : IState<SampleFsm>
{
    public async UniTask<int> EnterAsync(CancellationToken token)
    {
        await UniTask.Delay(1000, cancellationToken: token);
        Debug.Log("Enter WalkState");
        return (int)EventId.Idle;
    }
}

public enum EventId
{
    Walk,
    Idle
}
```

### 使い方

#### Fsmの定義

Fsmを定義したいクラスに`Fsm<T>`を継承します。`T`には自分自身を入れてください。

```csharp
using PureFsm;

public class SampleFsm : Fsm<SampleFsm> // SampleFsm自身を指定
{
}
```

そのコンストラクタにて、対象となるステートを追加します。コンストラクタには`IEnumerable<IState<T>>`
を渡してください。baseクラスのコンストラクタに渡すことで、ステートを追加できます。

```csharp

public class SampleFsm : Fsm<SampleFsm>
{
     public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states) // ステートを追加するコンストラクタ
     {
     }
}
```

#### ステートの定義

ステートとして、`IState<T>`を実装したクラスを作成します。

`EnterAsync()`にはステートに入った時の処理を記述します。戻り値にてステート遷移用のイベントIDを返します。

`ExitAsync()`にはステートから出る時の処理を記述します(`ExitAsync()`の実装は任意です)。

```csharp
public interface IState<T> where T : Fsm<T>
{
    UniTask<int> EnterAsync(CancellationToken token); // ステートに入った時の処理
    UniTask ExitAsync(CancellationToken token) => UniTask.CompletedTask; // ステートから出た時の処理（任意）
}
```

ステートの実装例は以下の通りです。

```csharp
public class IdleState : IState<SampleFsm>
{
    public UniTask<int> EnterAsync(CancellationToken token)
    {
        Debug.Log("Enter IdleState");
        return 1; // 例えばAddTransition<IdleState, WalkState>(1)と書かれていると、WalkStateに遷移する（後述）
    }
    
    public UniTask ExitAsync(CancellationToken token)
    {
        Debug.Log("Exit IdleState");
        return UniTask.CompletedTask;
    }
}

public class WalkState : IState<SampleFsm>
{
    public async UniTask<int> EnterAsync(CancellationToken token)
    {
        await UniTask.Delay(1000, cancellationToken: token); // async/awaitで非同期処理を記述できる
        Debug.Log("Enter WalkState");
        return 0;
    }
    
    public async UniTask ExitAsync(CancellationToken token)
    {
        await UniTask.Delay(1000, cancellationToken: token);
        Debug.Log("Exit WalkState");
        return UniTask.CompletedTask;
    }
}
```

#### ステート間の遷移の追加

最後に、コンストラクタ内で`AddTransition<IState<T>, IState<T>>(int eventId)`を使って、ステート間の遷移を追加します（コンストラクタ以外で追加することも出来ます）。

> [!WARNING]
>`eventId`には`-1`を利用しないでください。これはステートマシンの終了を意味します（後述）。

```csharp
public class SampleFsm : Fsm<SampleFsm>
{
    public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states)
    {
        AddTransition<IdleState, WalkState>(0); // IdleStateからWalkStateへの遷移に、イベントIDを0として追加
        AddTransition<WalkState, IdleState>(1); // WalkStateからIdleStateへの遷移に、イベントIDを1として追加
    }
}
```

> [!NOTE]
> `int`での管理がつらい場合は、`enum`を使って管理することも出来ます(型レベルでは対応していません)。
> 
> ```csharp
> public enum EventId
> {
>     Walk,
>     Idle
> }
> 
> public class SampleFsm : Fsm<SampleFsm>
> {
>     public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states)
>     {
>         AddTransition<IdleState, WalkState>((int)EventId.Walk);
>         AddTransition<WalkState, IdleState>((int)EventId.Idle);
>     }
> }
> ```

以上で、ステートマシンの実装は完了です。

### ステートマシンの実行

ステートマシンを実行するには、`Fsm<T>.Run<T>()`メソッドを呼び出します。これは継承先のクラスのみが実行できるので、もし外部から実行したい場合は別途`public`メソッドを用意してください。

```csharp
public class SampleFsm : Fsm<SampleFsm>
{
    public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states)
    {
        AddTransition<IdleState, WalkState>(0);
        AddTransition<WalkState, IdleState>(1);
        
        _ = Run<IdleState>(); // IdleStateからステートマシンを開始
    }
    
    public void RunWalkState()
    {
        _ = Run<WalkState>(); // WalkStateからステートマシンを開始
    }
}
```

ステートマシンを終了するには、`Fsm<T>.Stop()`メソッドを呼び出します。

```csharp
public class SampleFsm : Fsm<SampleFsm>
{
    public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states)
    {
        AddTransition<IdleState, WalkState>(0);
        AddTransition<WalkState, IdleState>(1);
        
        _ = Run<IdleState>();
    }
    
    public void StopFsm()
    {
        Stop(); // ステートマシンを終了
    }
}
```

また、ステートにて、そのステートの移動先がない場合は、`EnterAsync`メソッドの戻り値に`-1`を返すことで、ステートマシンを終了させることができます。

```csharp
public class EndState : IState<SampleFsm>
{
    public UniTask<int> EnterAsync(CancellationToken token)
    {
        Debug.Log("Enter EndState");
        return -1; // ステートマシンを終了
    }
}
```

先ほどの`Fsm<T>.Run<T>()`を`await`すれば、ステートマシン自体の終了を待つことも可能です。

```csharp
public class SampleFsm : Fsm<SampleFsm>
{
    public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states)
    {
        AddTransition<IdleState, WalkState>(0);
        AddTransition<WalkState, IdleState>(1);
    }

    public async UniTask RunIdleAsync()
    {
        await Run<IdleState>(); // IdleStateからステートマシンを開始し、ステート自体の終了を待つ
        Debug.Log("End");
    }    
}
```

### DIコンテナとの連携

PureFsmはDIコンテナとの連携が可能です。これによって各Stateに静的な依存性を注入した状態でステートマシンを構築することが出来ます。

以下のように、それぞれのStateにコンストラクタインジェクションを行うことができます。

```csharp
using PureFsm;

public class SampleFsm : Fsm<SampleFsm>
{
    public SampleFsm(IEnumerable<IState<SampleFsm>> states) : base(states)
    {
        // ...
    }
}

public class IdleState : IState<SampleFsm>
{
    private readonly Foo _foo;

    public IdleState(Foo foo)
    {
        _foo = foo;
    }

    public UniTask<int> EnterAsync(CancellationToken token)
    {
        return 1;
    }
}

public class WalkState : IState<SampleFsm>
{
    private readonly Bar _bar;
    private readonly Baz _baz;
    
    public WalkState(Bar bar, Baz baz)
    {
        _bar = bar;
        _baz = baz;
    }

    public UniTask<int> EnterAsync(CancellationToken token)
    {
        return 0;
    }
}
```

これらはVContainerで以下のように解決できます。

```csharp
using PureFsm;
using VContainer;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<SampleFsm>(Lifetime.Singleton);
        
        builder.Register<Foo>(Lifetime.Singleton).As<IState<TestFsm>>();
        builder.Register<Bar>(Lifetime.Singleton).As<IState<TestFsm>>();
        builder.Register<Baz>(Lifetime.Singleton).As<IState<TestFsm>>();
    }
}
```

これによって、自動的にFsmのインスタンスと各Stateのインスタンスが解決されます。


