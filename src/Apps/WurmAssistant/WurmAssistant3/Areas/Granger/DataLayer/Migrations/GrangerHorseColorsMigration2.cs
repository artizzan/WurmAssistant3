using System;
using System.Linq;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.DataLayer.Migrations
{
    [KernelBind(BindingHint.Transient), UsedImplicitly]
    public class GrangerHorseColorsMigration2
    {
        [NotNull] readonly GrangerSimpleDb grangerSimpleDb;
        [NotNull] readonly DefaultBreedingEvaluatorOptionsData defaultBreedingEvaluatorOptionsData;

        public GrangerHorseColorsMigration2([NotNull] GrangerSimpleDb grangerSimpleDb, [NotNull] DefaultBreedingEvaluatorOptionsData defaultBreedingEvaluatorOptionsData)
        {
            if (grangerSimpleDb == null) throw new ArgumentNullException(nameof(grangerSimpleDb));
            if (defaultBreedingEvaluatorOptionsData == null) throw new ArgumentNullException(nameof(defaultBreedingEvaluatorOptionsData));
            this.grangerSimpleDb = grangerSimpleDb;
            this.defaultBreedingEvaluatorOptionsData = defaultBreedingEvaluatorOptionsData;
        }

        public void Run()
        {
            foreach (var creatureEntity in grangerSimpleDb.Creatures.Values)
            {
                switch (creatureEntity.CreatureColorId)
                {
                    case "Unknown":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Unknown;
                        break;
                    case "Black":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Black;
                        break;
                    case "White":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.White;
                        break;
                    case "Grey":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Grey;
                        break;
                    case "Brown":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Brown;
                        break;
                    case "Gold":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Gold;
                        break;
                    case "BloodBay":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.BloodBay;
                        break;
                    case "EbonyBlack":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.EbonyBlack;
                        break;
                    case "PiebaldPinto":
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.PiebaldPinto;
                        break;
                    case null:
                        creatureEntity.CreatureColorId = DefaultCreatureColorIds.Unknown;
                        break;
                    default:
                        break;
                }
            }

            foreach (var creatureColorEntity in grangerSimpleDb.CreatureColors.Values.ToArray())
            {
                switch (creatureColorEntity.Id)
                {
                    case "Unknown":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "Unknown",
                            DefaultCreatureColorIds.Unknown,
                            "Unknown",
                            string.Empty);
                        break;
                    case "Black":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "Black",
                            DefaultCreatureColorIds.Black,
                            "Black",
                            "black");
                        break;
                    case "White":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "White",
                            DefaultCreatureColorIds.White,
                            "White",
                            "white");
                        break;
                    case "Grey":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "Grey",
                            DefaultCreatureColorIds.Grey,
                            "Grey",
                            "grey");
                        break;
                    case "Brown":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "Brown",
                            DefaultCreatureColorIds.Brown,
                            "Brown",
                            "brown");
                        break;
                    case "Gold":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "Gold",
                            DefaultCreatureColorIds.Gold,
                            "Gold",
                            "gold");
                        break;
                    case "BloodBay":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "BloodBay",
                            DefaultCreatureColorIds.BloodBay,
                            "Blood Bay",
                            "blood bay");
                        break;
                    case "EbonyBlack":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "EbonyBlack",
                            DefaultCreatureColorIds.EbonyBlack,
                            "Ebony Black",
                            "ebony black");
                        break;
                    case "PiebaldPinto":
                        UpdateCreatureColorEntity(creatureColorEntity,
                            "PiebaldPinto",
                            DefaultCreatureColorIds.PiebaldPinto,
                            "Piebald Pinto",
                            "piebald pinto");
                        break;
                    default:
                        UpdateCreatureColorEntity(creatureColorEntity,
                            creatureColorEntity.Id,
                            creatureColorEntity.Id,
                            creatureColorEntity.Id,
                            string.Empty);
                        break;
                }
            }

            grangerSimpleDb.Save();
        }

        void UpdateCreatureColorEntity(CreatureColorEntity creatureColorEntity, string oldId, string newId, string displayName, string wurmLogText)
        {
            creatureColorEntity.Id = newId;
            creatureColorEntity.DisplayName = displayName;
            creatureColorEntity.WurmLogText = wurmLogText;

            if (newId != oldId)
            {
                var oldvalue = defaultBreedingEvaluatorOptionsData.CreatureColorValuesForEntityId[oldId];
                defaultBreedingEvaluatorOptionsData.CreatureColorValuesForEntityId.Remove(oldId);
                defaultBreedingEvaluatorOptionsData.CreatureColorValuesForEntityId[newId] = oldvalue;

                grangerSimpleDb.CreatureColors.Remove(oldId);
                grangerSimpleDb.CreatureColors[newId] = creatureColorEntity;
            }
        }
    }
}