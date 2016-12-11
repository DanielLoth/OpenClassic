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
                players[i] = resolver.Resolve<IPlayer>();
            }
        }

        public List<IPlayer> Players => players;
    }
}
