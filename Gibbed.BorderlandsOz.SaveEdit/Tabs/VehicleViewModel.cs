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
    [Export(typeof(VehicleViewModel))]
    internal class VehicleViewModel : PropertyChangedBase
    {
        #region Fields
        private string _SelectedMoonBuggy1 = "None";
        private string _SelectedMoonBuggy2 = "None";
        private readonly List<string> _ExtraMoonBuggy = new List<string>();

        private string _SelectedStingRay1 = "None";
        private string _SelectedStingRay2 = "None";
        private readonly List<string> _ExtraStingRay = new List<string>();
        #endregion

        #region Properties
        public string SelectedMoonBuggy1
        {
            get { return this._SelectedMoonBuggy1; }
            set
            {
                this._SelectedMoonBuggy1 = value;
                this.NotifyOfPropertyChange(() => this.SelectedMoonBuggy1);
            }
        }

        public string SelectedMoonBuggy2
        {
            get { return this._SelectedMoonBuggy2; }
            set
            {
                this._SelectedMoonBuggy2 = value;
                this.NotifyOfPropertyChange(() => this.SelectedMoonBuggy2);
            }
        }

        public List<string> ExtraMoonBuggy
        {
            get { return this._ExtraMoonBuggy; }
        }

        public string SelectedStingRay1
        {
            get { return this._SelectedStingRay1; }
            set
            {
                this._SelectedStingRay1 = value;
                this.NotifyOfPropertyChange(() => this.SelectedStingRay1);
            }
        }

        public string SelectedStingRay2
        {
            get { return this._SelectedStingRay2; }
            set
            {
                this._SelectedStingRay2 = value;
                this.NotifyOfPropertyChange(() => this.SelectedStingRay2);
            }
        }

        public List<string> ExtraStingRay
        {
            get { return this._ExtraStingRay; }
        }

        public ObservableCollection<AssetDisplay> MoonBuggyAssets { get; private set; }
        public ObservableCollection<AssetDisplay> StingRayAssets { get; private set; }
        #endregion

        [ImportingConstructor]
        public VehicleViewModel()
        {
            this.MoonBuggyAssets = new ObservableCollection<AssetDisplay>();
            this.StingRayAssets = new ObservableCollection<AssetDisplay>();

            BuildCustomizationAssets(CustomizationUsage.MoonBuggy, this.MoonBuggyAssets);
            BuildCustomizationAssets(CustomizationUsage.StingRay, this.StingRayAssets);
        }

        private static void BuildCustomizationAssets(CustomizationUsage usage, ObservableCollection<AssetDisplay> target)
        {
            target.Clear();
            target.Add(new AssetDisplay("None", "None", "Base Game"));
            var assets = new List<KeyValuePair<AssetDisplay, int>>();
            foreach (var kv in
                InfoManager.Customizations.Items.Where(
                    kv => kv.Value.Type == CustomizationType.Skin && kv.Value.Usage.Contains(usage) == true).OrderBy
                    (cd => cd.Value.Name))
            {
                string group;
                int priority;

                if (kv.Value.DLC != null)
                {
                    if (kv.Value.DLC.Package != null)
                    {
                        group = kv.Value.DLC.Package.DisplayName;
                        priority = kv.Value.DLC.Package.Id;
                    }
                    else
                    {
                        group = "??? " + kv.Value.DLC.ResourcePath + " ???";
                        priority = int.MaxValue;
                    }
                }
                else
                {
                    group = "Base Game";
                    priority = int.MinValue;
                }

                assets.Add(new KeyValuePair<AssetDisplay, int>(new AssetDisplay(kv.Value.Name, kv.Key, group),
                                                               priority));
            }
            assets.OrderBy(kv => kv.Value).Apply(kv => target.Add(kv.Key));
        }

        private static void ImportTarget(string name,
                                         IEnumerable<ChosenVehicleCustomization> customizations,
                                         Action<string> skin1,
                                         Action<string> skin2,
                                         Action<string> extra)
        {
            skin1("None");
            skin2("None");

            var customization = customizations.FirstOrDefault(c => c.Family == name);
            if (customization != null &&
                customization.Customizations != null)
            {
                if (customization.Customizations.Count > 0)
                {
                    skin1(customization.Customizations[0]);
                }

                if (customization.Customizations.Count > 1)
                {
                    skin2(customization.Customizations[1]);
                }

                if (customization.Customizations.Count > 2)
                {
                    customization.Customizations.Skip(2).Apply(extra);
                }
            }
        }

        public void ImportData(WillowTwoPlayerSaveGame saveGame)
        {
            this.ExtraMoonBuggy.Clear();
            ImportTarget("GD_Globals.VehicleSpawnStation.VehicleFamily_MoonBuggy",
                         saveGame.ChosenVehicleCustomizations,
                         s => this.SelectedMoonBuggy1 = s,
                         s => this.SelectedMoonBuggy2 = s,
                         s => this.ExtraMoonBuggy.Add(s));

            this.ExtraStingRay.Clear();
            ImportTarget("GD_Globals.VehicleSpawnStation.VehicleFamily_StingRay",
                         saveGame.ChosenVehicleCustomizations,
                         s => this.SelectedStingRay1 = s,
                         s => this.SelectedStingRay2 = s,
                         s => this.ExtraStingRay.Add(s));
        }

        private static void ExportTarget(string name,
                                         List<ChosenVehicleCustomization> customizations,
                                         string skin1,
                                         string skin2,
                                         IEnumerable<string> extras)
        {
            customizations.RemoveAll(c => c.Family == name);

            var skins = extras.ToArray();
            if (skin1 != "None" ||
                skin2 != "None" ||
                skins.Length > 0)
            {
                var customization = new ChosenVehicleCustomization()
                {
                    Family = name,
                };
                customization.Customizations.Add(skin1);
                customization.Customizations.Add(skin2);
                customization.Customizations.AddRange(skins);
                customizations.Add(customization);
            }
        }

        public void ExportData(WillowTwoPlayerSaveGame saveGame)
        {
            ExportTarget("GD_Globals.VehicleSpawnStation.VehicleFamily_MoonBuggy",
                         saveGame.ChosenVehicleCustomizations,
                         this.SelectedMoonBuggy1,
                         this.SelectedMoonBuggy2,
                         this.ExtraMoonBuggy);
            ExportTarget("GD_Globals.VehicleSpawnStation.VehicleFamily_StingRay",
                         saveGame.ChosenVehicleCustomizations,
                         this.SelectedStingRay1,
                         this.SelectedStingRay2,
                         this.ExtraStingRay);
        }
    }
}
