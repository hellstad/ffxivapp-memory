﻿// FFXIVAPP.Memory
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

using System.Collections.Concurrent;
using System.Linq;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory.Helpers
{
    public static class StatusEffectHelper
    {
        private static ConcurrentDictionary<uint, StatusItem> _statusEffects;

        private static ConcurrentDictionary<uint, StatusItem> StatusEffects
        {
            get { return _statusEffects ?? (_statusEffects = new ConcurrentDictionary<uint, StatusItem>()); }
            set
            {
                if (_statusEffects == null)
                {
                    _statusEffects = new ConcurrentDictionary<uint, StatusItem>();
                }
                _statusEffects = value;
            }
        }

        public static StatusItem StatusInfo(uint id)
        {
            lock (StatusEffects)
            {
                if (!StatusEffects.Any())
                {
                    Generate();
                }
                if (StatusEffects.ContainsKey(id))
                {
                    return StatusEffects[id];
                }
                return new StatusItem
                {
                    Name = new Localization
                    {
                        Chinese = "???",
                        English = "???",
                        French = "???",
                        German = "???",
                        Japanese = "???",
                        Korean = "???"
                    },
                    CompanyAction = false
                };
            }
        }

        private static void Generate()
        {
            APIHelper.GetStatusEffects(StatusEffects);
        }
    }
}
