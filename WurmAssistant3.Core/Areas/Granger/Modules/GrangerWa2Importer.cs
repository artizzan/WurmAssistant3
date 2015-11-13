using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Views;
using Dtos = WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules
{
    class GrangerWa2Importer
    {
        private readonly GrangerFeature grangerFeature;
        private readonly GrangerContext context;
        private readonly ILogger logger;

        public GrangerWa2Importer(GrangerFeature grangerFeature, GrangerContext context, ILogger logger)
        {
            if (grangerFeature == null) throw new ArgumentNullException("context");
            if (context == null) throw new ArgumentNullException("context");
            if (logger == null) throw new ArgumentNullException("logger");
            this.grangerFeature = grangerFeature;
            this.context = context;
            this.logger = logger;
        }

        public async Task ImportFromDtoAsync(Dtos.WurmAssistantDto dto)
        {
            List<ImportItem<Dtos.Creature, CreatureEntity>> importItems =
                new List<ImportItem<Dtos.Creature, CreatureEntity>>();
            var uniquelyIdCreatures = dto.Creatures.Where(creature => creature.GlobalId != null).ToArray();
            PrepareUniquelyIdCreatures(uniquelyIdCreatures, importItems);
            var nonUniquelyIdCreatures = dto.Creatures.Where(creature => creature.GlobalId == null).ToArray();
            PrepareNonUniquelyIdCreatures(nonUniquelyIdCreatures, importItems);

            var mergeAssistantView = new ImportMergeAssistantView(importItems, logger);
            mergeAssistantView.Text = "Choose creatures to import...";
            mergeAssistantView.StartPosition = FormStartPosition.CenterScreen;
            mergeAssistantView.Show();
            await mergeAssistantView.Completed;
        }

        private void PrepareUniquelyIdCreatures(Dtos.Creature[] uniquelyIdCreatures,
            List<ImportItem<Dtos.Creature, CreatureEntity>> importItems)
        {
            foreach (var creature in uniquelyIdCreatures)
            {
                importItems.Add(new ImportItem<Dtos.Creature, CreatureEntity>()
                {
                    Source = creature,
                    Blocked = true,
                    Comment = "Importing uniquely identified creatures is not yet supported",
                });
            }
        }

        private void PrepareNonUniquelyIdCreatures(Dtos.Creature[] nonUniquelyIdCreatures,
            List<ImportItem<Dtos.Creature, CreatureEntity>> importItems)
        {
            foreach (var c in nonUniquelyIdCreatures)
            {
                var creature = c;

                var item = new ImportItem<Dtos.Creature, CreatureEntity>();
                item.Source = creature;
                var existingCreature =
                    context.Creatures.FirstOrDefault(
                        entity => entity.Herd == creature.HerdId && entity.Name == creature.Name);
                if (existingCreature != null)
                {
                    item.Destination = existingCreature;
                }
                item.ResolutionAction = (result, source, dest) =>
                {
                    if (result == MergeResult.AddAsNew)
                    {
                        if (context.Herds.All(entity => entity.HerdID != source.HerdId))
                        {
                            context.InsertHerd(source.HerdId);
                        }

                        if (
                            context.Creatures.Any(
                                entity => entity.Name == creature.Name && entity.Herd == creature.HerdId))
                        {
                            throw new InvalidOperationException(
                                "Creature with this name and herd already exist in DB. Move this creature to another herd before importing.");
                        }

                        context.InsertCreature(new CreatureEntity()
                        {
                            Herd = source.HerdId,
                            Age = ParseAge(source.CreatureAge),
                            BirthDate = source.BirthDate,
                            BrandedFor = source.BrandedFor,
                            Color = ParseColor(source.CreatureColor),
                            Comments = source.Comments,
                            EpicCurve = source.EpicCurve,
                            FatherName = source.FatherName,
                            GroomedOn = source.GroomedOn,
                            Id = CreatureEntity.GenerateNewCreatureId(context),
                            IsMale = source.IsMale,
                            MotherName = source.MotherName,
                            Name = source.Name,
                            NotInMood = source.NotInMood,
                            PairedWith = source.PairedWith,
                            PregnantUntil = source.PregnantUntil,
                            ServerName = source.ServerName,
                            SpecialTags = source.SpecialTags.ToHashSet(),
                            Traits = ParseTraits(source.CreatureTraits),
                            TakenCareOfBy = source.TakenCareOfBy,
                            TraitsInspectedAtSkill = source.TraitsInspectedAtSkill,
                        });
                    }
                };
                importItems.Add(item);
            }
        }

        private CreatureAge ParseAge(string creatureAge)
        {
            return CreatureAge.CreateAgeFromEnumString(creatureAge);
        }

        private CreatureColor ParseColor(string creatureColor)
        {
            return CreatureColor.CreateColorFromEnumString(creatureColor);
        }

        private List<CreatureTrait> ParseTraits(List<string> creatureTraits)
        {
            return creatureTraits.Select(CreatureTrait.FromWurmTextRepr).ToList();
        }
    }
}