using DryIoc;
using OpenClassic.Server.Domain;
using OpenClassic.Server.Networking;
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

            container.Register<GameServer>(Reuse.Singleton);
            container.Register<IGameEngine, GameEngine>(Reuse.Singleton);
            container.Register<IWorld, World>(Reuse.Singleton);
        }
    }
}
