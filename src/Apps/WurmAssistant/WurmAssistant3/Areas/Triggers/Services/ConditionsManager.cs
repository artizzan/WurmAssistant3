using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Triggers.Contracts.ActionQueueParsing;
using AldursLab.WurmAssistant3.Areas.Triggers.Parts.ActionQueueParsing;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Services
{
    [KernelHint(BindingHint.Singleton), PersistentObject("ActionQueueParsing_ConditionsManager")]
    public class ConditionsManager : PersistentObjectBase, IActionQueueConditions
    {
        [JsonProperty]
        Dictionary<Guid, Condition> allConditions = new Dictionary<Guid, Condition>();

        List<IActionQueueParsingCondition> actionStart = new List<IActionQueueParsingCondition>();
        List<IActionQueueParsingCondition> actionFalstart = new List<IActionQueueParsingCondition>();
        List<IActionQueueParsingCondition> actionEnd = new List<IActionQueueParsingCondition>();
        List<IActionQueueParsingCondition> actionFalsEnd = new List<IActionQueueParsingCondition>();
        List<IActionQueueParsingCondition> actionFalsEndPreviousEvent = new List<IActionQueueParsingCondition>();
        List<IActionQueueParsingCondition> levelingStart = new List<IActionQueueParsingCondition>();
        List<IActionQueueParsingCondition> levelingEnd = new List<IActionQueueParsingCondition>();

        protected override void OnPersistentDataLoaded()
        {
            MergeAllDefaultConditions();
            RebuildConditionsCache();
        }

        public IEnumerable<IActionQueueParsingCondition> ActionStart{get { return actionStart; }}
        public IEnumerable<IActionQueueParsingCondition> ActionFalstart { get { return actionFalstart; } }
        public IEnumerable<IActionQueueParsingCondition> ActionEnd { get { return actionEnd; } }
        public IEnumerable<IActionQueueParsingCondition> ActionFalsEnd { get { return actionFalsEnd; } }
        public IEnumerable<IActionQueueParsingCondition> ActionFalsEndPreviousEvent { get { return actionFalsEndPreviousEvent; } }
        public IEnumerable<IActionQueueParsingCondition> LevelingStart { get { return levelingStart; } }
        public IEnumerable<IActionQueueParsingCondition> LevelingEnd { get { return levelingEnd; } }

        public event EventHandler<EventArgs> ConditionsChanged;

        void MergeAllDefaultConditions()
        {
            // ActionStart, StartsWith
            MergeDefault("8514edfc-e23b-4348-9463-b94b9352feb8", "You start", ConditionKind.ActionStart, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("3edf0fef-7a16-427c-bf6e-96a5d3a7c862", "You continue to", ConditionKind.ActionStart, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("a36cdd5e-849d-4f41-94d2-18de449bcea6", "You throw out the line and start fishing.", ConditionKind.ActionStart, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("4e4351dd-bf0b-4178-b6e6-7bcfe451e261", "You start to string", ConditionKind.ActionStart, MatchingKind.StartsWithCaseSensitiveOrdinal);

            // ActionStart, Contains
            // none

            // ActionFalstart, StartsWith
            MergeDefault("fb2ff841-61e8-4cf4-8453-b7704baca564", "You start dragging", ConditionKind.ActionFalstart, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("8afb4eab-fbe3-4cfa-b6ce-20a66baa37fe", "You start leading", ConditionKind.ActionFalstart, MatchingKind.StartsWithCaseSensitiveOrdinal);

            // ActionEnd, StartsWith
            MergeDefault("208dffc0-8d3c-4f42-aa8c-fca1dfe2b0d5", "You improve", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("1c7b083c-e63f-4064-9f20-4569c45d25d6", "You continue on", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("0a4c60c0-042f-4e79-995a-37ffe57e8b01", "You nail", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("49e9d100-2eae-4480-9764-971811e435bb", "You dig", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("c4a9b848-ca48-476e-afdd-778fa03d175c", "You attach", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("e2749010-ef09-4e54-8340-bbc23912c36a", "You repair", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("05101f80-7e2a-4498-b581-626409961081", "You fail", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("fa296144-3c0c-42d8-a02c-5fea3560e98d", "You stop", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("8e8ab655-f9e0-4b9d-8b88-659ea6739010", "You mine some", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("604c03c0-59d9-4ffe-b815-f83b6bbfb923", "You damage", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("c24e362e-6e73-4955-bf01-9ebfaaa77711", "You harvest", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("1bec5a06-1fd3-45ee-95ea-30e20d7d2181", "You plant", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("40715e4e-4f01-4f1c-8504-d261d13d8fa8", "You pull", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("0457ff97-28fc-45d7-a183-ebd11b98112c", "You push", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("b2bb6b75-1a30-4fe8-8831-023f64e21cea", "You almost made it", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("e452edde-dbf1-4559-b9d6-f3e7135ee737", "You create", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("aebe4e6c-0d06-4844-a5bf-169f4d66add8", "You will want to", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("ce4c2016-d499-4fcc-9096-82c296f58d55", "You need to temper", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("954e065f-d36a-40ed-b53a-5caf6adc4fa3", "You finish", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("73154d95-1952-4153-8adf-84268215bf30", "Roof completed", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("a8a67f3c-ad24-4e4a-8671-572042d37fb8", "Metal needs to be glowing hot while smithing", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("c6bda0f1-d792-4d50-bfba-b0bd1f07d9e5", "You continue to build on a floo", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("42d338ad-a430-4ca5-b50e-1bac2260e30e", "You have now tended", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("9e275eb3-3401-4899-90b3-caa994f77645", "You sow", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("1e4c41b5-0cf7-4de3-8ce8-b650534764a6", "You turn", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("ef41616b-5ddd-4e2c-8786-178a51d63cdb", "You realize you harvested", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("b530400c-93ad-4a92-82c0-54696a0a8818", "The ground is cultivated", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("2073683e-3ee5-47be-9ee0-24fc3624ff31", "You gather", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("5ecf5402-4910-41a6-a432-ee895ae563ac", "You cut down", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("3fa7c6bd-152e-4ada-8ff2-1d2b4959e019", "You cut a sprout", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("28e6b2e4-200b-485a-bd7c-9666b985b03d", "You must use a", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("87f5e530-7a86-42ca-ad25-7604697e7bf2", "You notice some notches", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("bf3ba37f-e3fb-401c-a289-57eaf9ad2a22", "The dirt is packed", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("6c88fbc0-bbe1-4596-8595-cd8c5e59f53e", "You lay the foundation", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("fa195f99-30c5-4005-8d43-58578f4eaf5d", "You bandage", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("c4189911-7746-4ac1-8006-f4066e3b9b98", "You prune", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("362f17b8-ddf7-4d54-807b-21ba556c2c70", "You make a lot of errors", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("1d9fac3e-236b-4b77-b2ce-e0614ffeaf52", "You try to bandage", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("6e980d66-2ca8-4559-b3bb-47ca4302a4ee", "Sadly, the sprout does not", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("a3f8dc17-830b-4d42-b06c-1f2194e78368", "You chip away", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("66565234-8b61-4e1e-96b8-79bd067b1e38", "The last parts of the", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("6c60f597-0be3-4be0-b2cb-ec1a12f0f031", "The ground is paved", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("87032848-92e5-44aa-b30b-552751248272", "The ground is no longer paved", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("1bea4f73-40ac-4dfc-85eb-51bc4ecbcb96", "You hit rock", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("b5f7489a-5af4-433e-9701-2bd1377ab33c", "You must rest", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("cf7e3ee4-4700-400e-b5f7-fb1835e53e3f", "You use some of the dirt in one corner", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("40976d13-3b23-468c-9e14-18a664d25dcd", "You proudly close the gift", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("b3583aa4-9e9c-467e-8666-60b5abe996f2", "You add a stone and some mortar", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("867435fc-096e-47d5-81bd-20558e1da9a3", "The field is now nicely groomed", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("33905388-0527-4714-abe3-32b97d8c3d41", "You find", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("de0bf98a-ee5a-4ffa-9636-4a24454a2ca0", "You let your love change the area", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("8e124b08-9b84-4fd5-83f4-0aa50929233e", "It is now flat", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("3286bcd1-eff2-4383-9f0a-cdfd2eb76e63", "You catch a ", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("2f96da62-df8d-43b2-83ad-105bb21b02c0", "You frown as you fail to improve the power.", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("04d53559-33d0-4106-97be-36b1f9b655e6", "The field is now tended.", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("f7f3028d-cac2-4ce7-8d15-52f8e7d195cb", "The crops are growing nicely", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("4d470096-9550-4e9a-a533-129848c39c35", "The field looks better after your tending.", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("73c8908b-3110-4d8d-8e05-5046a329b7d0", "The ore will run out soon.", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("365361bc-1277-4a6f-a648-4548e3cdc4a1", "You succeed", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("3ae399c4-100d-4032-a4bf-23edb1d6fd97", "You fill the small bucket", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("20a636c9-9317-49c2-ba47-df28f6648a17", "The field is tended.", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("0f747060-9ac5-4eec-b284-b3bf07adb9f7", "The line snaps", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("59c60905-1dc3-401f-839c-ba26ab80ff7c", "You string the", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("42b7d740-a5a9-4b9b-a28c-76f0407480e2", "You seem to catch something but it escapes", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("3c02d605-1680-444a-a5d4-969705d1d011", "You fill the gem with the power of your determination", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("c79ded6d-1b23-4779-98b3-0f95571e5b4d", "The gem is of too low quality to store any power and is damaged a bit", ConditionKind.ActionEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);

            // ActionEnd, Contains
            MergeDefault("c034e6a8-fa1b-4040-a809-979a2a8ea299", "has some irregularities", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("eab9b8ae-a59b-4b37-a6c9-9cc41a826069", "has some dents", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("f42873bf-b8cb-4f88-9583-3634b1771428", "needs to be sharpened", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("411265b2-6c23-4296-a8e9-0d60420e642c", "is finished", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("c788b71f-f6e8-455e-8c0c-79e4b29c07a0", "will probably give birth", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("15a3cf01-4714-44f0-8188-7b36260df5b7", "shys away and interrupts", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("c1ac24fa-40d8-4669-8e41-18f9da2cb93a", "falls apart with a crash", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("25c45d79-d4e6-405c-b58d-36c744c05bec", "feed you more.", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("ca1ec471-4aa9-4525-8abb-113669bb94ae", "is already well tended.", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("27aa5a03-da01-4d50-b2f4-2b2a3c9092e7", "too little material", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);
            MergeDefault("4ed7df5c-6493-4bb2-a03f-f77d1b7d2e4b", "You pick some flowers.", ConditionKind.ActionEnd, MatchingKind.ContainsCaseSensitiveOrdinal);

            // ActionFalsEnd, Starts With
            MergeDefault("58576570-b007-4c68-bb20-276d7b0b5361", "You stop dragging", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("50bec963-f7ec-4f8a-80b1-ff37295558f7", "A forge made from", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("069e3d64-6b26-4fa4-90a5-440b04e13190", "It is made from", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("07c19d94-ebf4-40d2-9a0b-9e45fa8419d3", "A small, very rudimentary", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("6430c462-1b04-4d59-a457-c5d0417f7a65", "You stop leading", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("0550a7b2-7ff9-42c2-8d52-c37b0d01254c", "You fail to produce", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("d5e03de2-da9d-40a6-a8e3-c1b3e4da739d", "A tool for", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("f0761198-82b7-4fe1-b4b9-ed7700cc41dd", "The roof is finished already", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("ba3d4e98-5f00-4d71-8ff7-26d6d608606c", "A high guard tower", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("c175c764-70c5-41d8-9ef8-c7390ede3162", "You create a box side", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("65425a32-eee0-4781-951a-95ccfed0f742", "You create another box side", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("6215f90e-da04-4d48-b78f-b7bf4a8eef91", "You create yet another box side", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("03a79cbe-12c6-4b26-8f2a-919477593e7e", "You create the last box side", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("6627c8e8-8ffb-41dc-92d0-7b21c105b993", "You create a bottom", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("4850d84d-bcc4-43e0-bd77-a5c1fc1a44ac", "You create a top", ConditionKind.ActionFalsEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);

            // ActionFalsEndPreviousAction
            MergeDefault("d4c55ee2-58a6-479f-b25f-a2f725d86007", "A decorative lamp", ConditionKind.ActionFalsEndPreviousEvent, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("ba079c48-6ffa-4460-bd2d-464ee1278aa1", "A high guard tower", ConditionKind.ActionFalsEndPreviousEvent, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("ac9a5592-541b-4f72-96d2-003104aed0a6", "that you dispose of.", ConditionKind.ActionFalsEndPreviousEvent, MatchingKind.StartsWithCaseSensitiveOrdinal);

            // Leveling Start
            MergeDefault("f9929abe-4f05-4031-b9c5-c646aaa336c6", "You start to level the ground", ConditionKind.LevelingStart, MatchingKind.StartsWithCaseSensitiveOrdinal);

            // Leveling End
            MergeDefault("d5aea2ad-edf1-4f9d-8490-4662bc5430e4", "You stop leveling", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("c615f01d-f01b-496d-8880-f1863888fc05", "You can only level tiles that you are adjacent to", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("31a7c07a-b0ef-40b0-a3ef-f8906e3123a9", "You need to be standing on flat ground", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("76a41b0d-9de7-41ef-ad9e-2a99146e9f3b", "It is now flat", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("cf29f191-ce02-4a37-bae5-34c031b0946d", "You must rest", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("ac8a663a-9037-4bd0-8825-f93bb982c602", "Done.", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("65514f8d-df6d-4117-aceb-535421ad3566", "You are not strong enough to carry one more dirt pile", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("0b79076f-853f-4d07-b334-f5289e606265", "You assemble some dirt from a corner", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
            MergeDefault("63b88183-76a7-4d03-9824-c739c01b74b7", "The ground is flat here.", ConditionKind.LevelingEnd, MatchingKind.StartsWithCaseSensitiveOrdinal);
        }

        void MergeDefault(string guidString, string pattern, ConditionKind conditionKind, MatchingKind matchingKind)
        {
            var id = new Guid(guidString);
            if (!allConditions.ContainsKey(id))
            {
                allConditions.Add(
                    id,
                    new Condition(id, true)
                    {
                        MatchingKind = matchingKind,
                        ConditionKind = conditionKind,
                        Disabled = false,
                        Pattern = pattern
                    });
            }
        }

        public void ShowConditionsEditGui()
        {
            var editgui = new EditGui(this);
            if (editgui.ShowDialog() == DialogResult.OK)
            {
                allConditions = editgui.GetEditedConditions()
                                       .ToDictionary(condition => condition.ConditionId, condition => condition);
                
                RebuildConditionsCache();
                OnConditionsChanged();
            }
        }

        protected virtual void OnConditionsChanged()
        {
            var handler = ConditionsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void RestoreDefaults()
        {
            allConditions = new Dictionary<Guid, Condition>();
            MergeAllDefaultConditions();
            RebuildConditionsCache();
            OnConditionsChanged();
        }

        private void RebuildConditionsCache()
        {
            actionStart = CreateKindList(ConditionKind.ActionStart);
            actionFalstart = CreateKindList(ConditionKind.ActionFalstart);
            actionEnd = CreateKindList(ConditionKind.ActionEnd);
            actionFalsEnd = CreateKindList(ConditionKind.ActionFalsEnd);
            actionFalsEndPreviousEvent = CreateKindList(ConditionKind.ActionFalsEndPreviousEvent);
            levelingStart = CreateKindList(ConditionKind.LevelingStart);
            levelingEnd = CreateKindList(ConditionKind.LevelingEnd);
        }

        private List<IActionQueueParsingCondition> CreateKindList(ConditionKind kind)
        {
            return allConditions.Where(pair => pair.Value.ConditionKind == kind && !pair.Value.Disabled)
                             .Select(pair => pair.Value)
                             .Cast<IActionQueueParsingCondition>()
                             .ToList();
        }

        public IEnumerable<Condition> GetCurrentConditionsCopies()
        {
            return allConditions.Select(pair => pair.Value.CreateCopy()).ToList();
        }
    }
}
