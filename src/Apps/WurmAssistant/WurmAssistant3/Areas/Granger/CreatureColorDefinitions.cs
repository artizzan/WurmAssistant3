using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            SetupDefaultColor("Unknown", Color.Empty);
            SetupDefaultColor("Black", Color.DarkSlateGray);
            SetupDefaultColor("White", Color.GhostWhite);
            SetupDefaultColor("Grey", Color.LightGray);
            SetupDefaultColor("Brown", Color.Brown);
            SetupDefaultColor("Gold", Color.Gold);
            SetupDefaultColor("BloodBay", Color.RosyBrown);
            SetupDefaultColor("EbonyBlack", Color.Black);
            SetupDefaultColor("PiebaldPinto", Color.DarkGray);
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

        public CreatureColor GetForId(string text)
        {
            return new CreatureColor(grangerContext.GetCreatureColor(text));
        }

        public void AddNew(string newId)
        {
            grangerContext.AddOrUpdateCreatureColor(new CreatureColorEntity()
            {
                Id = newId
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
    }
}