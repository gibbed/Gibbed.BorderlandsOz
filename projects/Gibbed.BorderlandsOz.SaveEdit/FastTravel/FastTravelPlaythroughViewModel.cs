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

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave;

namespace Gibbed.BorderlandsOz.SaveEdit
{
    [Export(typeof(FastTravelPlaythroughViewModel))]
    internal class FastTravelPlaythroughViewModel : PropertyChangedBase
    {
        #region Fields
        private readonly string _DisplayName;
        private string _LastVisitedTeleporter;
        #endregion

        #region Imports
        private FastTravelViewModel _FastTravel;

        public FastTravelViewModel FastTravel
        {
            get { return this._FastTravel; }
            set
            {
                this._FastTravel = value;
                this.NotifyOfPropertyChange(nameof(FastTravel));
            }
        }
        #endregion

        #region Properties
        public string DisplayName
        {
            get { return this._DisplayName; }
        }

        public string LastVisitedTeleporter
        {
            get { return this._LastVisitedTeleporter; }
            set
            {
                this._LastVisitedTeleporter = value;
                this.NotifyOfPropertyChange(nameof(LastVisitedTeleporter));
            }
        }

        public ObservableCollection<AssetDisplay> AvailableTeleporters { get; private set; }
        public ObservableCollection<VisitedTeleporterDisplay> VisitedTeleporters { get; private set; }
        #endregion

        public FastTravelPlaythroughViewModel(string displayName, FastTravelViewModel fastTravel)
        {
            this._DisplayName = displayName;
            this._FastTravel = fastTravel;

            this.AvailableTeleporters = new ObservableCollection<AssetDisplay>();
            this.VisitedTeleporters = new ObservableCollection<VisitedTeleporterDisplay>();

            foreach (var teleporter in this.FastTravel.AvailableTeleporters)
            {
                this.AvailableTeleporters.Add(teleporter);
            }

            foreach (var station in this.FastTravel.VisitedTeleporters)
            {
                this.VisitedTeleporters.Add(new VisitedTeleporterDisplay()
                {
                    DisplayName = station.Name,
                    ResourceName = station.Path,
                    Visited = false,
                    Custom = false,
                    Group = station.Group,
                });
            }
        }

        public void ImportData(MissionPlaythroughData data)
        {
            foreach (var teleporter in this.AvailableTeleporters.Where(t => t.Custom == true).ToList())
            {
                this.AvailableTeleporters.Remove(teleporter);
            }

            if (this.AvailableTeleporters.Any(t => t.Path == data.LastVisitedTeleporterName) == false)
            {
                this.AvailableTeleporters.Add(new AssetDisplay(
                    data.LastVisitedTeleporterName,
                    data.LastVisitedTeleporterName,
                    "Unknown",
                    true));
            }

            this.LastVisitedTeleporter = data.LastVisitedTeleporterName;

            var visitedStations = data.ActivatedTeleportersList.ToList();
            foreach (var station in this.VisitedTeleporters.ToArray())
            {
                if (visitedStations.Contains(station.ResourceName) == true)
                {
                    station.Visited = true;
                    visitedStations.Remove(station.ResourceName);
                }
                else if (station.Custom == true)
                {
                    this.VisitedTeleporters.Remove(station);
                }
                else
                {
                    station.Visited = false;
                }
            }

            foreach (var station in visitedStations)
            {
                this.VisitedTeleporters.Add(new VisitedTeleporterDisplay()
                {
                    DisplayName = station,
                    ResourceName = station,
                    Visited = true,
                    Custom = true,
                    Group = "Unknown",
                });
            }
        }

        public void ExportData(MissionPlaythroughData data)
        {
            data.LastVisitedTeleporterName = this.LastVisitedTeleporter;
            data.ActivatedTeleportersList.Clear();
            foreach (var station in this.VisitedTeleporters)
            {
                if (station.Visited == true)
                {
                    data.ActivatedTeleportersList.Add(station.ResourceName);
                }
            }
        }

        public void VisitedCheckAll()
        {
            foreach (var visitedTeleporter in this.VisitedTeleporters)
            {
                visitedTeleporter.Visited = true;
            }
        }

        public void VisitedUncheckAll()
        {
            foreach (var visitedTeleporter in this.VisitedTeleporters)
            {
                visitedTeleporter.Visited = false;
            }
        }

        #region VisitedTeleporterDisplay
        public class VisitedTeleporterDisplay : PropertyChangedBase
        {
            #region Fields
            private string _DisplayName;
            private string _ResourceName;
            private bool _Visited;
            private bool _Custom;
            private string _Group;
            #endregion

            #region Properties
            public string DisplayName
            {
                get { return this._DisplayName; }
                set
                {
                    this._DisplayName = value;
                    this.NotifyOfPropertyChange(nameof(DisplayName));
                }
            }

            public string ResourceName
            {
                get { return this._ResourceName; }
                set
                {
                    this._ResourceName = value;
                    this.NotifyOfPropertyChange(nameof(ResourceName));
                }
            }

            public bool Visited
            {
                get { return this._Visited; }
                set
                {
                    this._Visited = value;
                    this.NotifyOfPropertyChange(nameof(Visited));
                }
            }

            public bool Custom
            {
                get { return this._Custom; }
                set
                {
                    this._Custom = value;
                    this.NotifyOfPropertyChange(nameof(Custom));
                }
            }

            public string Group
            {
                get { return this._Group; }
                set
                {
                    this._Group = value;
                    this.NotifyOfPropertyChange(nameof(Group));
                }
            }
            #endregion
        }
        #endregion
    }
}
