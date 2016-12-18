using DryIoc;
using OpenClassic.Server.Collections;
using OpenClassic.Server.Domain;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Collections
{
    public class EntityCollectionTests
    {
        public static Container Container
        {
            get
            {
                var container = new Container();
                container.Register<IPlayer, Player>();
                container.Register<ISpatialDictionary<IPlayer>, NaiveSpatialDictionary<IPlayer>>();
                container.Register<ISpatialDictionary<INpc>, NaiveSpatialDictionary<INpc>>();
                container.Register<ISpatialDictionary<IGameObject>, NaiveSpatialDictionary<IGameObject>>();

                return container;
            }
        }

        [Fact]
        public void AddShouldInsertIntoAddedList()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);

            Assert.True(entities.AddedReadOnly.Contains(player));
        }

        [Fact]
        public void AddShouldInsertMultipleIntoAddedList()
        {
            var container = Container;
            var players = new HashSet<IPlayer> { container.Resolve<IPlayer>(), container.Resolve<IPlayer>() };
            var entities = new EntityCollection<IPlayer>();

            entities.Add(players);

            Assert.True(players.SetEquals(entities.AddedReadOnly));
        }

        [Fact]
        public void RemoveShouldInsertIntoRemovedList()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            entities.Remove(player);

            Assert.True(entities.RemovedReadOnly.Contains(player));
        }

        [Fact]
        public void ChangedShouldReturnTrueWhenAddedListContainsEntities()
        {
            var container = Container;
            var players = new HashSet<IPlayer> { container.Resolve<IPlayer>(), container.Resolve<IPlayer>() };
            var entities = new EntityCollection<IPlayer>();

            entities.Add(players);

            Assert.True(entities.Changed);
        }

        [Fact]
        public void ChangedShouldReturnTrueWhenRemovedListContainsEntities()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            entities.Remove(player);

            Assert.True(entities.Changed);
        }

        [Fact]
        public void ChangeShouldReturnTrueWhenAddedAndRemovedListsContainEntities()
        {
            var player1 = Container.Resolve<IPlayer>();
            var player2 = Container.Resolve<IPlayer>();

            var entities = new EntityCollection<IPlayer>();

            entities.Add(player1);
            entities.Remove(player2);

            Assert.True(entities.Changed);
        }

        [Fact]
        public void ChangedShouldReturnFalseWhenAddedAndRemovedListsAreEmpty()
        {
            var entities = new EntityCollection<IPlayer>();

            Assert.False(entities.Changed);
        }

        [Fact]
        public void RemovingReturnsTrueWhenRemovedListContainsEntity()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            entities.Remove(player);

            var removing = entities.Removing(player);

            Assert.True(removing);
        }

        [Fact]
        public void RemovingReturnsFalseWhenRemovedListDoesNotContainEntity()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            var removing = entities.Removing(player);

            Assert.False(removing);
        }

        [Fact]
        public void ContainsReturnsTrueWhenAddedListContainsEntity()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);

            var addedListContains = entities.Contains(player);

            Assert.True(addedListContains);
        }

        [Fact]
        public void ContainsReturnsTrueWhenKnownListContainsEntity()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);
            entities.Update();

            // Verify that player is no longer in the 'added' list.
            Assert.False(entities.AddedReadOnly.Contains(player));

            var knownListContains = entities.Contains(player);

            Assert.True(knownListContains);
        }

        [Fact]
        public void UpdateMovesEntitiesFromAddedToKnownList()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            entities.Add(player);
            entities.Update();

            // Verify that player is no longer in the 'added' list.
            Assert.False(entities.AddedReadOnly.Contains(player));

            // Verify that the 'known' list now contains an entity.
            Assert.True(entities.KnownReadOnly.Contains(player));
        }

        [Fact]
        public void UpdateRemovesEntitiesInRemovedListFromKnownList()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            // Add a player, update to put it in the 'known' list.
            entities.Add(player);
            entities.Update();

            // Remove the player, update to remove it from the 'known' list.
            entities.Remove(player);
            entities.Update();

            Assert.False(entities.KnownReadOnly.Contains(player));
        }

        [Fact]
        public void UpdateClearsAddedList()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            // Add a player, update to put it in the 'known' list.
            entities.Add(player);
            entities.Update();

            Assert.False(entities.AddedReadOnly.Contains(player));
        }

        [Fact]
        public void UpdateClearsRemovedList()
        {
            var player = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            // Add a player, update to put it in the 'known' list.
            entities.Add(player);
            entities.Update();

            // Remove the player, update to remove it from the 'known' list.
            entities.Remove(player);
            entities.Update();

            Assert.False(entities.RemovedReadOnly.Contains(player));
        }

        [Fact]
        public void AllPropertyReturnsEntitiesFromBothAddedAndKnownLists()
        {
            var player1 = Container.Resolve<IPlayer>();
            var player2 = Container.Resolve<IPlayer>();
            var entities = new EntityCollection<IPlayer>();

            // Add player1 and then update to move it to the 'known' list.
            entities.Add(player1);
            entities.Update();

            // Add player2 to the 'added' list.
            entities.Add(player2);

            var all = new HashSet<IPlayer>(entities.All);

            Assert.True(all.Contains(player1));
            Assert.True(all.Contains(player2));
        }
    }
}
