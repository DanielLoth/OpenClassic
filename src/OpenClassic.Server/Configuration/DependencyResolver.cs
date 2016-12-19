using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DryIoc;
using OpenClassic.Server.Collections;
using OpenClassic.Server.Domain;
using OpenClassic.Server.Networking;
using OpenClassic.Server.Networking.Rscd;
using OpenClassic.Server.Util;
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
            container.Register<ISpatialDictionary<IGameObject>, NaiveSpatialDictionary<IGameObject>>(Reuse.Singleton);

            container.Register<IPlayer, Player>(Reuse.Transient);
            container.Register<INpc, Npc>(Reuse.Transient);
            container.Register<IGameObject, GameObject>(Reuse.Transient);
        }

        static void InitialiseDependencies(IContainer container)
        {
            Debug.Assert(container != null);

            InitialiseGameConnectionHandler(container);
            InitialiseWorld(container);
        }

        static void InitialiseGameConnectionHandler(IContainer container)
        {
            Debug.Assert(container != null);

            var engine = container.Resolve<IGameEngine>();
            var packetHandlers = container.Resolve<IPacketHandler[]>();

            Debug.Assert(engine != null);
            Debug.Assert(packetHandlers != null);

            GameConnectionHandler.Init(engine, packetHandlers);
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

            var npcLocations = DataLoader.GetNpcLocations();
            var npcs = new List<INpc>(npcLocations.Count);
            for (var i = 0; i < npcs.Capacity; i++)
            {
                var newNpc = container.Resolve<INpc>();
                newNpc.Index = (short)i;
                newNpc.Active = true;

                var npcLoc = npcLocations[i];
                newNpc.Id = npcLoc.NpcId;

                newNpc.StartX = npcLoc.StartX;
                newNpc.StartY = npcLoc.StartY;

                newNpc.MinX = npcLoc.MinX;
                newNpc.MinY = npcLoc.MinY;

                newNpc.MaxX = npcLoc.MaxX;
                newNpc.MaxY = npcLoc.MaxY;

                var startPoint = new Point(npcLoc.StartX, npcLoc.StartY);
                newNpc.Location = startPoint;

                npcs.Add(newNpc);

                world.NpcSpatialMap.Add(newNpc);
            }

            var gameObjectLocs = DataLoader.GetObjectLocations();
            var gameObjects = new List<IGameObject>(gameObjectLocs.Count);
            for (var i = 0; i < gameObjectLocs.Count; i++)
            {
                var newGameObject = container.Resolve<IGameObject>();
                newGameObject.Index = (short)i;

                var objLoc = gameObjectLocs[i];

                newGameObject.Id = objLoc.Id;
                newGameObject.Type = objLoc.Type;
                newGameObject.Direction = objLoc.Direction;

                var location = new Point(objLoc.X, objLoc.Y);
                newGameObject.Location = location;

                gameObjects.Add(newGameObject);

                world.ObjectSpatialMap.Add(newGameObject);
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
