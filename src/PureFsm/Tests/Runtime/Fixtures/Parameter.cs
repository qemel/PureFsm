namespace PureFsm.Tests.Runtime
{
    class Parameter
    {
        public bool ChangedToB;
        public bool ChangedToC;
        public bool ExitA;

        public override string ToString() => $"ChangedToB: {ChangedToB}, ChangedToC: {ChangedToC}, ExitA: {ExitA}";
    }
}