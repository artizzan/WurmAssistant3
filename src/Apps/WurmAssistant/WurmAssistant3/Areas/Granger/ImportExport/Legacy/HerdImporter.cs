using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;

namespace AldursLab.WurmAssistant3.Areas.Granger.ImportExport.Legacy
{
    public class HerdImporter
    {
        public void ImportHerd(GrangerContext context, string newHerdName, string xmlFilePath)
        {
            if (newHerdName == null || newHerdName.Trim() == string.Empty)
            {
                throw new GrangerException("new herd name cannot be empty");
            }

            // check if this herd already exists in database
            if (context.Herds.Any(x => x.HerdId == newHerdName))
            {
                throw new GrangerException($"there is already a herd with named {newHerdName} in database");
            }

            XDocument doc = XDocument.Load(xmlFilePath);
            var creatureEntities = new List<CreatureEntity>();       
            var elements = doc.Root.Elements("Horse").Union(doc.Root.Elements("Creature"));
            foreach (var x in elements)
            {
                var entity = new CreatureEntity();
                entity.Herd = newHerdName;
                entity.Name = x.Element("Name").Value;

                if (creatureEntities.Any(y => y.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new GrangerException(
                        $"Creature named {entity.Name} was already added from this XML file. Review the file for any errors.");
                }

                entity.FatherName = x.Element("Father").Value;
                entity.MotherName = x.Element("Mother").Value;
                entity.Traits = GetTraitsFromXML(x.Element("Traits"));

                var notInMood = x.Element("NotInMoodUntil").Value;
                if (string.IsNullOrEmpty(notInMood))
                {
                    entity.NotInMood = null;
                }
                else
                {
                    entity.NotInMood = DateTime.Parse(notInMood, CultureInfo.InvariantCulture);
                }

                var pregnantUntil = x.Element("PregnantUntil").Value;
                if (string.IsNullOrEmpty(pregnantUntil))
                {
                    entity.PregnantUntil = null;
                }
                else
                {
                    entity.PregnantUntil = DateTime.Parse(pregnantUntil, CultureInfo.InvariantCulture);
                }

                var groomedOn = x.Element("GroomedOn").Value;
                if (string.IsNullOrEmpty(groomedOn)) entity.GroomedOn = null;
                else entity.GroomedOn = DateTime.Parse(groomedOn, CultureInfo.InvariantCulture);

                var gender = x.Element("Gender").Value;
                if (string.IsNullOrEmpty(gender)) entity.IsMale = null;
                else entity.IsMale = gender.Equals("male", StringComparison.InvariantCultureIgnoreCase);

                entity.TakenCareOfBy = x.Element("CaredBy").Value;

                var xInspect = x.Element("InspectSkill");
                if (string.IsNullOrEmpty(xInspect.Value)) entity.TraitsInspectedAtSkill = null;
                else entity.TraitsInspectedAtSkill = float.Parse(xInspect.Value, CultureInfo.InvariantCulture);
                var xInspectAttr = xInspect.Attribute("IsEpic");
                if (string.IsNullOrEmpty(xInspectAttr.Value)) entity.EpicCurve = null;
                else entity.EpicCurve = bool.Parse(xInspectAttr.Value);

                entity.Age = CreatureAge.CreateAgeFromEnumString(x.Element("Age").Value);
                entity.Color = CreatureColor.CreateColorFromEnumString(x.Element("Color").Value);
                entity.Comments = x.Element("Comments").Value;
                entity.SpecialTagsRaw = x.Element("Tags").Value;
                entity.BrandedFor = x.Element("BrandedFor").Value;
                var smilexaminedElement = x.Element("SmilexamineLastDate");
                if (smilexaminedElement != null)
                {
                    entity.SmilexamineLastDate = DateTime.Parse(smilexaminedElement.Value, CultureInfo.InvariantCulture);
                }

                var serverNameElem = x.Element("ServerName");
                entity.ServerName = serverNameElem?.Value;

                creatureEntities.Add(entity);
            }

            context.InsertHerd(newHerdName);
            foreach (var creatureEntity in creatureEntities)
            {
                creatureEntity.Id = CreatureEntity.GenerateNewCreatureId(context);
                context.InsertCreature(creatureEntity);
            }
        }

        List<CreatureTrait> GetTraitsFromXML(XElement xTraits)
        {
            var result = new List<CreatureTrait>();
            foreach (var xTrait in xTraits.Elements("Trait"))
            {
                result.Add(CreatureTrait.FromEnumIntStr(xTrait.Attribute("TraitId").Value));
            }
            return result;
        }
    }
}
