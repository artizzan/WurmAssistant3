using System;
using System.Collections.Generic;
using AldursLab.Persistence;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    [KernelBind(BindingHint.Transient)]
    public class Importer
    {
        readonly IFileDialogs fileDialogs;
        readonly ISerializer serializer;
        readonly ILogger logger;

        public Importer([NotNull] IFileDialogs fileDialogs, [NotNull] ISerializer serializer, [NotNull] ILogger logger)
        {
            if (fileDialogs == null) throw new ArgumentNullException(nameof(fileDialogs));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.fileDialogs = fileDialogs;
            this.serializer = serializer;
            this.logger = logger;
        }

        public void Import(TriggerManager triggerManager)
        {
            var serialized = fileDialogs.TryChooseAndReadTextFile();
            if (serialized != null)
            {
                var deserializedTriggers = serializer.Deserialize<TriggersDto>(serialized);
                ConflictResolution? globalConflictResolutionChoice = null;
                foreach (var triggerEntity in deserializedTriggers.TriggerEntities)
                {
                    var existingTrigger = triggerManager.FindTriggerByName(triggerEntity.Name);
                    if (existingTrigger != null)
                    {
                        if (globalConflictResolutionChoice != null)
                        {
                            ResolveUsing(globalConflictResolutionChoice.Value, triggerManager, triggerEntity, existingTrigger);
                            continue;
                        }

                        var message = "Imported trigger seems to already exists:\r\n" +
                                      "\r\n" +
                                      $"Imported trigger: {triggerEntity.GetDescription()}\r\n" +
                                      $"Existing trigger: {existingTrigger.GetDescription()}\r\n" +
                                      "\r\n" +
                                      "What should we do?";
                        var userChoice = new ConflictResolveDialog();
                        userChoice.SetText(message);
                        userChoice.ShowDialog();

                        ResolveUsing(userChoice.ConflictResolution, triggerManager, triggerEntity, existingTrigger);

                        if (userChoice.UseThisResolutionForAllConflicts)
                        {
                            globalConflictResolutionChoice = userChoice.ConflictResolution;
                        }
                    }
                    else
                    {
                        ResolveUsing(ConflictResolution.ImportAsNew, triggerManager, triggerEntity, null);
                    }
                }
            }
        }

        void ResolveUsing(
            ConflictResolution conflictResolution,
            TriggerManager triggerManager,
            TriggerEntity newTriggerEntity,
            [CanBeNull] ITrigger existingTrigger)
        {
            if (conflictResolution == ConflictResolution.Skip)
            {
                return;
            }
            else if (conflictResolution == ConflictResolution.Replace)
            {
                if (existingTrigger == null) throw new InvalidOperationException("existingTrigger is null");

                var existingTriggerEntity = existingTrigger.GetTriggerEntityCopy(serializer);
                triggerManager.RemoveTrigger(existingTrigger);
                try
                {
                    triggerManager.CreateTriggerFromEntity(newTriggerEntity);
                }
                catch (Exception exception)
                {
                    logger.Error(exception, $"Error at trigger creation attempt (replace): {newTriggerEntity.GetDescription()}");

                    try
                    {
                        triggerManager.CreateTriggerFromEntity(existingTriggerEntity);
                    }
                    catch (Exception innerException)
                    {
                        logger.Error(innerException, $"Error at trigger recreate attempt: {existingTriggerEntity.GetDescription()}");
                    }
                    
                    throw;
                }
            }
            else if (conflictResolution == ConflictResolution.ImportAsNew)
            {
                newTriggerEntity.TriggerId = Guid.NewGuid();
                if (existingTrigger != null && newTriggerEntity.Name == existingTrigger.Name)
                {
                    newTriggerEntity.Name = newTriggerEntity.Name + GetStampText();
                }
                try
                {
                    triggerManager.CreateTriggerFromEntity(newTriggerEntity);
                }
                catch (Exception exception)
                {
                    logger.Error(exception, $"Error at trigger creation attempt (import as new): {newTriggerEntity.GetDescription()}");

                    throw;
                }
            }
        }

        string GetStampText()
        {
            return " " + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}