/* Copyright (c) 2015 Rick (rick 'at' gibbed 'dot' us)
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

using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave
{
    [ProtoContract]
    public class Graveyard : INotifyPropertyChanged
    {
        #region Fields
        private List<Tombstone> _TombstoneList;
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public List<Tombstone> TombstoneList
        {
            get { return this._TombstoneList; }
            set
            {
                if (value != this._TombstoneList)
                {
                    this._TombstoneList = value;
                    this.NotifyPropertyChanged("TombstoneList");
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
