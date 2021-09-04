#region Licence
/**
* Copyright © 2014-2019 OTTools <https://github.com/ottools/ItemEditor/>
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; either version 2 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along
* with this program; if not, write to the Free Software Foundation, Inc.,
* 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
#endregion

#region Using Statements
using OTLib.Server.Items;
using OTLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
#endregion

namespace ItemEditor
{
    public class Item
    {
        #region Private Properties

        protected byte[] spriteHash = null;

        #endregion

        #region Constructor

        public Item()
        {
            this.Type = ServerItemType.None;
            this.StackOrder = TileStackOrder.None;
            this.Movable = true;
            this.Name = string.Empty;
        }

        #endregion

        #region Public Properties

        public ushort ID { get; set; }

        public ServerItemType Type { get; set; }

        public bool HasStackOrder { get; set; }

        public TileStackOrder StackOrder { get; set; }

        public bool Unpassable { get; set; }

        public bool BlockMissiles { get; set; }

        public bool BlockPathfinder { get; set; }

        public bool HasElevation { get; set; }

        public bool ForceUse { get; set; }

        public bool MultiUse { get; set; }

        public bool Pickupable { get; set; }

        public bool Movable { get; set; }

        public bool Stackable { get; set; }

        public bool Readable { get; set; }

        public bool Rotatable { get; set; }

        public bool Hangable { get; set; }

        public bool HookSouth { get; set; }

        public bool HookEast { get; set; }

        public bool HasCharges { get; set; }

        public bool IgnoreLook { get; set; }

        public bool FullGround { get; set; }

        public bool AllowDistanceRead { get; set; }

        public bool IsAnimation { get; set; }

        public ushort GroundSpeed { get; set; }

        public ushort LightLevel { get; set; }

        public ushort LightColor { get; set; }

        public ushort MaxReadChars { get; set; }

        public ushort MaxReadWriteChars { get; set; }

        public ushort MinimapColor { get; set; }

        public ushort TradeAs { get; set; }

        public string Name { get; set; }

        // used to find sprites during updates
        public virtual byte[] SpriteHash
        {
            get
            {
                return this.spriteHash;
            }

            set
            {
                this.spriteHash = value;
            }
        }

        #endregion

        #region Public Methods

        public bool Equals(Item item)
        {
            if (this.Type != item.Type ||
                this.StackOrder != item.StackOrder ||
                this.Unpassable != item.Unpassable ||
                this.BlockMissiles != item.BlockMissiles ||
                this.BlockPathfinder != item.BlockPathfinder ||
                this.HasElevation != item.HasElevation ||
                this.ForceUse != item.ForceUse ||
                this.MultiUse != item.MultiUse ||
                this.Pickupable != item.Pickupable ||
                this.Movable != item.Movable ||
                this.Stackable != item.Stackable ||
                this.Readable != item.Readable ||
                this.Rotatable != item.Rotatable ||
                this.Hangable != item.Hangable ||
                this.HookSouth != item.HookSouth ||
                this.HookEast != item.HookEast ||
                this.IgnoreLook != item.IgnoreLook ||
                this.FullGround != item.FullGround ||
                this.IsAnimation != item.IsAnimation ||
                this.GroundSpeed != item.GroundSpeed ||
                this.LightLevel != item.LightLevel ||
                this.LightColor != item.LightColor ||
                this.MaxReadChars != item.MaxReadChars ||
                this.MaxReadWriteChars != item.MaxReadWriteChars ||
                this.MinimapColor != item.MinimapColor ||
                this.TradeAs != item.TradeAs)
            {
                return false;
            }

            if (this.Name.CompareTo(item.Name) != 0)
            {
                return false;
            }

            return true;
        }

        public bool HasProperties(ServerItemFlag properties)
        {
            if (properties == ServerItemFlag.None) return false;
            if (properties.HasFlag(ServerItemFlag.Unpassable) && !this.Unpassable) return false;
            if (properties.HasFlag(ServerItemFlag.BlockMissiles) && !this.BlockMissiles) return false;
            if (properties.HasFlag(ServerItemFlag.BlockPathfinder) && !this.BlockPathfinder) return false;
            if (properties.HasFlag(ServerItemFlag.HasElevation) && !this.HasElevation) return false;
            if (properties.HasFlag(ServerItemFlag.ForceUse) && !this.ForceUse) return false;
            if (properties.HasFlag(ServerItemFlag.MultiUse) && !this.MultiUse) return false;
            if (properties.HasFlag(ServerItemFlag.Pickupable) && !this.Pickupable) return false;
            if (properties.HasFlag(ServerItemFlag.Movable) && !this.Movable) return false;
            if (properties.HasFlag(ServerItemFlag.Stackable) && !this.Stackable) return false;
            if (properties.HasFlag(ServerItemFlag.Readable) && !this.Readable) return false;
            if (properties.HasFlag(ServerItemFlag.Rotatable) && !this.Rotatable) return false;
            if (properties.HasFlag(ServerItemFlag.Hangable) && !this.Hangable) return false;
            if (properties.HasFlag(ServerItemFlag.HookSouth) && !this.HookSouth) return false;
            if (properties.HasFlag(ServerItemFlag.HookEast) && !this.HookEast) return false;
            if (properties.HasFlag(ServerItemFlag.AllowDistanceRead) && !this.AllowDistanceRead) return false;
            if (properties.HasFlag(ServerItemFlag.IgnoreLook) && !this.IgnoreLook) return false;
            if (properties.HasFlag(ServerItemFlag.FullGround) && !this.FullGround) return false;
            if (properties.HasFlag(ServerItemFlag.IsAnimation) && !this.IsAnimation) return false;
            return true;
        }

        public Item CopyPropertiesFrom(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Type type = this.GetType();

            foreach (PropertyInfo property in item.GetType().GetProperties())
            {
                if (property.Name == "SpriteHash")
                {
                    continue;
                }

                PropertyInfo targetProperty = type.GetProperty(property.Name);
                if (targetProperty == null)
                {
                    continue;
                }

                if (!targetProperty.CanWrite)
                {
                    continue;
                }

                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }

                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }

                if (!targetProperty.PropertyType.IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }

                targetProperty.SetValue(this, property.GetValue(item, null), null);
            }

            return this;
        }

        #endregion
    }
}
