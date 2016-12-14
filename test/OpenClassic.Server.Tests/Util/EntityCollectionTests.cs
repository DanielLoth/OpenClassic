using OpenClassic.Server.Domain;
using OpenClassic.Server.Util;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Util
{
    public class EntityCollectionTests
    {
        [Fact]
        public void AddShouldInsertIntoAddedList()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);

            Assert.True(entities.Added.Count == 1);
        }

        [Fact]
        public void AddShouldInsertMultipleIntoAddedList()
        {
            var players = new List<IPlayer> { new Player(), new Player(), new Player() };
            var entities = new EntityCollection<IPlayer>();

            entities.Add(players);

            Assert.Equal(players.Count, entities.Added.Count);
        }

        [Fact]
        public void RemoveShouldInsertIntoRemovedList()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Remove(player);

            Assert.True(entities.Removed.Count == 1);
        }

        [Fact]
        public void ChangedShouldReturnTrueWhenAddedListContainsEntities()
        {
            var players = new List<IPlayer> { new Player(), new Player(), new Player() };
            var entities = new EntityCollection<IPlayer>();

            entities.Add(players);

            Assert.True(entities.Changed());
        }

        [Fact]
        public void ChangedShouldReturnTrueWhenRemovedListContainsEntities()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Remove(player);

            Assert.True(entities.Changed());
        }

        [Fact]
        public void ChangeShouldReturnTrueWhenAddedAndRemovedListsContainEntities()
        {
            var player1 = new Player();
            var player2 = new Player();

            var entities = new EntityCollection<IPlayer>();

            entities.Add(player1);
            entities.Remove(player2);

            Assert.True(entities.Changed());
        }

        [Fact]
        public void ChangedShouldReturnFalseWhenAddedAndRemovedListsAreEmpty()
        {
            var entities = new EntityCollection<IPlayer>();

            Assert.False(entities.Changed());
        }

        [Fact]
        public void RemovingReturnsTrueWhenRemovedListContainsEntity()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Remove(player);

            var removing = entities.Removing(player);

            Assert.True(removing);
        }

        [Fact]
        public void RemovingReturnsFalseWhenRemovedListDoesNotContainEntity()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            var removing = entities.Removing(player);

            Assert.False(removing);
        }

        [Fact]
        public void ContainsReturnsTrueWhenAddedListContainsEntity()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);

            var addedListContains = entities.Contains(player);

            Assert.True(addedListContains);
        }

        [Fact]
        public void ContainsReturnsTrueWhenKnownListContainsEntity()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);
            entities.Update();

            // Verify that player is no longer in the 'added' list.
            Assert.False(entities.Added.Contains(player));

            var knownListContains = entities.Contains(player);

            Assert.True(knownListContains);
        }

        [Fact]
        public void UpdateMovesEntitiesFromAddedToKnownList()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);
            entities.Update();

            // Verify that player is no longer in the 'added' list.
            Assert.False(entities.Added.Contains(player));

            // Verify that the 'known' list now contains an entity.
            Assert.True(entities.Known.Contains(player));
        }

        [Fact]
        public void UpdateRemovesEntitiesInRemovedListFromKnownList()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            // Add a player, update to put it in the 'known' list.
            entities.Add(player);
            entities.Update();

            // Remove the player, update to remove it from the 'known' list.
            entities.Remove(player);
            entities.Update();

            Assert.False(entities.Known.Contains(player));
        }

        [Fact]
        public void UpdateClearsAddedList()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            // Add a player, update to put it in the 'known' list.
            entities.Add(player);
            entities.Update();

            Assert.False(entities.Added.Contains(player));
        }

        [Fact]
        public void UpdateClearsRemovedList()
        {
            var player = new Player();
            var entities = new EntityCollection<IPlayer>();

            // Add a player, update to put it in the 'known' list.
            entities.Add(player);
            entities.Update();

            // Remove the player, update to remove it from the 'known' list.
            entities.Remove(player);
            entities.Update();

            Assert.False(entities.Removed.Contains(player));
        }

        [Fact]
        public void AllPropertyReturnsEntitiesFromBothAddedAndKnownLists()
        {
            var player1 = new Player();
            var player2 = new Player();
            var entities = new EntityCollection<IPlayer>();

            entities.Added.Add(player1);
            entities.Known.Add(player2);

            var all = new HashSet<IPlayer>(entities.All);

            Assert.True(all.Contains(player1));
            Assert.True(all.Contains(player2));
        }
    }
}
