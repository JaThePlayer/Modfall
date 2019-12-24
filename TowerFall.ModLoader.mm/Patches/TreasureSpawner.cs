using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall.ModLoader.mm;

namespace TowerFall
{
    class patch_TreasureSpawner : TreasureSpawner
    {
        public patch_TreasureSpawner() : base(null, null)
        {
            // noop
        }

        public ArrowTypes GetRandomArrowType(bool includeDefaultArrows)
        {
            List<ArrowTypes> list = new List<ArrowTypes>();
            if (includeDefaultArrows && !this.Exclusions.Contains(Pickups.Arrows))
            {
                list.Add(ArrowTypes.Normal);
            }
            if (!this.Exclusions.Contains(Pickups.BombArrows))
            {
                list.Add(ArrowTypes.Bomb);
            }
            if (!this.Exclusions.Contains(Pickups.LaserArrows))
            {
                list.Add(ArrowTypes.Laser);
            }
            if (!this.Exclusions.Contains(Pickups.BrambleArrows))
            {
                list.Add(ArrowTypes.Bramble);
            }
            if (!this.Exclusions.Contains(Pickups.DrillArrows))
            {
                list.Add(ArrowTypes.Drill);
            }
            if (SaveData.Instance.Unlocks.SunkenCity && !this.Exclusions.Contains(Pickups.BoltArrows))
            {
                list.Add(ArrowTypes.Bolt);
            }
            if (SaveData.Instance.Unlocks.TowerForge && !this.Exclusions.Contains(Pickups.SuperBombArrows))
            {
                list.Add(ArrowTypes.SuperBomb);
            }
            if (SaveData.Instance.Unlocks.Ascension && !this.Exclusions.Contains(Pickups.FeatherArrows))
            {
                list.Add(ArrowTypes.Feather);
            }
            if (GameData.DarkWorldDLC && !this.Exclusions.Contains(Pickups.TriggerArrows))
            {
                list.Add(ArrowTypes.Trigger);
            }
            if (GameData.DarkWorldDLC && !this.Exclusions.Contains(Pickups.PrismArrows))
            {
                list.Add(ArrowTypes.Prism);
            }

            // Add modded arrows to the mix
            for (int i = 0; i < ModArrow.ModArrowTypes.Count; i++)
            {
                list.Add((ArrowTypes)i + 11);
            }

            if (list.Count == 0)
            {
                return ArrowTypes.Normal;
            }

            return Random.Choose(list);
        }

        public List<Pickups> GetArrowShufflePickups()
        {
            List<Pickups> list = new List<Pickups>();
            if (!this.Exclusions.Contains(Pickups.BombArrows))
            {
                list.Add(Pickups.BombArrows);
            }
            if (!this.Exclusions.Contains(Pickups.LaserArrows))
            {
                list.Add(Pickups.LaserArrows);
            }
            if (!this.Exclusions.Contains(Pickups.BrambleArrows))
            {
                list.Add(Pickups.BrambleArrows);
            }
            if (!this.Exclusions.Contains(Pickups.DrillArrows))
            {
                list.Add(Pickups.DrillArrows);
            }
            if (SaveData.Instance.Unlocks.SunkenCity && !this.Exclusions.Contains(Pickups.BoltArrows))
            {
                list.Add(Pickups.BoltArrows);
            }
            if (SaveData.Instance.Unlocks.TowerForge && !this.Exclusions.Contains(Pickups.SuperBombArrows))
            {
                list.Add(Pickups.SuperBombArrows);
            }
            if (SaveData.Instance.Unlocks.Ascension && !this.Exclusions.Contains(Pickups.FeatherArrows))
            {
                list.Add(Pickups.FeatherArrows);
            }
            if (GameData.DarkWorldDLC && !this.Exclusions.Contains(Pickups.TriggerArrows))
            {
                list.Add(Pickups.TriggerArrows);
            }
            if (GameData.DarkWorldDLC && !this.Exclusions.Contains(Pickups.PrismArrows))
            {
                list.Add(Pickups.PrismArrows);
            }

            // Add modded arrows to the mix
            foreach (int type in patch_Pickup.ModArrowPickupTypes.Keys)
            {
                list.Add((Pickups)type);
            }

            list.Shuffle();
            while (list.Count > 2)
            {
                list.RemoveAt(0);
            }
            return list;
        }
    }
}
