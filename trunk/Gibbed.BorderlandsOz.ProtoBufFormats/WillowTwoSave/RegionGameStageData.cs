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
    public class RegionGameStageData : INotifyPropertyChanged
    {
        #region Fields
        private string _Region;
        private int _GameStage;
        private bool _IsFromDLC;
        private int _DLCPackageId;
        private int _PlaythroughIndex;
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public string Region
        {
            get { return this._Region; }
            set
            {
                if (value != this._Region)
                {
                    this._Region = value;
                    this.NotifyPropertyChanged("Region");
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public int GameStage
        {
            get { return this._GameStage; }
            set
            {
                if (value != this._GameStage)
                {
                    this._GameStage = value;
                    this.NotifyPropertyChanged("GameStage");
                }
            }
        }

        [ProtoMember(3, IsRequired = true)]
        public bool IsFromDLC
        {
            get { return this._IsFromDLC; }
            set
            {
                if (value != this._IsFromDLC)
                {
                    this._IsFromDLC = value;
                    this.NotifyPropertyChanged("IsFromDLC");
                }
            }
        }

        [ProtoMember(4, IsRequired = true)]
        public int DLCPackageId
        {
            get { return this._DLCPackageId; }
            set
            {
                if (value != this._DLCPackageId)
                {
                    this._DLCPackageId = value;
                    this.NotifyPropertyChanged("DLCPackageId");
                }
            }
        }

        [ProtoMember(5, IsRequired = true)]
        public int PlaythroughIndex
        {
            get { return this._PlaythroughIndex; }
            set
            {
                if (value != this._PlaythroughIndex)
                {
                    this._PlaythroughIndex = value;
                    this.NotifyPropertyChanged("PlaythroughIndex");
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
