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
    public class ObjectiveData : INotifyPropertyChanged
    {
        #region Fields
        private string _SwitchName;
        private int _SwitchValue;
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public string SwitchName
        {
            get { return this._SwitchName; }
            set
            {
                if (value != this._SwitchName)
                {
                    this._SwitchName = value;
                    this.NotifyPropertyChanged("SwitchName");
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public int SwitchValue
        {
            get { return this._SwitchValue; }
            set
            {
                if (value != this._SwitchValue)
                {
                    this._SwitchValue = value;
                    this.NotifyPropertyChanged("SwitchValue");
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
