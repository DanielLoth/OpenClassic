using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DryIoc;
using OpenClassic.Server.Domain;
using OpenClassic.Server.Networking;
using OpenClassic.Server.Networking.Rscd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace OpenClassic.Server.Configuration
{
    public static class DependencyResolver
    {
        public static IContainer Current { get; }

        static DependencyResolver()
        {
            var container = new Container();

            RegisterDependencies(container);
            RegisterProtocolSpecificDependencies(container);

            Current = container;
        }

        static void RegisterDependencies(IContainer container)
        {
            Debug.Assert(container != null);

            var configProvider = new JsonConfigProvider();
            var config = configProvider.GetConfig();

            container.UseInstance<IConfig>(config);

            container.Register<GameServer>(Reuse.Singleton);
            container.Register<IGameEngine, GameEngine>(Reuse.Singleton);
            container.Register<IWorld, World>(Reuse.Singleton);
        }

        static void RegisterProtocolSpecificDependencies(IContainer container)
        {
            Debug.Assert(container != null);

            var config = container.Resolve<IConfig>();

            if (string.Equals("RSCD", config.ServerProtocol, StringComparison.OrdinalIgnoreCase))
            {
                RegisterRscdDependencies(container);
            }
            else
            {
                throw new InvalidConfigException("DependencyResolver coult not finish initialisation. " +
                    "Please only use 'RSCD' for the ServerProtocol value in Settings.json");
            }
        }

        static void RegisterRscdDependencies(IContainer container)
        {
            Debug.Assert(container != null);

            container.Register<ChannelInitializer<ISocketChannel>, RscdChannelInitializer>(Reuse.Singleton);

            var packetHandlerConcreteTypes = GetChildrenOfInterface(typeof(IRscdPacketHandlerMarker));
            foreach (var handlerType in packetHandlerConcreteTypes)
            {
                container.Register(typeof(IPacketHandler), handlerType, Reuse.Singleton);
            }
        }

        #region Helper methods

        private static List<Type> GetChildrenOfInterface(Type interfaceType)
        {
            Debug.Assert(interfaceType != null);

            var types =
                Assembly.GetEntryAssembly()
                    .GetTypes()
                    .Where(x => x.GetInterfaces().Contains(interfaceType))
                    .ToList();

            return types;
        }

        #endregion
    }
}
