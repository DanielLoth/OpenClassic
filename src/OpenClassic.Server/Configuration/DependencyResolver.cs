using DryIoc;
using System;

namespace OpenClassic.Server.Configuration
{
    public static class DependencyResolver
    {
        public static IContainer Current { get; }

        static DependencyResolver()
        {
            var container = new Container();

            RegisterDependencies(container);

            Current = container;
        }

        static void RegisterDependencies(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
        }
    }
}
