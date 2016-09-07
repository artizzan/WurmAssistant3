using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;

namespace AldursLab.WurmAssistant3.Areas.Granger.ImportExport.Legacy
{
    public class HerdExporter
    {
        public XDocument CreateXml(GrangerContext context, string herdName)
        {
            if (herdName == null) throw new GrangerException("No herd specified");

            var creatures = context.Creatures.Where(x => x.Herd == herdName).ToArray();

            if (creatures.Length == 0)
            {
                throw new GrangerException(string.Format("No creatures found in {0} herd or herd did not exist", herdName));
            }

            XElement root = new XElement("Herd", new XAttribute("OriginalHerdName", herdName));

            foreach (CreatureEntity creatureEntity in creatures)
            {
                var creature =
                    new XElement("Creature",
                        new XElement("Name", creatureEntity.Name),
                        new XElement("Father", creatureEntity.FatherName),
                        new XElement("Mother", creatureEntity.MotherName),
                        new XElement("Traits", GetTraitListXML(creatureEntity)),
                        new XElement("NotInMoodUntil",
                            creatureEntity.NotInMood?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                        new XElement("PregnantUntil",
                            creatureEntity.PregnantUntil?.ToString(
                                CultureInfo.InvariantCulture) ?? string.Empty),
                        new XElement("GroomedOn",
                            creatureEntity.GroomedOn?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                        new XElement("Gender", GetGender(creatureEntity)),
                        new XElement("CaredBy", creatureEntity.TakenCareOfBy),
                        new XElement("InspectSkill",
                            creatureEntity.TraitsInspectedAtSkill,
                            new XAttribute("IsEpic",
                                creatureEntity.EpicCurve.HasValue
                                    ? creatureEntity.EpicCurve.ToString()
                                    : bool.FalseString)),
                        new XElement("Age", creatureEntity.Age),
                        new XElement("CreatureColorId", creatureEntity.CreatureColorId),
                        new XElement("Comments", creatureEntity.Comments),
                        new XElement("Tags", creatureEntity.SpecialTagsRaw),
                        new XElement("BrandedFor", creatureEntity.BrandedFor),
                        new XElement("ServerName", creatureEntity.ServerName),
                        new XElement("SmilexamineLastDate",
                            creatureEntity.SmilexamineLastDate?.ToString(CultureInfo.InvariantCulture) ?? string.Empty));
                root.Add(creature);
            }

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                root);
        }

        IEnumerable<XElement> GetTraitListXML(CreatureEntity creatureEntity)
        {
            var result = new List<XElement>();
            foreach (var trait in creatureEntity.Traits)
            {
                result.Add(new XElement("Trait", new XAttribute("TraitId", (int)trait.CreatureTraitId), trait.ToString()));
            }
            return result;
        }

        string GetGender(CreatureEntity creatureEntity)
        {
            if (creatureEntity.IsMale == null) return string.Empty;
            else return creatureEntity.IsMale.Value ? "male" : "female";
        }
    }
}