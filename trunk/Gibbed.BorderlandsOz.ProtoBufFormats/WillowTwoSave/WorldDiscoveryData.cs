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
    public class WorldDiscoveryData : INotifyPropertyChanged
    {
        #region Fields
        private string _DiscoveryName;
        private bool _HasBeenUncovered;
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public string DiscoveryName
        {
            get { return this._DiscoveryName; }
            set
            {
                if (value != this._DiscoveryName)
                {
                    this._DiscoveryName = value;
                    this.NotifyPropertyChanged("DiscoveryName");
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public bool HasBeenUncovered
        {
            get { return this._HasBeenUncovered; }
            set
            {
                if (value != this._HasBeenUncovered)
                {
                    this._HasBeenUncovered = value;
                    this.NotifyPropertyChanged("HasBeenUncovered");
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
