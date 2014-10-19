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

using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave
{
    [ProtoContract]
    public class OneOffLevelChallengeData : INotifyPropertyChanged
    {
        #region Fields
        private int _PackageId;
        private int _ContentId;
        private List<uint> _Completion = new List<uint>();
        #endregion

        #region Serialization
        [ProtoAfterDeserialization]
        // ReSharper disable UnusedMember.Local
        private void OnDeserialization()
            // ReSharper restore UnusedMember.Local
        {
            this._Completion = this._Completion ?? new List<uint>();
        }

        private bool ShouldSerializeCompletion()
        {
            return this._Completion != null &&
                   this._Completion.Count > 0;
        }
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public int PackageId
        {
            get { return this._PackageId; }
            set
            {
                if (value != this._PackageId)
                {
                    this._PackageId = value;
                    this.NotifyPropertyChanged("PackageId");
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public int ContentId
        {
            get { return this._ContentId; }
            set
            {
                if (value != this._ContentId)
                {
                    this._ContentId = value;
                    this.NotifyPropertyChanged("ContentId");
                }
            }
        }

        [ProtoMember(3, IsRequired = true)]
        public List<uint> Completion
        {
            get { return this._Completion; }
            set
            {
                if (value != this._Completion)
                {
                    this._Completion = value;
                    this.NotifyPropertyChanged("Completion");
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
