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

using System;
using System.Linq;
using Gibbed.BorderlandsOz.GameInfo;

namespace Gibbed.BorderlandsOz.FileFormats
{
    public static class AssetLibraryManagerHelpers
    {
        private static bool GetIndex(this AssetLibraryManager assetLibraryManager,
                                     Platform platform,
                                     int setId,
                                     AssetGroup group,
                                     string package,
                                     string asset,
                                     out uint index)
        {
            var config = assetLibraryManager.Configurations[group];

            var set = assetLibraryManager.GetSet(setId);
            var library = set.Libraries[group];

            var sublibrary =
                library.Sublibraries.FirstOrDefault(sl => sl.Package == package && sl.Assets.Contains(asset) == true);
            if (sublibrary == null)
            {
                index = 0;
                return false;
            }
            var sublibraryIndex = library.Sublibraries.IndexOf(sublibrary);
            var assetIndex = sublibrary.Assets.IndexOf(asset);

            var platformConfig = InfoManager.PlatformConfigurations.GetOrDefault(platform);
            if (platformConfig != null)
            {
                var platformSet = platformConfig.GetSet(setId);
                if (platformSet != null &&
                    platformSet.Libraries.ContainsKey(group) == true)
                {
                    var platformLibrary = platformSet.Libraries[group];
                    if (platformLibrary.SublibraryRemappingAtoB != null &&
                        platformLibrary.SublibraryRemappingAtoB.Count > 0)
                    {
                        if (platformLibrary.SublibraryRemappingAtoB.ContainsKey(sublibraryIndex) == false)
                        {
                            throw new InvalidOperationException(
                                string.Format("don't know how to remap sublibrary {0} for set {1}!",
                                              sublibraryIndex,
                                              setId));
                        }
                        sublibraryIndex = platformLibrary.SublibraryRemappingAtoB[sublibraryIndex];
                    }
                }
            }

            index = 0;
            index |= (((uint)assetIndex) & config.AssetMask) << 0;
            index |= (((uint)sublibraryIndex) & config.SublibraryMask) << config.AssetBits;

            if (setId != 0)
            {
                index |= 1u << config.AssetBits + config.SublibraryBits - 1;
            }

            return true;
        }

        public static void Encode(this AssetLibraryManager assetLibraryManager,
                                  BitWriter writer,
                                  Platform platform,
                                  int setId,
                                  AssetGroup group,
                                  string value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (string.IsNullOrEmpty(value) == true)
            {
                throw new ArgumentNullException("value");
            }

            var config = assetLibraryManager.Configurations[group];

            uint index;
            if (value == "None")
            {
                index = config.NoneIndex;
            }
            else
            {
                var parts = value.Split(new[]
                {
                    '.'
                },
                                        2);
                if (parts.Length != 2)
                {
                    throw new ArgumentException();
                }

                var package = parts[0];
                var asset = parts[1];
                if (assetLibraryManager.GetIndex(platform, setId, group, package, asset, out index) == false)
                {
                    if (assetLibraryManager.GetIndex(platform, 0, group, package, asset, out index) == false)
                    {
                        throw new ArgumentException("unsupported asset");
                    }
                }
            }

            writer.WriteUInt32(index, config.SublibraryBits + config.AssetBits);
        }

        public static string Decode(this AssetLibraryManager assetLibraryManager,
                                    BitReader reader,
                                    Platform platform,
                                    int setId,
                                    AssetGroup group)
        {
            var config = assetLibraryManager.Configurations[group];

            var index = reader.ReadUInt32(config.SublibraryBits + config.AssetBits);
            if (index == config.NoneIndex)
            {
                return "None";
            }

            var assetIndex = (int)((index >> 0) & config.AssetMask);
            var sublibraryIndex = (int)((index >> config.AssetBits) & config.SublibraryMask);
            var useSetId = ((index >> config.AssetBits) & config.UseSetIdMask) != 0;
            var actualSetId = useSetId == false ? 0 : setId;

            var platformConfig = InfoManager.PlatformConfigurations.GetOrDefault(platform);
            if (platformConfig != null)
            {
                var platformSet = platformConfig.GetSet(actualSetId);
                if (platformSet != null &&
                    platformSet.Libraries.ContainsKey(group) == true)
                {
                    var platformLibrary = platformSet.Libraries[group];
                    if (platformLibrary.SublibraryRemappingBtoA != null &&
                        platformLibrary.SublibraryRemappingBtoA.Count > 0)
                    {
                        if (platformLibrary.SublibraryRemappingBtoA.ContainsKey(sublibraryIndex) == false)
                        {
                            throw new InvalidOperationException(
                                string.Format("don't know how to remap sublibrary {0} for set {1}!",
                                              sublibraryIndex,
                                              actualSetId));
                        }
                        sublibraryIndex = platformLibrary.SublibraryRemappingBtoA[sublibraryIndex];
                    }
                }
            }

            var set = assetLibraryManager.GetSet(actualSetId);
            var library = set.Libraries[group];

            if (sublibraryIndex < 0 || sublibraryIndex >= library.Sublibraries.Count)
            {
                throw new ArgumentOutOfRangeException(string.Format("invalid sublibrary index {1} in set {0}",
                                                                    sublibraryIndex,
                                                                    set.Id));
            }

            return library.Sublibraries[sublibraryIndex].GetAsset(assetIndex);
        }
    }
}
