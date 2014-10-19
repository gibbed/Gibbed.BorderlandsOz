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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Caliburn.Micro.Contrib.Results;
using Gibbed.BorderlandsOz.FileFormats.Items;
using Gibbed.BorderlandsOz.GameInfo;
using Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave;

namespace Gibbed.BorderlandsOz.SaveEdit
{
    [Export(typeof(BankViewModel))]
    internal class BankViewModel : PropertyChangedBase
    {
        #region Imports
        private CharacterViewModel _Character;
        private BackpackViewModel _Backpack;

        [Import(typeof(CharacterViewModel))]
        public CharacterViewModel Character
        {
            get { return this._Character; }
            set
            {
                this._Character = value;
                this.NotifyOfPropertyChange(() => this.Character);
            }
        }

        [Import(typeof(BackpackViewModel))]
        public BackpackViewModel Backpack
        {
            get { return this._Backpack; }
            set
            {
                this._Backpack = value;
                this.NotifyOfPropertyChange(() => this.Backpack);
            }
        }
        #endregion

        #region Fields
        private readonly ObservableCollection<IBaseSlotViewModel> _Slots;

        private readonly List<KeyValuePair<BankSlot, Exception>> _BrokenSlots;

        private IBaseSlotViewModel _SelectedSlot;

        private readonly ICommand _NewWeapon;
        private readonly ICommand _NewItem;
        #endregion

        #region Properties
        public IEnumerable<DownloadablePackageDefinition> DownloadablePackages
        {
            get
            {
                return
                    InfoManager.DownloadableContents.Items
                               .Where(dc => dc.Value.Type == DownloadableContentType.ItemSet &&
                                            dc.Value.Package != null)
                               .Select(dc => dc.Value.Package)
                               .Where(dp => InfoManager.AssetLibraryManager.Sets.Any(s => s.Id == dp.Id) == true)
                               .Distinct()
                               .OrderBy(dp => dp.Id);
            }
        }

        public bool HasDownloadablePackages
        {
            get { return this.DownloadablePackages.Any(); }
        }

        public ObservableCollection<IBaseSlotViewModel> Slots
        {
            get { return this._Slots; }
        }

        public List<KeyValuePair<BankSlot, Exception>> BrokenSlots
        {
            get { return this._BrokenSlots; }
        }

        public IBaseSlotViewModel SelectedSlot
        {
            get { return this._SelectedSlot; }
            set
            {
                this._SelectedSlot = value;
                this.NotifyOfPropertyChange(() => this.SelectedSlot);
            }
        }

        public ICommand NewWeapon
        {
            get { return this._NewWeapon; }
        }

        public ICommand NewItem
        {
            get { return this._NewItem; }
        }
        #endregion

        [ImportingConstructor]
        public BankViewModel()
        {
            this._Slots = new ObservableCollection<IBaseSlotViewModel>();
            this._BrokenSlots = new List<KeyValuePair<BankSlot, Exception>>();
            this._NewWeapon = new DelegateCommand<int>(this.DoNewWeapon);
            this._NewItem = new DelegateCommand<int>(this.DoNewItem);
        }

        public void DoNewWeapon(int assetLibrarySetId)
        {
            var weapon = new BaseWeapon()
            {
                UniqueId = new Random().Next(int.MinValue, int.MaxValue),
                // TODO: check other item unique IDs to prevent rare collisions
                AssetLibrarySetId = assetLibrarySetId,
            };
            var viewModel = new BaseWeaponViewModel(weapon);
            this.Slots.Add(viewModel);
            this.SelectedSlot = viewModel;
        }

        public void DoNewItem(int assetLibrarySetId)
        {
            var item = new BaseItem()
            {
                UniqueId = new Random().Next(int.MinValue, int.MaxValue),
                // TODO: check other item unique IDs to prevent rare collisions
                AssetLibrarySetId = assetLibrarySetId,
            };
            var viewModel = new BaseItemViewModel(item);
            this.Slots.Add(viewModel);
            this.SelectedSlot = viewModel;
        }

