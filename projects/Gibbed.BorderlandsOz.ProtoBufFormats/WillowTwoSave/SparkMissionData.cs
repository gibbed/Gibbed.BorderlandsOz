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

using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave
{
    [ProtoContract]
    public class SparkMissionData : INotifyPropertyChanged
    {
        #region Fields
        private string _Mission;
        private int _SparkMissionNumber;
        private List<int> _ObjectivesProgress = new List<int>();
        private int _ActiveObjectiveSetIndex;
        private MissionStatus _Status;
        private bool _NeedsRewards;
        #endregion

        #region Serialization
        [ProtoAfterDeserialization]
        private void OnDeserialization()
        {
            this._ObjectivesProgress = this._ObjectivesProgress ?? new List<int>();
        }

        private bool ShouldSerializeObjectivesProgress()
        {
            return this._ObjectivesProgress != null &&
                   this._ObjectivesProgress.Count > 0;
        }
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = true)]
        public string Mission
        {
            get { return this._Mission; }
            set
            {
                if (value != this._Mission)
                {
                    this._Mission = value;
                    this.NotifyOfPropertyChange(nameof(Mission));
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public int SparkMissionNumber
        {
            get { return this._SparkMissionNumber; }
            set
            {
                if (value != this._SparkMissionNumber)
                {
                    this._SparkMissionNumber = value;
                    this.NotifyOfPropertyChange(nameof(SparkMissionNumber));
                }
            }
        }

        [ProtoMember(3, IsRequired = true, IsPacked = true)]
        public List<int> ObjectivesProgress
        {
            get { return this._ObjectivesProgress; }
            set
            {
                if (value != this._ObjectivesProgress)
                {
                    this._ObjectivesProgress = value;
                    this.NotifyOfPropertyChange(nameof(ObjectivesProgress));
                }
            }
        }

        [ProtoMember(4, IsRequired = true)]
        public int ActiveObjectiveSetIndex
        {
            get { return this._ActiveObjectiveSetIndex; }
            set
            {
                if (value != this._ActiveObjectiveSetIndex)
                {
                    this._ActiveObjectiveSetIndex = value;
                    this.NotifyOfPropertyChange(nameof(ActiveObjectiveSetIndex));
                }
            }
        }

        [ProtoMember(5, IsRequired = true)]
        public MissionStatus Status
        {
            get { return this._Status; }
            set
            {
                if (value != this._Status)
                {
                    this._Status = value;
                    this.NotifyOfPropertyChange(nameof(Status));
                }
            }
        }

        [ProtoMember(6, IsRequired = true)]
        public bool NeedsRewards
        {
            get { return this._NeedsRewards; }
            set
            {
                if (value != this._NeedsRewards)
                {
                    this._NeedsRewards = value;
                    this.NotifyOfPropertyChange(nameof(NeedsRewards));
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

        public override string ToString()
        {
            return this.Mission ?? base.ToString();
        }
    }
}
