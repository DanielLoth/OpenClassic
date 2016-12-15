using System.Collections.Generic;

namespace OpenClassic.Server.Domain
{
    public class World : IWorld
    {
        private readonly List<IPlayer> _players = new List<IPlayer>(500);
        private readonly List<INpc> _npcs = new List<INpc>(2000);

        public void InitialiseWorld(List<IPlayer> players, List<INpc> npcs)
        {
            _players.AddRange(players);
            _npcs.AddRange(npcs);
        }

        public List<IPlayer> Players => _players;

        public List<INpc> Npcs => _npcs;

        public IPlayer GetAvailablePlayer()
        {
            for (var i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                if (!player.Active)
                {
                    player.Active = true;
                    return player;
                }
            }

            return null;
        }
    }
}
