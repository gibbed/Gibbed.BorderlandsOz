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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gibbed.BorderlandsOz.GameInfo;
using Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave;

namespace Gibbed.BorderlandsOz.SaveEdit
{
    [Export(typeof(FastTravelViewModel))]
    internal class FastTravelViewModel : PropertyChangedBase
    {
        #region Fields
        private readonly ObservableCollection<FastTravelPlaythroughViewModel> _Playthroughs;
        #endregion

        #region Properties
        public ObservableCollection<AssetDisplay> AvailableTeleporters { get; private set; }
        public ObservableCollection<AssetDisplay> VisitedTeleporters { get; private set; }

        public ObservableCollection<FastTravelPlaythroughViewModel> Playthroughs
        {
            get { return this._Playthroughs; }
        }
        #endregion

        [ImportingConstructor]
        public FastTravelViewModel()
        {
            this.AvailableTeleporters = new ObservableCollection<AssetDisplay>()
            {
                new AssetDisplay("None", "None", "Base Game")
            };

            this.VisitedTeleporters = new ObservableCollection<AssetDisplay>();

            var fastTravelStations = InfoManager.TravelStations
                .Items
                .Where(kv => kv.Value is FastTravelStationDefinition)
                .Select(kv => kv.Value)
                .Cast<FastTravelStationDefinition>()
                .ToList();

            var levelTravelStations = InfoManager.TravelStations
                .Items
                .Where(kv => kv.Value is LevelTravelStationDefinition)
                .Select(kv => kv.Value)
                .Cast<LevelTravelStationDefinition>()
                .ToList();

            var displayNames = new Dictionary<string, int>();
            foreach (var station in fastTravelStations)
            {
                string displayName = string.IsNullOrEmpty(station.Sign) == false
                                         ? station.Sign
                                         : station.StationDisplayName;
                displayNames.TryGetValue(displayName, out int count);
                count++;
                displayNames[displayName] = count;
            }

            foreach (var kv in InfoManager.FastTravelStationOrdering
                .Items
                .OrderBy(
                    fsto =>
                    fsto.Value.DLCExpansion == null
                        ? 0
                        : (fsto.Value.DLCExpansion.Package != null
                               ? fsto.Value.DLCExpansion.Package.Id
                               : int.MaxValue)))
            {
                string group = kv.Value.DLCExpansion == null ? "Base Game" : kv.Value.DLCExpansion.Package.DisplayName;
                foreach (var station in kv.Value.Stations)
                {
                    if (fastTravelStations.Remove(station) == false)
                    {
                        continue;
                    }

                    string displayName = string.IsNullOrEmpty(station.Sign) == false
                                             ? station.Sign
                                             : station.StationDisplayName;
                    displayNames.TryGetValue(displayName, out var displayCount);
                    if (displayCount > 1 && string.IsNullOrEmpty(station.ResourceName) == false)
                    {
                        displayName += $" ({station.ResourceName})";
                    }

                    this.AvailableTeleporters.Add(new AssetDisplay(displayName, station.ResourceName, group));
                    this.VisitedTeleporters.Add(new AssetDisplay(displayName, station.ResourceName, group));
                }
            }

            foreach (var station in levelTravelStations
                .OrderBy(
                    lts =>
                    lts.DLCExpansion == null
                        ? 0
                        : (lts.DLCExpansion.Package != null
                               ? lts.DLCExpansion.Package.Id
                               : int.MaxValue))
                .ThenBy(lts => lts.ResourceName))
            {
                string group = station.DLCExpansion == null
                                   ? "Base Game"
                                   : station.DLCExpansion.Package.DisplayName;
                var displayName = string.IsNullOrEmpty(station.DisplayName) == false
                                      ? station.DisplayName
                                      : station.ResourceName;
                displayNames.TryGetValue(displayName, out var displayCount);
                if (displayCount > 1 && string.IsNullOrEmpty(station.ResourceName) == false)
                {
                    displayName += $" ({station.ResourceName})";
                }

                this.AvailableTeleporters.Add(new AssetDisplay(
                    displayName,
                    station.ResourceName,
                    $"Level Transitions ({group})"));
            }

            this._Playthroughs = new ObservableCollection<FastTravelPlaythroughViewModel>();
        }

        public void ImportData(WillowTwoPlayerSaveGame saveGame)
        {
            this.Playthroughs.Clear();
            int index = 0;
            foreach (var missionPlaythrough in saveGame.MissionPlaythroughs)
            {
                var viewModel = new FastTravelPlaythroughViewModel($"Playthrough #{index + 1}", this);
                viewModel.ImportData(missionPlaythrough);
                this.Playthroughs.Add(viewModel);
                index++;
            }
        }

        public void ExportData(WillowTwoPlayerSaveGame saveGame)
        {
            var count = Math.Min(this.Playthroughs.Count, saveGame.MissionPlaythroughs.Count);
            for (int i = 0; i < count; i++)
            {
                var viewModel = this.Playthroughs[i];
                var missionPlaythrough = saveGame.MissionPlaythroughs[i];
                viewModel.ExportData(missionPlaythrough);
            }
        }
    }
}
