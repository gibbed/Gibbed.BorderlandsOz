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
    public class DLCExpansionData : INotifyPropertyChanged
    {
        #region Fields
        private int _Tag;
        private byte[] _Data;
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public int Tag
        {
            get { return this._Tag; }
            set
            {
                if (value != this._Tag)
                {
                    this._Tag = value;
                    this.NotifyPropertyChanged("Tag");
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public byte[] Data
        {
            get { return this._Data; }
            set
            {
                if (value != this._Data)
                {
                    this._Data = value;
                    this.NotifyPropertyChanged("Data");
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
