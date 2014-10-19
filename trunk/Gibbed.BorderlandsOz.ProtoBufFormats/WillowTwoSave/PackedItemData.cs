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

using System.ComponentModel;
using ProtoBuf;

namespace Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave
{
    [ProtoContract]
    public class PackedItemData : INotifyPropertyChanged
    {
        #region Fields
        private byte[] _InventorySerialNumber;
        private int _Quantity;
        private bool _Equipped;
        /* Should be PlayerMark type, but due to their DLC expansion hack I
         * can't leave it this way due to protobuf.net's handling of it.
         */
        private int _Mark;
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public byte[] InventorySerialNumber
        {
            get { return this._InventorySerialNumber; }
            set
            {
                if (value != this._InventorySerialNumber)
                {
                    this._InventorySerialNumber = value;
                    this.NotifyPropertyChanged("InventorySerialNumber");
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public int Quantity
        {
            get { return this._Quantity; }
            set
            {
                if (value != this._Quantity)
                {
                    this._Quantity = value;
                    this.NotifyPropertyChanged("Quantity");
                }
            }
        }

        [ProtoMember(3, IsRequired = true)]
        public bool Equipped
        {
            get { return this._Equipped; }
            set
            {
                if (value != this._Equipped)
                {
                    this._Equipped = value;
                    this.NotifyPropertyChanged("Equipped");
                }
            }
        }

        [ProtoMember(4, IsRequired = true)]
        public int Mark
        {
            get { return this._Mark; }
            set
            {
                if (value != this._Mark)
                {
                    this._Mark = value;
                    this.NotifyPropertyChanged("Mark");
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
