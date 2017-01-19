using OpenClassic.Server.Domain;
using System;
using System.Diagnostics;
using System.Text;

namespace OpenClassic.Server.Networking.Rscd
{
    public class RscdPacketWriter : AbstractPacketWriter
    {
        public void SendServerInfo(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 110);
            WriteLong(session, DateTime.Now.Ticks);
            WriteBytes(session, Encoding.UTF8.GetBytes("Australia"));
            FormatPacket(session);
        }

        public void SendFatigue(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 126);
            WriteShort(session, 50);
            FormatPacket(session);
        }

        public void SendWorldInfo(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 131);

            var location = session.Player.Location;

            WriteShort(session, session.Player.Index);
            WriteShort(session, 2304);
            WriteShort(session, 1776);
            WriteShort(session, 0);
            WriteShort(session, World.WorldWidth);
            FormatPacket(session);
        }

        public void SendInventory(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 114);

            WriteByte(session, 2); // Number of items in inventory

            // TODO: Loop through the inventory here instead of just sending one item.
            WriteShort(session, 10); // ID for coins
            WriteInt(session, 1337); // Amount

            WriteShort(session, 11); // ID for bronze arrows
            WriteInt(session, 1337); // Amount
            // End loop

            FormatPacket(session);
        }

        public void SendBank(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 93);

            var itemCount = 1;
            var maxItems = byte.MaxValue;

            WriteByte(session, itemCount); // Bank item count
            WriteByte(session, maxItems); // Max number of bank items

            WriteShort(session, 10); // Item id
            WriteInt(session, 1337); // Item amount

            FormatPacket(session);
        }

        public void SendCombatStyle(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 129);
            WriteByte(session, 2); // Combat style - 0 to 3 inclusive.
            FormatPacket(session);
        }

        public void SendShowAppearanceScreen(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 207);
            FormatPacket(session);
        }

        public void SendCloseShop(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 220);
            FormatPacket(session);
        }

        public void SendClientConfig(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 152);

            WriteByte(session, 1); // Auto camera angle
            WriteByte(session, 0); // Mouse button setting
            WriteByte(session, 1); // Sound effects
            WriteByte(session, 1); // Show roofs
            WriteByte(session, 1); // Enable automatic screenshots
            WriteByte(session, 1); // Always show combat type window.

            FormatPacket(session);
        }

        public void SendStats(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 180);

            const int statCount = 18;

            for (var i = 0; i < statCount; i++)
            {
                const int currentLevel = 50;
                WriteByte(session, currentLevel);
            }

            for (var i = 0; i < statCount; i++)
            {
                const int baseLevel = 50;
                WriteByte(session, baseLevel);
            }

            for (var i = 0; i < statCount; i++)
            {
                const int experience = 100000;
                WriteInt(session, experience);
            }

            FormatPacket(session);
        }

        public void SendLoginBox(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 248);

            const int daysSinceLastLogin = 180;
            WriteShort(session, daysSinceLastLogin);

            const int subscriptionDaysLeft = 30;
            WriteShort(session, subscriptionDaysLeft);

            WriteBytes(session, Encoding.UTF8.GetBytes("127.0.0.1"));

            FormatPacket(session);
        }

        public void SendCantLogout(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 136);
            FormatPacket(session);
        }

        public void SendLogout(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 222);
            FormatPacket(session);
        }

        public void SendDied(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 165);
            FormatPacket(session);
        }

        public void SendPrayers(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 209);
            // TODO: Finish
        }

        #region Update packets

        public void SendPlayerPositionUpdate(ISession session)
        {
            Debug.Assert(session != null);

            CreatePacket(session, 145);

            var player = session.Player;
            var loc = player.Location;

            WriteBits(session, loc.X, 11);
            WriteBits(session, loc.Y, 13);
            WriteBits(session, 7, 4); // sprite
            WriteBits(session, 0, 8); // number of known players

            // TODO: Loop through each known player, send update information

            // TODO: Loop through each new player, send update information

            FormatPacket(session);
        }

        public void SendNpcPositionUpdate(ISession session)
        {
            Debug.Assert(session != null);

            var player = session.Player;
            var watchedNpcs = player.WatchedNpcs;

            var newNpcs = watchedNpcs.AddedReadOnly;
            var knownNpcs = watchedNpcs.KnownReadOnly;

            CreatePacket(session, 77);
            WriteBits(session, knownNpcs.Count, 8); // Number of known NPCs

            // TODO: Loop through known NPCs here.
            foreach (var npc in knownNpcs)
            {
                WriteBits(session, npc.Index, 16);
                if (watchedNpcs.Removing(npc))
                {
                    WriteBits(session, 1, 1);
                    WriteBits(session, 1, 1);
                    WriteBits(session, 12, 4);
                }
                //else if (n.hasMoved())
                //{
                //    packet.addBits(1, 1);
                //    packet.addBits(0, 1);
                //    packet.addBits(n.getSprite(), 3);
                //}
                //else if (n.spriteChanged())
                //{
                //    packet.addBits(1, 1);
                //    packet.addBits(1, 1);
                //    packet.addBits(n.getSprite(), 4);
                //}
                else
                {
                    WriteBits(session, 0, 1);
                }
            }

            // TODO: Loop through each new NPC here
            foreach (var npc in newNpcs)
            {
                var xOffset = GetMobOffset(npc.Location.X, player.Location.X);
                var yOffset = GetMobOffset(npc.Location.Y, player.Location.Y);

                WriteBits(session, npc.Index, 16);
                WriteBits(session, xOffset, 5);
                WriteBits(session, yOffset, 5);
                WriteBits(session, 1, 4); // Sprite, default value = 1
                WriteBits(session, npc.Id, 10);

                //byte[] offsets = DataConversions.getMobPositionOffsets(n.getLocation(), playerToUpdate.getLocation());
                //packet.addBits(n.getIndex(), 16);
                //packet.addBits(offsets[0], 5);
                //packet.addBits(offsets[1], 5);
                //packet.addBits(n.getSprite(), 4);
                //packet.addBits(n.getID(), 10);
            }

            FormatPacket(session);
        }

        private byte GetMobOffset(short ord1, short ord2)
        {
            var offset = (byte)(ord1 - ord2);
            if (ord2 > ord1)
            {
                return (byte)(ord1 - ord2 + 32);
            }
            else
            {
                return (byte)(ord1 - ord2);
            }
        }

        private byte GetObjectOffset(short ord1, short ord2)
        {
            return (byte)(ord1 - ord2);
        }

        //public static byte[] getMobPositionOffsets(Point p1, Point p2)
        //{
        //    byte[] rv = new byte[2];
        //    rv[0] = getMobCoordOffset(p1.getX(), p2.getX());
        //    rv[1] = getMobCoordOffset(p1.getY(), p2.getY());
        //    return rv;
        //}

        //private static byte getMobCoordOffset(int coord1, int coord2)
        //{
        //    byte offset = (byte)(coord1 - coord2);
        //    if (offset < 0)
        //    {
        //        offset += 32;
        //    }
        //    return offset;
        //}

        public void SendGameObjectUpdate(ISession session)
        {
            Debug.Assert(session != null);

            // TODO: Implement the 'send game objects' packet.
            // Note: Only send this packet if the player's watched game objects have changed.

            var player = session.Player;
            var watchedObjects = player.WatchedObjects;

            if (watchedObjects.Changed) // If the player's visible game objects have changed...
            {
                CreatePacket(session, 27);

                foreach (var obj in watchedObjects.KnownReadOnly)
                {
                    if (obj.Type != 0) continue;

                    if (watchedObjects.Removing(obj))
                    {
                        var xOffset = GetObjectOffset(obj.Location.X, player.Location.X);
                        var yOffset = GetObjectOffset(obj.Location.Y, player.Location.Y);

                        WriteShort(session, 60000);
                        WriteByte(session, xOffset);
                        WriteByte(session, yOffset);
                        WriteByte(session, obj.Direction);
                    }
                }

                foreach (var obj in watchedObjects.AddedReadOnly)
                {
                    if (obj.Type != 0) continue;

                    var xOffset = GetObjectOffset(obj.Location.X, player.Location.X);
                    var yOffset = GetObjectOffset(obj.Location.Y, player.Location.Y);

                    WriteShort(session, obj.Id);
                    WriteByte(session, xOffset);
                    WriteByte(session, yOffset);
                    WriteByte(session, obj.Direction);
                }

                FormatPacket(session);
            }
        }

        public void SendWallObjectUpdate(ISession session)
        {
            Debug.Assert(session != null);

            // TODO: Implement the 'send wall objects' packet.
            // Note: Only send this packet if the player's watched wall objects have changed.

            var player = session.Player;
            var watchedObjects = player.WatchedObjects;

            if (watchedObjects.Changed) // If the player's visible wall objects have changed...
            {
                CreatePacket(session, 95);

                foreach (var obj in watchedObjects.KnownReadOnly)
                {
                    if (obj.Type != 1) continue;

                    if (watchedObjects.Removing(obj))
                    {
                        var xOffset = GetObjectOffset(obj.Location.X, player.Location.X);
                        var yOffset = GetObjectOffset(obj.Location.Y, player.Location.Y);

                        WriteShort(session, 60000);
                        WriteByte(session, xOffset);
                        WriteByte(session, yOffset);
                        WriteByte(session, obj.Direction);
                    }
                }

                foreach (var obj in watchedObjects.AddedReadOnly)
                {
                    if (obj.Type != 1) continue;

                    var xOffset = GetObjectOffset(obj.Location.X, player.Location.X);
                    var yOffset = GetObjectOffset(obj.Location.Y, player.Location.Y);

                    WriteShort(session, obj.Id);
                    WriteByte(session, xOffset);
                    WriteByte(session, yOffset);
                    WriteByte(session, obj.Direction);
                }

                FormatPacket(session);
            }
        }

        public void SendItemUpdate(ISession session)
        {
            Debug.Assert(session != null);

            // TODO: Implement the 'send item update' packet.
            // Note: Only send this packet if the player's watched items have changed.

            if (false) // If the player's visible items have changed...
            {
#pragma warning disable CS0162 // Unreachable code detected
                CreatePacket(session, 109);
#pragma warning restore CS0162 // Unreachable code detected

                // TODO: Loop through known items, remove any that are no longer visible.

                // TODO: Loop through any new items.

                FormatPacket(session);
            }
        }

        public void SendPlayerAppearanceUpdate(ISession session)
        {
            Debug.Assert(session != null);

            // TODO: Implement the 'send player appearance update' packet.
            // Note: This packet is only sent if something about the player's worldview has changed.
            // Changes include: Bubbles, chat messages, hit updates, projectiles, and player appearances.

            var player = session.Player;
            if (player.AppearanceUpdateRequired)
            {
#pragma warning disable CS0162 // Unreachable code detected
                CreatePacket(session, 53);
#pragma warning restore CS0162 // Unreachable code detected

                WriteShort(session, 1); // Update size.

                // TODO: Implement the following:
                // 1. Loop through bubbles
                // 2. Loop through chat messages
                // 3. Loop through players requiring hit (damage) updates
                // 4. Loop through projectiles
                // 5. Loop through player appearance updates

                WriteShort(session, player.Index);
                WriteByte(session, 5); // Update type of 5 is for appearance updates
                WriteShort(session, 10); // Appearance id
                WriteLong(session, 23277428); // Username hash 'Lothy'.

                var sprites = player.GetSprites();
                WriteByte(session, sprites.Length);
                foreach (var sprite in sprites)
                {
                    WriteByte(session, sprite);
                }

                WriteByte(session, player.HairColour);
                WriteByte(session, player.TopColour);
                WriteByte(session, player.TrouserColour);
                WriteByte(session, player.SkinColour);
                WriteByte(session, 3); // Combat level
                WriteByte(session, 1); // 1 for skulled, 0 for not skulled
                WriteByte(session, 3); // Crown - Value of 3/2/1

                FormatPacket(session);
            }
        }

        public void SendNpcAppearanceUpdate(ISession session)
        {
            Debug.Assert(session != null);

            // TODO: Implement the 'send NPC appearance update' packet.
            // Note: This packet is only sent if NPC chat messages or damage hits need to be displayed.

            if (false)
            {
#pragma warning disable CS0162 // Unreachable code detected
                CreatePacket(session, 190);
#pragma warning restore CS0162 // Unreachable code detected

                WriteShort(session, 0); // Update size.

                // TODO: Loop through NPC chat messages.
                // TODO: Loop through NPC hit updates.

                FormatPacket(session);
            }
        }

        #endregion
    }
}
