using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using MonoMod;
using Monocle;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0649
namespace TowerFall
{
    class patch_QuestSpawnPortal : QuestSpawnPortal
    {
        public patch_QuestSpawnPortal() : base(Vector2.Zero, null)
        {
            // no-op
        }

        // exposing private fields
        [MonoModIgnore]
        private Facing lastFacing;
        [MonoModIgnore]
        private Queue<string> toSpawn;
        [MonoModIgnore]
        private Counter addCounter;
        [MonoModIgnore]
        private bool autoDisappear;
        [MonoModIgnore]
        private Wiggler spawnWiggler;
        [MonoModIgnore]
        private extern bool Disappear();

        public static Dictionary<string, Type> CustomEnemies = new Dictionary<string, Type>();

        private readonly static Type[] _EnemyCtorTypeArray = new Type[] { typeof(Vector2), typeof(Facing)};

        private void FinishSpawn(Sprite<int> sprite)
        {
            if (sprite.CurrentAnimID != 1 || sprite.CurrentFrame != 25 || toSpawn.Count == 0)
            {
                return;
            }
            Facing facing;
            if (X == 160f)
            {
                facing = lastFacing;
                if (lastFacing == Facing.Left)
                {
                    lastFacing = Facing.Right;
                }
                else
                {
                    lastFacing = Facing.Left;
                }
            }
            else
            {
                facing = ((X < 160f) ? Facing.Right : Facing.Left);
            }
            string text = toSpawn.Dequeue();
            if (Events.QuestSpawnPortal.SpawnEnemy(text))
            {

            }
            else if (CustomEnemies.ContainsKey(text))
            {
                Enemy enemy = (Enemy)CustomEnemies[text].GetConstructor(_EnemyCtorTypeArray).Invoke(new object[] { Position, facing });
                Level.Add(enemy);
            } else
            {
                switch (text)
                {
                    default:
                        throw new Exception("Unknown enemy type: " + text);
                    case "Worm":
                        Level.Add(new Worm(Position + new Vector2(0f, 5f)));
                        break;
                    case "Mole":
                        Level.Add(new Mole(Position, facing));
                        break;
                    case "TechnoMage":
                        Level.Add(new TechnoMage(Position, facing));
                        break;
                    case "FlamingSkull":
                        Level.Add(new FlamingSkull(Position, facing));
                        break;
                    case "Exploder":
                        Level.Add(new Exploder(Position, facing, Nodes));
                        break;
                    case "Birdman":
                        Level.Add(new Birdman(Position, facing, darkWorld: false));
                        break;
                    case "DarkBirdman":
                        Level.Add(new Birdman(Position, facing, darkWorld: true));
                        break;
                    case "EvilCrystal":
                        Level.Add(new EvilCrystal(Position, facing, EvilCrystal.CrystalColors.Red, Nodes));
                        break;
                    case "BlueCrystal":
                        Level.Add(new EvilCrystal(Position, facing, EvilCrystal.CrystalColors.Blue, Nodes));
                        break;
                    case "BoltCrystal":
                        Level.Add(new EvilCrystal(Position, facing, EvilCrystal.CrystalColors.Green, Nodes));
                        break;
                    case "PrismCrystal":
                        Level.Add(new EvilCrystal(Position, facing, EvilCrystal.CrystalColors.Pink, Nodes));
                        break;
                    case "Ghost":
                        Level.Add(new Ghost(Position, facing, Nodes, Ghost.GhostTypes.Blue));
                        break;
                    case "GreenGhost":
                        Level.Add(new Ghost(Position, facing, Nodes, Ghost.GhostTypes.Green));
                        break;
                    case "Elemental":
                        Level.Add(new Ghost(Position, facing, Nodes, Ghost.GhostTypes.Fire));
                        break;
                    case "GreenElemental":
                        Level.Add(new Ghost(Position, facing, Nodes, Ghost.GhostTypes.GreenFire));
                        break;
                    case "Slime":
                        Level.Add(new Slime(Position + new Vector2(0f, 5f), facing, Slime.SlimeColors.Green));
                        break;
                    case "RedSlime":
                        Level.Add(new Slime(Position + new Vector2(0f, 5f), facing, Slime.SlimeColors.Red));
                        break;
                    case "BlueSlime":
                        Level.Add(new Slime(Position + new Vector2(0f, 5f), facing, Slime.SlimeColors.Blue));
                        break;
                    case "Bat":
                        Level.Add(new Bat(Position, facing, Bat.BatType.Eye));
                        break;
                    case "BombBat":
                        Level.Add(new Bat(Position, facing, Bat.BatType.Bomb));
                        break;
                    case "SuperBombBat":
                        Level.Add(new Bat(Position, facing, Bat.BatType.SuperBomb));
                        break;
                    case "Crow":
                        Level.Add(new Bat(Position, facing, Bat.BatType.Bird));
                        break;
                    case "Cultist":
                        Level.Add(new Cultist(Position + new Vector2(0f, 8f), facing, Cultist.CultistTypes.Normal));
                        break;
                    case "ScytheCultist":
                        Level.Add(new Cultist(Position + new Vector2(0f, 8f), facing, Cultist.CultistTypes.Scythe));
                        break;
                    case "BossCultist":
                        Level.Add(new Cultist(Position + new Vector2(0f, 8f), facing, Cultist.CultistTypes.Boss));
                        break;
                    case "Skeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "SkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: true, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "BombSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Bomb, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "BombSkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Bomb, hasShield: true, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "LaserSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Laser, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "LaserSkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Laser, hasShield: true, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "MimicSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: false, hasWings: false, canMimic: true, jester: false, boss: false));
                        break;
                    case "DrillSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Drill, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "DrillSkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Drill, hasShield: true, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "BoltSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Bolt, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "BoltSkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Bolt, hasShield: true, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "Jester":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: true, hasWings: false, canMimic: false, jester: true, boss: false));
                        break;
                    case "BossSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: true));
                        break;
                    case "BossWingSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: false, hasWings: true, canMimic: false, jester: false, boss: true));
                        break;
                    case "WingSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: false, hasWings: true, canMimic: false, jester: false, boss: false));
                        break;
                    case "WingSkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Normal, hasShield: true, hasWings: true, canMimic: false, jester: false, boss: false));
                        break;
                    case "TriggerSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Trigger, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "TriggerSkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Trigger, hasShield: true, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "PrismSkeleton":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Prism, hasShield: false, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                    case "PrismSkeletonS":
                        Level.Add(new Skeleton(Position + new Vector2(0f, 2f), facing, ArrowTypes.Prism, hasShield: true, hasWings: false, canMimic: false, jester: false, boss: false));
                        break;
                }
            }
            
            addCounter.Set(2);
            if (toSpawn.Count > 0)
            {
                sprite.Play(1, restart: true);
            }
            else
            {
                sprite.Play(0);
                if (autoDisappear)
                {
                    Disappear();
                }
            }
            Sounds.sfx_portalSpawn.Play(X);
            Level.ParticlesFG.Emit(Particles.EnPortalSpawn, 16, Position, new Vector2(6f, 8f));
            spawnWiggler.Start();
        }

    }
}
