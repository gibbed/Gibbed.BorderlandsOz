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
using Gibbed.BorderlandsOz.GameInfo;
using Gibbed.IO;

namespace Gibbed.BorderlandsOz.FileFormats.Items
{
    public abstract class PackedDataHelper<TWeapon, TItem>
        where TWeapon : IPackableWeapon, new()
        where TItem : IPackableItem, new()
    {
        private static byte[] Encode(IPackedSlot packed,
                                     int uniqueId,
                                     bool isWeapon,
                                     int assetLibrarySetId,
                                     Platform platform)
        {
            var writer = new BitWriter();
            writer.WriteInt32(InfoManager.AssetLibraryManager.Version, 7);
            writer.WriteBoolean(isWeapon);
            writer.WriteInt32(uniqueId, 32);
            writer.WriteUInt16(0xFFFF, 16);
            writer.WriteInt32(assetLibrarySetId, 8);
            packed.Write(writer, platform);

            var unobfuscatedBytes = writer.GetBuffer();
            if (unobfuscatedBytes.Length > 40)
            {
                throw new InvalidOperationException();
            }

            if (unobfuscatedBytes.Length < 40)
            {
                var start = unobfuscatedBytes.Length;
                Array.Resize(ref unobfuscatedBytes, 40);
                for (int i = start; i < unobfuscatedBytes.Length; i++)
                {
                    unobfuscatedBytes[i] = 0xFF;
                }
            }

            var hash = CRC32.Hash(unobfuscatedBytes, 0, unobfuscatedBytes.Length);
            var computedCheck = (ushort)(((hash & 0xFFFF0000) >> 16) ^ ((hash & 0x0000FFFF) >> 0));

            unobfuscatedBytes[5] = (byte)((computedCheck & 0xFF00) >> 8);
            unobfuscatedBytes[6] = (byte)((computedCheck & 0x00FF) >> 0);

            for (int i = unobfuscatedBytes.Length - 1; i > 5; i--)
            {
                if (unobfuscatedBytes[i] != 0xFF)
                {
                    Array.Resize(ref unobfuscatedBytes, i + 1);
                    break;
                }
            }

            var data = (byte[])unobfuscatedBytes.Clone();
            var seed = BitConverter.ToUInt32(data, 1).Swap();
            BogoEncrypt(seed, data, 5, data.Length - 5);
            return data;
        }

        public static byte[] Encode(IPackableSlot packable, Platform platform)
        {
            if (packable == null)
            {
                throw new ArgumentNullException("packable");
            }

            /*
            var assetLibrarySet =
                InfoManager.AssetLibraryManager.Sets.SingleOrDefault(s => s.Id == packable.AssetLibrarySetId);
            if (assetLibrarySet == null)
            {
                throw new ArgumentException("unsupported asset library set");
            }
            */

            var weapon = packable as IPackableWeapon;
            if (weapon != null)
            {
                var packed = weapon.Pack(platform);
                return Encode(packed, weapon.UniqueId, true, weapon.AssetLibrarySetId, platform);
            }

            var item = packable as IPackableItem;
            if (item != null)
            {
                var packed = item.Pack(platform);
                return Encode(packed, item.UniqueId, false, item.AssetLibrarySetId, platform);
            }

            throw new NotSupportedException();
        }

        private static void BogoEncrypt(uint seed, byte[] buffer, int offset, int length)
        {
            if (seed == 0)
            {
                return;
            }

            var temp = new byte[length];

            var rightHalf = (int)((seed % 32) % length);
            var leftHalf = length - rightHalf;

            Array.Copy(buffer, offset, temp, leftHalf, rightHalf);
            Array.Copy(buffer, offset + rightHalf, temp, 0, leftHalf);

            var xor = (uint)((int)seed >> 5);
            for (int i = 0; i < length; i++)
            {
                // ReSharper disable RedundantCast
                xor = (uint)(((ulong)xor * 0x10A860C1UL) % 0xFFFFFFFBUL);
                // ReSharper restore RedundantCast
                temp[i] ^= (byte)(xor & 0xFF);
            }

            Array.Copy(temp, 0, buffer, offset, length);
        }

        public static IPackableSlot Decode(byte[] data, Platform platform)
        {
            if (data.Length < 5 || data.Length > 40)
            {
                throw new ArgumentOutOfRangeException("data");
            }

            var seed = BitConverter.ToUInt32(data, 1).Swap();
            var unobfuscatedBytes = (byte[])data.Clone();
            BogoDecrypt(seed, unobfuscatedBytes, 5, unobfuscatedBytes.Length - 5);

            var fileCheck = BitConverter.ToUInt16(unobfuscatedBytes, 5).Swap();

            unobfuscatedBytes[5] = 0xFF;
            unobfuscatedBytes[6] = 0xFF;

            if (unobfuscatedBytes.Length < 40)
            {
                var start = unobfuscatedBytes.Length;
                Array.Resize(ref unobfuscatedBytes, 40);
                for (int i = start; i < unobfuscatedBytes.Length; i++)
                {
                    unobfuscatedBytes[i] = 0xFF;
                }
            }

            var hash = CRC32.Hash(unobfuscatedBytes, 0, unobfuscatedBytes.Length);
            var computedCheck = (ushort)(((hash & 0xFFFF0000) >> 16) ^ ((hash & 0x0000FFFF) >> 0));

            if (fileCheck != computedCheck)
            {
                throw new FormatException("checksum failure in packed data");
            }

            var reader = new BitReader(unobfuscatedBytes);

            var version = reader.ReadInt32(7);
            if (version != InfoManager.AssetLibraryManager.Version)
            {
                throw new FormatException("invalid version in packed data");
            }

            var isWeapon = reader.ReadBoolean();

            int uniqueId = 0;
            if (version >= 3)
            {
                uniqueId = reader.ReadInt32(32);
            }

            ushort check = reader.ReadUInt16(16);
            if (check != 0xFFFF)
            {
                throw new FormatException("invalid check in packed data");
            }

            int setId = 0;
            if (version >= 2)
            {
                setId = reader.ReadInt32(8);
            }

            /*
            var set = InfoManager.AssetLibraryManager.GetSet(setId);
            if (set == null)
            {
                throw new FormatException(
                    string.Format(
                        "unknown asset library set {0} in packed data (this generally means new DLC that is not supported yet)",
                        setId));
            }
            */

            if (isWeapon == true)
            {
                var packed = new PackedWeapon();
                packed.Read(reader, platform);

                var weapon = new TWeapon
                {
                    UniqueId = uniqueId,
                    AssetLibrarySetId = setId,
                };
                weapon.Unpack(packed, platform);
                return weapon;
            }
            else
            {
                var packed = new PackedItem();
                packed.Read(reader, platform);

                var item = new TItem
                {
                    UniqueId = uniqueId,
                    AssetLibrarySetId = setId,
                };
                item.Unpack(packed, platform);
                return item;
            }
        }

        private static void BogoDecrypt(uint seed, byte[] buffer, int offset, int length)
        {
            if (seed == 0)
            {
                return;
            }

            var temp = new byte[length];
            Array.Copy(buffer, offset, temp, 0, length);

            var xor = (uint)((int)seed >> 5);
            for (int i = 0; i < length; i++)
            {
                // ReSharper disable RedundantCast
                xor = (uint)(((ulong)xor * 0x10A860C1UL) % 0xFFFFFFFBUL);
                // ReSharper restore RedundantCast
                temp[i] ^= (byte)(xor & 0xFF);
            }

            var rightHalf = (int)((seed % 32) % length);
            var leftHalf = length - rightHalf;

            Array.Copy(temp, leftHalf, buffer, offset, rightHalf);
            Array.Copy(temp, 0, buffer, offset + rightHalf, leftHalf);
        }
    }
}
