using DryIoc;
using OpenClassic.Server.Configuration;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public class World : IWorld
    {
        private readonly IConfig config;
        private readonly List<IPlayer> players;
        private readonly List<INpc> npcs;

        public World(IConfig config)
        {
            Debug.Assert(config != null);

            this.config = config;

            var resolver = DependencyResolver.Current;
            Debug.Assert(resolver != null);

            const int playerCount = 500;

            players = new List<IPlayer>(playerCount);
            for (var i = 0; i < playerCount; i++)
            {
                var newPlayer = resolver.Resolve<IPlayer>();
                newPlayer.Index = (short)i;
                players.Add(newPlayer);
            }

            npcs = new List<INpc>();
        }

        public List<IPlayer> Players => players;

        public List<INpc> Npcs => npcs;

        public IPlayer GetAvailablePlayer()
        {
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (!player.IsActive)
                {
                    player.IsActive = true;
                    return player;
                }
            }

            return null;
        }
    }
}
