using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.ImportExport.Legacy
{
    public class HerdExporter
    {
        public XDocument CreateXML(GrangerContext context, string herdName)
        {
            if (herdName == null) throw new GrangerException("No herd specified");

            var horses = context.Horses.Where(x => x.Herd == herdName).ToArray();

            if (horses.Length == 0)
            {
                throw new GrangerException(string.Format("No creatures found in {0} herd or herd did not exist", herdName));
            }

            XElement root = new XElement("Herd", new XAttribute("OriginalHerdName", herdName));

            foreach (HorseEntity horseEntity in horses)
            {
                var horse =
                    new XElement("Horse",
                        new XElement("Name", horseEntity.Name),
                        new XElement("Father", horseEntity.FatherName),
                        new XElement("Mother", horseEntity.MotherName),
                        new XElement("Traits", GetTraitListXML(horseEntity)),
                        new XElement("NotInMoodUntil",
                            horseEntity.NotInMood.HasValue
                                ? horseEntity.NotInMood.Value.ToString(CultureInfo.InvariantCulture)
                                : string.Empty),
                        new XElement("PregnantUntil",
                            horseEntity.PregnantUntil.HasValue
                                ? horseEntity.PregnantUntil.Value.ToString(
                                    CultureInfo.InvariantCulture)
                                : string.Empty),
                        new XElement("GroomedOn",
                            horseEntity.GroomedOn.HasValue
                                ? horseEntity.GroomedOn.Value.ToString(CultureInfo.InvariantCulture)
                                : string.Empty),
                        new XElement("Gender", GetGender(horseEntity)),
                        new XElement("CaredBy", horseEntity.TakenCareOfBy),
                        new XElement("InspectSkill", horseEntity.TraitsInspectedAtSkill,
                            new XAttribute("IsEpic", horseEntity.EpicCurve.HasValue ? horseEntity.EpicCurve.ToString() : bool.FalseString)),
                        new XElement("Age", horseEntity.Age),
                        new XElement("Color", horseEntity.Color),
                        new XElement("Comments", horseEntity.Comments),
                        new XElement("Tags", horseEntity.SpecialTagsRaw),
                        new XElement("BrandedFor", horseEntity.BrandedFor));
                root.Add(horse);
            }

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                root);
        }

        IEnumerable<XElement> GetTraitListXML(HorseEntity horseEntity)
        {
            var result = new List<XElement>();
            foreach (var trait in horseEntity.Traits)
            {
                result.Add(new XElement("Trait", new XAttribute("TraitId", (int)trait.Trait), trait.ToString()));
            }
            return result;
        }

        string GetGender(HorseEntity horseEntity)
        {
            if (horseEntity.IsMale == null) return string.Empty;
            else return horseEntity.IsMale.Value ? "male" : "female";
        }
    }
}