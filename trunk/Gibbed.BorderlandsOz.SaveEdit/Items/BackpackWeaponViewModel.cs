﻿/* Copyright (c) 2015 Rick (rick 'at' gibbed 'dot' us)
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

using Gibbed.BorderlandsOz.FileFormats.Items;
using Gibbed.BorderlandsOz.ProtoBufFormats.WillowTwoSave;

namespace Gibbed.BorderlandsOz.SaveEdit
{
    internal class BackpackWeaponViewModel : BaseWeaponViewModel, IBackpackSlotViewModel
    {
        private readonly BackpackWeapon _Weapon;

        public IBackpackSlot BackpackSlot
        {
            get { return this._Weapon; }
        }

        public BackpackWeaponViewModel(BackpackWeapon weapon)
            : base(weapon)
        {
            this._Weapon = weapon;
        }

        #region Properties
        public QuickWeaponSlot QuickSlot
        {
            get { return this._Weapon.QuickSlot; }
            set
            {
                this._Weapon.QuickSlot = value;
                this.NotifyOfPropertyChange(() => this.QuickSlot);
                this.NotifyOfPropertyChange(() => this.DisplayGroup);
            }
        }

        public PlayerMark Mark
        {
            get { return this._Weapon.Mark; }
            set
            {
                this._Weapon.Mark = value;
                this.NotifyOfPropertyChange(() => this.Mark);
            }
        }
        #endregion

        #region Display Properties
        public override string DisplayGroup
        {
            get
            {
                if (this.QuickSlot != QuickWeaponSlot.None)
                {
                    return "Equipped";
                }

                return base.DisplayGroup;
            }
        }
        #endregion
    }
}
