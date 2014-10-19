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
    internal sealed class ItemTypeDefinition
    {
        internal ItemTypeDefinition()
        {
            this.Titles = new List<string>();
            this.Prefixes = new List<string>();
            this.AlphaParts = new List<string>();
            this.BetaParts = new List<string>();
            this.GammaParts = new List<string>();
            this.DeltaParts = new List<string>();
            this.EpsilonParts = new List<string>();
            this.ZetaParts = new List<string>();
            this.EtaParts = new List<string>();
            this.ThetaParts = new List<string>();
            this.MaterialParts = new List<string>();
        }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public ItemType Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "has_full_name")]
        public bool HasFullName { get; set; }

        [JsonProperty(PropertyName = "titles")]
        public List<string> Titles { get; set; }

        [JsonProperty(PropertyName = "prefixes")]
        public List<string> Prefixes { get; set; }

        [JsonProperty(PropertyName = "alpha_parts")]
        public List<string> AlphaParts { get; set; }

        [JsonProperty(PropertyName = "beta_parts")]
        public List<string> BetaParts { get; set; }

        [JsonProperty(PropertyName = "gamma_parts")]
        public List<string> GammaParts { get; set; }

        [JsonProperty(PropertyName = "delta_parts")]
        public List<string> DeltaParts { get; set; }

        [JsonProperty(PropertyName = "epsilon_parts")]
        public List<string> EpsilonParts { get; set; }

        [JsonProperty(PropertyName = "zeta_parts")]
        public List<string> ZetaParts { get; set; }

        [JsonProperty(PropertyName = "eta_parts")]
        public List<string> EtaParts { get; set; }

        [JsonProperty(PropertyName = "theta_parts")]
        public List<string> ThetaParts { get; set; }

        [JsonProperty(PropertyName = "material_parts")]
        public List<string> MaterialParts { get; set; }
    }
}

#pragma warning restore 649
