﻿// FFXIVAPP.Memory
// Reader.Target.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Delegates;
using FFXIVAPP.Memory.Helpers;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        public static TargetReadResult GetTargetInfo()
        {
            var result = new TargetReadResult();

            if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("CHARMAP"))
            {
                try
                {
                    var targetHateStructure = IntPtr.Zero;
                    switch (MemoryHandler.Instance.GameLanguage)
                    {
                        case "Chinese":
                            targetHateStructure = (IntPtr) MemoryHandler.Instance.SigScanner.Locations["CHARMAP"] + 1136;
                            break;
                        default:
                            if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("ENMITYMAP"))
                            {
                                targetHateStructure = MemoryHandler.Instance.SigScanner.Locations["ENMITYMAP"];
                            }
                            break;
                    }
                    var enmityEntries = new List<EnmityEntry>();

                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("TARGET"))
                    {
                        var targetAddress = (IntPtr) MemoryHandler.Instance.SigScanner.Locations["TARGET"];
                        var somethingFound = false;
                        if (targetAddress.ToInt64() > 0)
                        {
                            //var targetInfo = MemoryHandler.Instance.GetStructure<Structures.Target>(targetAddress);
                            uint currentTarget = 0;
                            uint mouseOverTarget = 0;
                            uint focusTarget = 0;
                            uint previousTarget = 0;
                            uint currentTargetID = 0;
                            var targetInfoSource = MemoryHandler.Instance.GetByteArray(targetAddress, 192);
                            switch (MemoryHandler.Instance.GameLanguage)
                            {
                                case "Chinese":
                                    currentTarget = BitConverter.ToUInt32(targetInfoSource, 0x0);
                                    mouseOverTarget = BitConverter.ToUInt32(targetInfoSource, 0xC);
                                    focusTarget = BitConverter.ToUInt32(targetInfoSource, 0x3C);
                                    previousTarget = BitConverter.ToUInt32(targetInfoSource, 0x48);
                                    currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x5C);
                                    break;
                                default:
                                    currentTarget = BitConverter.ToUInt32(targetInfoSource, 0x0);
                                    if (MemoryHandler.Instance.ProcessModel.IsWin64)
                                    {
                                        mouseOverTarget = BitConverter.ToUInt32(targetInfoSource, 0x10);
                                        focusTarget = BitConverter.ToUInt32(targetInfoSource, 0x50);
                                        previousTarget = BitConverter.ToUInt32(targetInfoSource, 0x68);
                                        currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x80);
                                    }
                                    else
                                    {
                                        mouseOverTarget = BitConverter.ToUInt32(targetInfoSource, 0xC);
                                        focusTarget = BitConverter.ToUInt32(targetInfoSource, 0x38);
                                        previousTarget = BitConverter.ToUInt32(targetInfoSource, 0x44);
                                        currentTargetID = BitConverter.ToUInt32(targetInfoSource, 0x58);
                                    }
                                    break;
                            }
                            if (currentTarget > 0)
                            {
                                try
                                {
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(currentTarget), 0x23F0); // old size: 0x3F40
                                    var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                    currentTargetID = entry.ID;
                                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("MAP"))
                                    {
                                        try
                                        {
                                            entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(MemoryHandler.Instance.SigScanner.Locations["MAP"]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    if (entry.IsValid)
                                    {
                                        result.TargetsFound = true;
                                        result.TargetEntity.CurrentTarget = entry;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (mouseOverTarget > 0)
                            {
                                try
                                {
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(mouseOverTarget), 0x23F0); // old size: 0x3F40
                                    var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("MAP"))
                                    {
                                        try
                                        {
                                            entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(MemoryHandler.Instance.SigScanner.Locations["MAP"]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    if (entry.IsValid)
                                    {
                                        result.TargetsFound = true;
                                        result.TargetEntity.MouseOverTarget = entry;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (focusTarget > 0)
                            {
                                var source = MemoryHandler.Instance.GetByteArray(new IntPtr(focusTarget), 0x23F0); // old size: 0x3F40
                                var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("MAP"))
                                {
                                    try
                                    {
                                        entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(MemoryHandler.Instance.SigScanner.Locations["MAP"]);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                if (entry.IsValid)
                                {
                                    result.TargetsFound = true;
                                    result.TargetEntity.FocusTarget = entry;
                                }
                            }
                            if (previousTarget > 0)
                            {
                                try
                                {
                                    var source = MemoryHandler.Instance.GetByteArray(new IntPtr(previousTarget), 0x23F0); // old size: 0x3F40
                                    var entry = ActorEntityHelper.ResolveActorFromBytes(source);
                                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("MAP"))
                                    {
                                        try
                                        {
                                            entry.MapIndex = (uint) MemoryHandler.Instance.GetPlatformUInt(MemoryHandler.Instance.SigScanner.Locations["MAP"]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    if (entry.IsValid)
                                    {
                                        result.TargetsFound = true;
                                        result.TargetEntity.PreviousTarget = entry;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (currentTargetID > 0)
                            {
                                result.TargetsFound = true;
                                result.TargetEntity.CurrentTargetID = currentTargetID;
                            }
                        }
                        if (result.TargetEntity.CurrentTargetID > 0 && targetHateStructure.ToInt64() > 0)
                        {
                            for (uint i = 0; i < 16; i++)
                            {
                                try
                                {
                                    var address = targetHateStructure.ToInt64() + (i * 72);
                                    var enmityEntry = new EnmityEntry
                                    {
                                        Name = MemoryHandler.Instance.GetString(new IntPtr(address)),
                                        ID = (uint) MemoryHandler.Instance.GetPlatformInt(new IntPtr(address), 64),
                                        Enmity = (uint) MemoryHandler.Instance.GetPlatformInt(new IntPtr(address), 68)
                                    };
                                    if (enmityEntry.ID <= 0)
                                    {
                                        continue;
                                    }
                                    if (String.IsNullOrWhiteSpace(enmityEntry.Name))
                                    {
                                        var pc = PCWorkerDelegate.GetEntity(enmityEntry.ID);
                                        var npc = NPCWorkerDelegate.GetEntity(enmityEntry.ID);
                                        var monster = MonsterWorkerDelegate.GetEntity(enmityEntry.ID);
                                        try
                                        {
                                            enmityEntry.Name = (pc ?? npc).Name ?? monster.Name;
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    enmityEntries.Add(enmityEntry);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                        result.TargetEntity.EnmityEntries = enmityEntries;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return result;
        }

        public class TargetReadResult
        {
            public TargetReadResult()
            {
                TargetEntity = new TargetEntity();
            }

            public TargetEntity TargetEntity { get; set; }
            public bool TargetsFound { get; set; }
        }
    }
}