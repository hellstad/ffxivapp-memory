﻿// Bootstrapper
// FFXIVAPP & Related Plugins/Modules
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using FFXIVAPP.Memory;
using FFXIVAPP.Memory.Helpers;
using FFXIVAPP.Memory.Models;

namespace Bootstrapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actionInfo = ActionHelper.ActionInfo(2);
            Console.WriteLine(actionInfo.Name.English);
            var statusInfo = StatusEffectHelper.StatusInfo(2);
            Console.WriteLine(statusInfo.Name.English);
            var mapInfo = ZoneHelper.MapInfo(138);
            Console.WriteLine(mapInfo.Name.English);
            MemoryHandler.Instance.SetStructures(new ProcessModel
            {
                IsWin64 = true
            });
            Scanner.Instance.LoadOffsets(Signatures.Resolve(new ProcessModel
            {
                IsWin64 = true
            }));
            Console.WriteLine("To exit this application press \"Enter\".");
            Console.ReadLine();
        }
    }
}
