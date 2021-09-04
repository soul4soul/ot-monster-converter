﻿#region Licence
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
using System;
#endregion

namespace OTLib.Server.Items
{
    public enum ServerItemAttribute : byte
    {
        ServerID = 0x10,
        ClientID = 0x11,
        Name = 0x12,
        GroundSpeed = 0x14,
        SpriteHash = 0x20,
        MinimaColor = 0x21,
        MaxReadWriteChars = 0x22,
        MaxReadChars = 0x23,
        Light = 0x2A,
        StackOrder = 0x2B,
        TradeAs = 0x2D
    }
}
