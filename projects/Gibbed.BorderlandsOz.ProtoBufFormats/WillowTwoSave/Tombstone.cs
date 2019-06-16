/* Copyright (c) 2019 Rick (rick 'at' gibbed 'dot' us)
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
    public class Tombstone : INotifyPropertyChanged
    {
        #region Fields
        private int _TotalTimePlayed;
        private int _CharacterLevel;
        private int _PercentMissionsComplete;
        private int _PercentChallengesComplete;
        private string _CharacterName;
        private string _FavoriteManufacturer;
        private string _FavoriteWeaponType;
        private string _KilledByDescription;
        private string _ScreenshotFilename;
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public int TotalTimePlayed
        {
            get { return this._TotalTimePlayed; }
            set
            {
                if (value != this._TotalTimePlayed)
                {
                    this._TotalTimePlayed = value;
                    this.NotifyOfPropertyChange(nameof(TotalTimePlayed));
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public int CharacterLevel
        {
            get { return this._CharacterLevel; }
            set
            {
                if (value != this._CharacterLevel)
                {
                    this._CharacterLevel = value;
                    this.NotifyOfPropertyChange(nameof(CharacterLevel));
                }
            }
        }

        [ProtoMember(3, IsRequired = true)]
        public int PercentMissionsComplete
        {
            get { return this._PercentMissionsComplete; }
            set
            {
                if (value != this._PercentMissionsComplete)
                {
                    this._PercentMissionsComplete = value;
                    this.NotifyOfPropertyChange(nameof(PercentMissionsComplete));
                }
            }
        }

        [ProtoMember(4, IsRequired = true)]
        public int PercentChallengesComplete
        {
            get { return this._PercentChallengesComplete; }
            set
            {
                if (value != this._PercentChallengesComplete)
                {
                    this._PercentChallengesComplete = value;
                    this.NotifyOfPropertyChange(nameof(PercentChallengesComplete));
                }
            }
        }

        [ProtoMember(5, IsRequired = true)]
        public string CharacterName
        {
            get { return this._CharacterName; }
            set
            {
                if (value != this._CharacterName)
                {
                    this._CharacterName = value;
                    this.NotifyOfPropertyChange(nameof(CharacterName));
                }
            }
        }

        [ProtoMember(6, IsRequired = true)]
        public string FavoriteManufacturer
        {
            get { return this._FavoriteManufacturer; }
            set
            {
                if (value != this._FavoriteManufacturer)
                {
                    this._FavoriteManufacturer = value;
                    this.NotifyOfPropertyChange(nameof(FavoriteManufacturer));
                }
            }
        }

        [ProtoMember(7, IsRequired = true)]
        public string FavoriteWeaponType
        {
            get { return this._FavoriteWeaponType; }
            set
            {
                if (value != this._FavoriteWeaponType)
                {
                    this._FavoriteWeaponType = value;
                    this.NotifyOfPropertyChange(nameof(FavoriteWeaponType));
                }
            }
        }

        [ProtoMember(8, IsRequired = true)]
        public string KilledByDescription
        {
            get { return this._KilledByDescription; }
            set
            {
                if (value != this._KilledByDescription)
                {
                    this._KilledByDescription = value;
                    this.NotifyOfPropertyChange(nameof(KilledByDescription));
                }
            }
        }

        [ProtoMember(9, IsRequired = true)]
        public string ScreenshotFilename
        {
            get { return this._ScreenshotFilename; }
            set
            {
                if (value != this._ScreenshotFilename)
                {
                    this._ScreenshotFilename = value;
                    this.NotifyOfPropertyChange(nameof(ScreenshotFilename));
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyOfPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
