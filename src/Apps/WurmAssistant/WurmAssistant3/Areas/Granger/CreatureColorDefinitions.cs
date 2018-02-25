using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class CreatureColorDefinitions
    {
        readonly GrangerContext grangerContext;

        public event EventHandler<EventArgs> DefinitionsChanged;

        public CreatureColorDefinitions([NotNull] GrangerContext grangerContext)
        {
            if (grangerContext == null) throw new ArgumentNullException(nameof(grangerContext));
            this.grangerContext = grangerContext;

            SetupDefaultColors();
        }

        void SetupDefaultColors()
        {
            SetupDefaultColor(DefaultCreatureColorIds.Unknown, Color.Empty);
            SetupDefaultColor(DefaultCreatureColorIds.Black, Color.DarkSlateGray);
            SetupDefaultColor(DefaultCreatureColorIds.White, Color.GhostWhite);
            SetupDefaultColor(DefaultCreatureColorIds.Grey, Color.LightGray);
            SetupDefaultColor(DefaultCreatureColorIds.Brown, Color.Brown);
            SetupDefaultColor(DefaultCreatureColorIds.Gold, Color.Gold);
            SetupDefaultColor(DefaultCreatureColorIds.BloodBay, Color.RosyBrown);
            SetupDefaultColor(DefaultCreatureColorIds.EbonyBlack, Color.Black);
            SetupDefaultColor(DefaultCreatureColorIds.PiebaldPinto, Color.DarkGray);
            grangerContext.SubmitChanges();
        }

        void SetupDefaultColor(string id, Color color)
        {
            var existing = grangerContext.GetCreatureColor(id);
            if (existing.IsUnknown)
            {
                grangerContext.SeedCreatureColor(new CreatureColorEntity()
                {
                    Id = id,
                    Color = color,
                    IsReadOnly = true
                });
                OnDefinitionsChanged();
            }
        }

        public IEnumerable<CreatureColor> GetColors()
        {
            return grangerContext.CreatureColorEntities.Select(entity => new CreatureColor(entity));
        }

        public bool Exists(string colorId)
        {
            return grangerContext.CreatureColorExists(colorId);
        }

        public CreatureColor GetForId(string colorId)
        {
            return new CreatureColor(grangerContext.GetCreatureColor(colorId));
        }

        public void AddNew(string newId)
        {
            grangerContext.AddOrUpdateCreatureColor(new CreatureColorEntity()
            {
                Id = newId,
                WurmLogText = newId.ToLowerInvariant(),
                DisplayName = newId.Capitalize()
            });
            OnDefinitionsChanged();
        }

        public void UpdateColor(string id, Color color)
        {
            var entity = grangerContext.GetCreatureColor(id);
            entity.Color = color;
            grangerContext.AddOrUpdateCreatureColor(entity);
            OnDefinitionsChanged();
        }

        public void UpdateWurmLogText(string id, string wurmLogText)
        {
            var entity = grangerContext.GetCreatureColor(id);
            entity.WurmLogText = wurmLogText;
            grangerContext.AddOrUpdateCreatureColor(entity);
            OnDefinitionsChanged();
        }

        public void UpdateDisplayName(string id, string displayName)
        {
            var entity = grangerContext.GetCreatureColor(id);
            entity.DisplayName = displayName;
            grangerContext.AddOrUpdateCreatureColor(entity);
            OnDefinitionsChanged();
        }

        public void Remove(string id)
        {
            var entity = grangerContext.GetCreatureColor(id);
            grangerContext.RemoveCreatureColor(entity);
            OnDefinitionsChanged();
        }

        protected virtual void OnDefinitionsChanged()
        {
            DefinitionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public string GetColorIdByWurmLogText(string colorWurmLogText)
        {
            var colorId = grangerContext.CreatureColorEntities
                                        .Where(entity => entity.WurmLogText == colorWurmLogText)
                                        .Select(entity => entity.Id)
                                        .FirstOrDefault();
            if (colorId == null)
            {
                colorId = colorWurmLogText.Capitalize();
                AddNew(colorId);
                UpdateColor(colorId, Color.Empty);
                UpdateWurmLogText(colorId, colorWurmLogText);
                UpdateDisplayName(colorId, colorId);
            }

            return colorId;
        }
    }
}