        private static readonly Regex _CodeSignature =
            new Regex(@"BL(OZ|TPS)\((?<data>(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?)\)",
                      RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public IEnumerable<IResult> PasteCode()
        {
            bool containsText;
            bool containsUnicodeText = false;
            if (MyClipboard.ContainsText(TextDataFormat.Text, out containsText) != MyClipboard.Result.Success ||
                MyClipboard.ContainsText(TextDataFormat.UnicodeText, out containsUnicodeText) !=
                MyClipboard.Result.Success)
            {
                yield return new MyMessageBox("Clipboard failure.", "Error")
                    .WithIcon(MessageBoxImage.Error);
            }

            if (containsText == false &&
                containsUnicodeText == false)
            {
                yield break;
            }

            var errors = 0;
            var viewModels = new List<IBaseSlotViewModel>();
            yield return new DelegateResult(
                () =>
                {
                    string codes;
                    if (MyClipboard.GetText(out codes) != MyClipboard.Result.Success)
                    {
                        MessageBox.Show("Clipboard failure.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        return;
                    }

                    // strip whitespace
                    codes = Regex.Replace(codes, @"\s+", "");

                    foreach (var match in _CodeSignature.Matches(codes).Cast<Match>()
                                                        .Where(m => m.Success == true))
                    {
                        var code = match.Groups["data"].Value;

                        IPackable packable;

                        try
                        {
                            var data = Convert.FromBase64String(code);
                            packable = BaseDataHelper.Decode(data, Platform.PC);
                        }
                        catch (Exception)
                        {
                            errors++;
                            continue;
                        }

                        // TODO: check other item unique IDs to prevent rare collisions
                        packable.UniqueId = new Random().Next(int.MinValue, int.MaxValue);

                        if (packable is BaseWeapon)
                        {
                            var weapon = (BaseWeapon)packable;
                            var viewModel = new BaseWeaponViewModel(weapon);
                            viewModels.Add(viewModel);
                        }
                        else if (packable is BaseItem)
                        {
                            var item = (BaseItem)packable;
                            var viewModel = new BaseItemViewModel(item);
                            viewModels.Add(viewModel);
                        }
                    }
                });

            if (viewModels.Count > 0)
            {
                viewModels.ForEach(vm => this.Slots.Add(vm));
                this.SelectedSlot = viewModels.First();
            }

            if (errors > 0)
            {
                yield return
                    new MyMessageBox("Failed to load " + errors.ToString(CultureInfo.InvariantCulture) + " codes.",
                                     "Warning")
                        .WithIcon(MessageBoxImage.Warning);
            }
            else if (viewModels.Count == 0)
            {
                yield return
                    new MyMessageBox("Did not find any codes in clipboard.",
                                     "Warning")
                        .WithIcon(MessageBoxImage.Warning);
            }
        }

        public IEnumerable<IResult> CopySelectedSlotCode()
        {
            yield return new DelegateResult(
                () =>
                {
                    if (this.SelectedSlot == null ||
                        (this.SelectedSlot.BaseSlot is IPackable) == false)
                    {
                        if (MyClipboard.SetText("") != MyClipboard.Result.Success)
                        {
                            MessageBox.Show("Clipboard failure.",
                                            "Error",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                        return;
                    }

                    // just a hack until I add a way to override the unique ID in Encode()
                    var copy = (IPackable)this.SelectedSlot.BaseSlot.Clone();
                    copy.UniqueId = 0;

                    var data = BaseDataHelper.Encode(copy, Platform.PC);
                    var sb = new StringBuilder();
                    sb.Append("BLOZ(");
                    sb.Append(Convert.ToBase64String(data, Base64FormattingOptions.None));
                    sb.Append(")");

                    if (MyClipboard.SetText(sb.ToString()) != MyClipboard.Result.Success)
                    {
                        MessageBox.Show("Clipboard failure.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                });
        }

        public void DuplicateSelectedSlot()
        {
            if (this.SelectedSlot == null)
            {
                return;
            }

            var copy = (IBaseSlot)this.SelectedSlot.BaseSlot.Clone();
            copy.UniqueId = new Random().Next(int.MinValue, int.MaxValue);
            // TODO: check other item unique IDs to prevent rare collisions

            if (copy is BaseWeapon)
            {
                var weapon = (BaseWeapon)copy;

                var viewModel = new BaseWeaponViewModel(weapon);
                this.Slots.Add(viewModel);
                this.SelectedSlot = viewModel;
            }
            else if (copy is BaseItem)
            {
                var item = (BaseItem)copy;

                var viewModel = new BaseItemViewModel(item);
                this.Slots.Add(viewModel);
                this.SelectedSlot = viewModel;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void UnbankSelectedSlot()
        {
            // TODO: implement me
        }

        public void DeleteSelectedSlot()
        {
            if (this.SelectedSlot == null)
            {
                return;
            }

            this.Slots.Remove(this.SelectedSlot);
            this.SelectedSlot = this.Slots.FirstOrDefault();
        }

        public void SyncAllLevels()
        {
            foreach (var viewModel in this.Slots)
            {
                if (viewModel is BaseWeaponViewModel)
                {
                    var weapon = (BaseWeaponViewModel)viewModel;
                    if ((weapon.ManufacturerGradeIndex + weapon.GameStage) >= 2)
                    {
                        weapon.ManufacturerGradeIndex = this.Character.SyncLevel;
                        weapon.GameStage = this.Character.SyncLevel;
                    }
                }
                else if (viewModel is BaseItemViewModel)
                {
                    var item = (BaseItemViewModel)viewModel;
                    if ((item.ManufacturerGradeIndex + item.GameStage) >= 2)
                    {
                        item.ManufacturerGradeIndex = this.Character.SyncLevel;
                        item.GameStage = this.Character.SyncLevel;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public void ImportData(WillowTwoPlayerSaveGame saveGame, Platform platform)
        {
            this.Slots.Clear();
            this._BrokenSlots.Clear();
            foreach (var bankSlot in saveGame.BankSlots)
            {
                IPackable slot;
                try
                {
                    slot = BaseDataHelper.Decode(bankSlot.InventorySerialNumber, platform);
                }
                catch (Exception e)
                {
                    this._BrokenSlots.Add(new KeyValuePair<BankSlot, Exception>(bankSlot, e));
                    continue;
                }

                var test = BaseDataHelper.Encode(slot, platform);
                if (bankSlot.InventorySerialNumber.SequenceEqual(test) == false)
                {
                    throw new FormatException("bank slot reencode mismatch");
                }

                if (slot is BaseWeapon)
                {
                    var viewModel = new BaseWeaponViewModel((BaseWeapon)slot);
                    this.Slots.Add(viewModel);
                }
                else if (slot is BaseItem)
                {
                    var viewModel = new BaseItemViewModel((BaseItem)slot);
                    this.Slots.Add(viewModel);
                }
            }
        }

        public void ExportData(WillowTwoPlayerSaveGame saveGame, Platform platform)
        {
            saveGame.BankSlots.Clear();

            foreach (var viewModel in this.Slots)
            {
                var slot = viewModel.BaseSlot;

                if (slot is BaseWeapon)
                {
                    var weapon = (BaseWeapon)slot;
                    var data = BaseDataHelper.Encode(weapon, platform);

                    saveGame.BankSlots.Add(new BankSlot()
                    {
                        InventorySerialNumber = data,
                    });
                }
                else if (slot is BaseItem)
                {
                    var item = (BaseItem)slot;
                    var data = BaseDataHelper.Encode(item, platform);

                    saveGame.BankSlots.Add(new BankSlot()
                    {
                        InventorySerialNumber = data,
                    });
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            this._BrokenSlots.ForEach(kv => saveGame.BankSlots.Add(kv.Key));
        }
    }
}
