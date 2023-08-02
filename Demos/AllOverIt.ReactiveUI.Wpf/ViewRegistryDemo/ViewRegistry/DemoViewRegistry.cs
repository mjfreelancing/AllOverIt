using AllOverIt.ReactiveUI.Factories;
using AllOverIt.ReactiveUI.ViewRegistry;

namespace ViewRegistryDemo.ViewRegistry
{
    public sealed class DemoViewRegistry : ViewRegistry<int>, IDemoViewRegistry
    {
        public DemoViewRegistry(IViewFactory viewFactory, IViewHandler viewHandler)
            : base(viewFactory, viewHandler)
        {
        }
    }
}
