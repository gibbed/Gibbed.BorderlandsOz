﻿/* Copyright (c) 2014 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

#pragma warning disable 649

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gibbed.BorderlandsOz.GameInfo.Raw
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class AssetLibrarySet
    {
        public AssetLibrarySet()
        {
            this.Libraries = new Dictionary<AssetGroup, AssetLibraryDefinition>();
        }

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "libraries", Required = Required.Always)]
        public Dictionary<AssetGroup, AssetLibraryDefinition> Libraries { get; set; }
    }
}

#pragma warning restore 649
