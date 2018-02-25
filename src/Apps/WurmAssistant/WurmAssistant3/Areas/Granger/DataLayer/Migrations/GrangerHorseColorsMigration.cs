using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.DataLayer.Migrations
{
    [KernelBind(BindingHint.Transient), UsedImplicitly]
    public class GrangerHorseColorsMigration
    {
        readonly GrangerSimpleDb grangerSimpleDb;

        public GrangerHorseColorsMigration([NotNull] GrangerSimpleDb grangerSimpleDb)
        {
            if (grangerSimpleDb == null) throw new ArgumentNullException(nameof(grangerSimpleDb));
            this.grangerSimpleDb = grangerSimpleDb;
        }

        public void Run()
        {
            foreach (var creatureEntity in grangerSimpleDb.Creatures.Values)
            {
                switch (creatureEntity.CreatureColorId)
                {
                    case "0":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Unknown;
                        break;
                    case "1":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Black;
                        break;
                    case "2":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.White;
                        break;
                    case "3":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Grey;
                        break;
                    case "4":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Brown;
                        break;
                    case "5":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Gold;
                        break;
                    case "6":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.BloodBay;
                        break;
                    case "7":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.EbonyBlack;
                        break;
                    case "8":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.PiebaldPinto;
                        break;
                    case null:
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Unknown;
                        break;
                    default:
                        break;
                }
            }
            grangerSimpleDb.Save();
        }
    }
}
