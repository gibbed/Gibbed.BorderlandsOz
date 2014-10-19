/* Copyright (c) 2014 Rick (rick 'at' gibbed 'dot' us)
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
    public class MissionPlaythroughData : INotifyPropertyChanged
    {
        #region Fields
        private int? _PlayThroughNumber;
        private string _ActiveMission;
        private List<MissionData> _MissionList = new List<MissionData>();
        private List<PendingMissionRewards> _PendingMissionRewards = new List<PendingMissionRewards>();
        private List<string> _FilteredMissions = new List<string>();
        private List<SparkMissionData> _SparkMissionList = new List<SparkMissionData>();
        private List<string> _ActivatedTeleportersList = new List<string>();
        private string _LastVisitedTeleporterName;
        #endregion

        #region Serialization
        [ProtoAfterDeserialization]
        // ReSharper disable UnusedMember.Local
        private void OnDeserialization()
            // ReSharper restore UnusedMember.Local
        {
            this._MissionList = this._MissionList ?? new List<MissionData>();
            this._PendingMissionRewards = this._PendingMissionRewards ?? new List<PendingMissionRewards>();
            this._FilteredMissions = this._FilteredMissions ?? new List<string>();
            this._SparkMissionList = this._SparkMissionList ?? new List<SparkMissionData>();
            this._ActivatedTeleportersList = this._ActivatedTeleportersList ?? new List<string>();
        }

        private bool ShouldSerializeMissionList()
        {
            return this._MissionList != null &&
                   this._MissionList.Count > 0;
        }

        private bool ShouldSerializePendingMissionRewards()
        {
            return this._PendingMissionRewards != null &&
                   this._PendingMissionRewards.Count > 0;
        }

        private bool ShouldSerializeFilteredMissions()
        {
            return this._FilteredMissions != null &&
                   this._FilteredMissions.Count > 0;
        }

        private bool ShouldSerializeSparkMissionList()
        {
            return this._SparkMissionList != null &&
                   this._SparkMissionList.Count > 0;
        }

        private bool ShouldSerializeActivatedTeleportersList()
        {
            return this._ActivatedTeleportersList != null &&
                   this._ActivatedTeleportersList.Count > 0;
        }
        #endregion

        #region Properties
        [ProtoMember(1, IsRequired = false)]
        public int? PlayThroughNumber
        {
            get { return this._PlayThroughNumber; }
            set
            {
                if (value != this._PlayThroughNumber)
                {
                    this._PlayThroughNumber = value;
                    this.NotifyPropertyChanged("PlayThroughNumber");
                }
            }
        }

        [ProtoMember(2, IsRequired = true)]
        public string ActiveMission
        {
            get { return this._ActiveMission; }
            set
            {
                if (value != this._ActiveMission)
                {
                    this._ActiveMission = value;
                    this.NotifyPropertyChanged("ActiveMission");
                }
            }
        }

        [ProtoMember(3, IsRequired = true)]
        public List<MissionData> MissionList
        {
            get { return this._MissionList; }
            set
            {
                if (value != this._MissionList)
                {
                    this._MissionList = value;
                    this.NotifyPropertyChanged("MissionList");
                }
            }
        }

        [ProtoMember(4, IsRequired = true)]
        public List<PendingMissionRewards> PendingMissionRewards
        {
            get { return this._PendingMissionRewards; }
            set
            {
                if (value != this._PendingMissionRewards)
                {
                    this._PendingMissionRewards = value;
                    this.NotifyPropertyChanged("PendingMissionRewards");
                }
            }
        }

        [ProtoMember(5, IsRequired = true)]
        public List<string> FilteredMissions
        {
            get { return this._FilteredMissions; }
            set
            {
                if (value != this._FilteredMissions)
                {
                    this._FilteredMissions = value;
                    this.NotifyPropertyChanged("FilteredMissions");
                }
            }
        }

        [ProtoMember(6, IsRequired = true)]
        public List<SparkMissionData> SparkMissionList
        {
            get { return this._SparkMissionList; }
            set
            {
                if (value != this._SparkMissionList)
                {
                    this._SparkMissionList = value;
                    this.NotifyPropertyChanged("SparkMissionList");
                }
            }
        }

        [ProtoMember(7, IsRequired = true)]
        public List<string> ActivatedTeleportersList
        {
            get { return this._ActivatedTeleportersList; }
            set
            {
                if (value != this._ActivatedTeleportersList)
                {
                    this._ActivatedTeleportersList = value;
                    this.NotifyPropertyChanged("ActivatedTeleportersList");
                }
            }
        }

        [ProtoMember(8, IsRequired = true)]
        public string LastVisitedTeleporterName
        {
            get { return this._LastVisitedTeleporterName; }
            set
            {
                if (value != this._LastVisitedTeleporterName)
                {
                    this._LastVisitedTeleporterName = value;
                    this.NotifyPropertyChanged("LastVisitedTeleporterName");
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
