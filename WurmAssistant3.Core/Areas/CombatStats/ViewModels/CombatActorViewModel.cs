using System;
using System.Globalization;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.ViewModels
{
    public class CombatActorViewModel
    {
        public CombatActorPairStats PairStats { get; private set; }
        public CombatActor AttackerActor { get; private set; }
        public CombatActor DefenderActor { get; private set; }

        public CombatActorViewModel(CombatActorPairStats pairStats, string mainActorName)
        {
            if (pairStats == null) throw new ArgumentNullException("pairStats");
            if (mainActorName == null) throw new ArgumentNullException("mainActorName");
            PairStats = pairStats;
            AttackerActor = pairStats.GetActorByName(mainActorName);
            DefenderActor = pairStats.ActorOne == AttackerActor ? pairStats.ActorTwo : pairStats.ActorOne;
        }

        [UsedImplicitly]
        public string CombatPair { get { return PairStats.PairId; } }

        [UsedImplicitly]
        public string AttackerName { get { return AttackerActor.Name; } }

        [UsedImplicitly]
        public string DefenderName { get { return DefenderActor.Name; } }

        [UsedImplicitly]
        public string DamageCaused { get { return BuildDamageStats(AttackerActor.DamageCausedStats); } }

        string BuildDamageStats(DamageCausedStats damageCausedStats)
        {
            var aggregation = damageCausedStats.Attacks.GroupBy(attack => attack.Damage)
                                               .Select(attacks =>
                                                   new
                                                   {
                                                       DamageCaused = attacks.Key,
                                                       AttackStrength =
                                                           string.Join(", ",
                                                               attacks.GroupBy(attack => attack.Strength)
                                                                      .Select(
                                                                          grouping =>
                                                                              grouping.Key + " x" + grouping.Count()))
                                                   });
            return string.Join(Environment.NewLine, aggregation.Select(arg => arg.DamageCaused + ": " + arg.AttackStrength));
        }

        [UsedImplicitly]
        public string Parries { get { return BuildParryStats(DefenderActor.ParryCounts); } }

        string BuildParryStats(Parry parryCounts)
        {
            return string.Join(Environment.NewLine, parryCounts.Counts.Select(pair => pair.Key + ": " + pair.Value));
        }

        [UsedImplicitly]
        public string TargetPrefs { get { return BuildTargetPrefs(AttackerActor.TargetPreferenceCounts); } }

        string BuildTargetPrefs(TargetPreference targetPreferenceCounts)
        {
            return string.Join(Environment.NewLine, targetPreferenceCounts.Counts.Select(pair => pair.Key + ": " + pair.Value));
        }

        [UsedImplicitly]
        public string Evasions { get { return BuildEvasionCounts(DefenderActor.EvadedCounts); } }

        string BuildEvasionCounts(Evasion evadedCounts)
        {
            return string.Join(Environment.NewLine, evadedCounts.Counts.Select(pair => pair.Key + ": " + pair.Value));
        }

        [UsedImplicitly]
        public string Misses { get { return AttackerActor.MissesCount.ToString(); } }

        [UsedImplicitly]
        public string GlancingBlows { get { return AttackerActor.GlancingBlowsCount.ToString(); } }

        [UsedImplicitly]
        public string ShieldBlocks { get { return DefenderActor.ShieldBlockCount.ToString(); } }

        [UsedImplicitly]
        public string WeaponSpellAttacks { get { return BuildSpellAttacks(AttackerActor); } }

        string BuildSpellAttacks(CombatActor actor)
        {
            string result = string.Empty;
            if (actor.AffectingHits > 0)
            {
                result += "Affecting: " + actor.AffectingHits + Environment.NewLine;
            }
            if (actor.FreezingHits > 0)
            {
                result += "Freezing: " + actor.FreezingHits + Environment.NewLine;
            }
            if (actor.FlamingHits > 0)
            {
                result += "Burning: " + actor.FlamingHits + Environment.NewLine;
            }
            return result;
        }

        [UsedImplicitly]
        public int TotalHits
        {
            get { return AttackerActor.DamageCausedStats.Attacks.Count(); }
        }

        [UsedImplicitly]
        public int TotalMainActorAttacks
        {
            get
            {
                return AttackerActor.DamageCausedStats.Attacks.Count()
                       + AttackerActor.GlancingBlowsCount
                       + AttackerActor.MissesCount
                       + DefenderActor.ShieldBlockCount
                       + DefenderActor.ParryCounts.Total
                       + DefenderActor.EvadedCounts.Total;
            }
        }

        [UsedImplicitly]
        public int TotalSecondaryActorAttacks
        {
            get
            {
                return DefenderActor.DamageCausedStats.Attacks.Count()
                       + DefenderActor.GlancingBlowsCount
                       + DefenderActor.MissesCount
                       + AttackerActor.ShieldBlockCount
                       + AttackerActor.ParryCounts.Total
                       + AttackerActor.EvadedCounts.Total;
            }
        }

        [UsedImplicitly]
        public string HitRatio
        {
            get
            {
                return
                    (AttackerActor.DamageCausedStats.Attacks.Count() / (double) TotalMainActorAttacks).ToString("P",
                        CultureInfo.CurrentCulture);
            }
        }

        [UsedImplicitly]
        public string MissRatio
        {
            get
            {
                return
                    (AttackerActor.MissesCount/(double) TotalMainActorAttacks).ToString("P",
                        CultureInfo.CurrentCulture);
            }
        }

        [UsedImplicitly]
        public string GlanceRatio
        {
            get
            {
                return
                    (AttackerActor.GlancingBlowsCount / (double)TotalMainActorAttacks).ToString("P",
                        CultureInfo.CurrentCulture);
            }
        }

        [UsedImplicitly]
        public string BlockRatio
        {
            get
            {
                return
                    (DefenderActor.ShieldBlockCount / (double)TotalMainActorAttacks).ToString("P",
                        CultureInfo.CurrentCulture);
            }
        }

        [UsedImplicitly]
        public string ParryRatio
        {
            get
            {
                return
                    (DefenderActor.ParryCounts.Total / (double)TotalMainActorAttacks).ToString("P",
                        CultureInfo.CurrentCulture);
            }
        }

        [UsedImplicitly]
        public string EvadeRatio
        {
            get
            {
                return
                    (DefenderActor.EvadedCounts.Total / (double)TotalMainActorAttacks).ToString("P",
                        CultureInfo.CurrentCulture);
            }
        }
    }
}
