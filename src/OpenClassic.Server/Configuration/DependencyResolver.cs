using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DryIoc;
using OpenClassic.Server.Collections;
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

            // Now that the container has been set up in entirety, perform
            // any initialisation of the newly registered dependencies.
            InitialiseDependencies(container);
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

            container.Register<ISpatialDictionary<IPlayer>, NaiveSpatialDictionary<IPlayer>>(Reuse.Singleton);
            container.Register<ISpatialDictionary<INpc>, NaiveSpatialDictionary<INpc>>(Reuse.Singleton);

            container.Register<IPlayer, Player>(Reuse.Transient);
            container.Register<INpc, Npc>(Reuse.Transient);
        }

        static void InitialiseDependencies(IContainer container)
        {
            Debug.Assert(container != null);

            InitialiseWorld(container);
        }

        static void InitialiseWorld(IContainer container)
        {
            Debug.Assert(container != null);


            var world = container.Resolve<IWorld>();

            var players = new List<IPlayer>(500);
            for (var i = 0; i < players.Capacity; i++)
            {
                var newPlayer = container.Resolve<IPlayer>();
                newPlayer.Index = (short)i;

                players.Add(newPlayer);
            }

            var npcs = new List<INpc>(2000);
            for (var i = 0; i < npcs.Capacity; i++)
            {
                var newNpc = container.Resolve<INpc>();
                newNpc.Index = (short)i;

                npcs.Add(newNpc);
            }

            world.InitialiseWorld(players, npcs);
        }

        #region Protocol-specific dependency registration

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
                throw new InvalidConfigException("DependencyResolver could not finish initialisation. " +
                    "Please only use 'RSCD' for the ServerProtocol value in Settings.json");
            }
        }

        static void RegisterRscdDependencies(IContainer container)
        {
            Debug.Assert(container != null);

            container.Register<ChannelInitializer<ISocketChannel>, RscdChannelInitializer>(Reuse.Singleton);
            container.Register<RscdPacketWriter>(Reuse.Singleton);
            container.Register<ISessionUpdater, RscdSessionUpdater>(Reuse.Singleton);

            var packetHandlerConcreteTypes = GetChildrenOfInterface(typeof(IRscdPacketHandlerMarker));
            foreach (var handlerType in packetHandlerConcreteTypes)
            {
                container.Register(typeof(IPacketHandler), handlerType, Reuse.Singleton);
            }
        }

        #endregion

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
