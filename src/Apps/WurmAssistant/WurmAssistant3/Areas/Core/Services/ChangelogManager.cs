using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms.Reusables;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    [KernelHint(BindingHint.Singleton), PersistentObject("ChangelogManager")]
    public class ChangelogManager : PersistentObjectBase, IChangelogManager
    {
        readonly IBinDirectory binDirectory;

        readonly string changelogPath;

        [JsonProperty] DateTimeOffset lastKnownChangeDate;

        public ChangelogManager([NotNull] IBinDirectory binDirectory)
        {
            if (binDirectory == null) throw new ArgumentNullException("binDirectory");
            this.binDirectory = binDirectory;

            changelogPath = Path.Combine(binDirectory.FullPath, "changelog-raw.txt");
        }

        public DateTimeOffset LastKnownChangeDate
        {
            get { return lastKnownChangeDate; }
            set { lastKnownChangeDate = value; FlagAsChanged(); }
        }

        public string GetNewChanges()
        {
            var newChanges = GetNewChangelogEntries(changelogPath);
            StringBuilder sb = new StringBuilder();
            if (newChanges.Any())
            {
                bool firstSection = true;
                string lastAuthor = string.Empty;

                var changesGroupedByDay = newChanges
                    .GroupBy(change => change.Date.Date)
                    .OrderByDescending(changes => changes.Key);
                foreach (var change in changesGroupedByDay)
                {
                    var date = change.Key;
                    var innerChanges = change.OrderBy(c => c.Date);

                    if (!firstSection) sb.AppendLine();
                    else firstSection = false;

                    sb.AppendLine(string.Format((string)"{0}", date.ToString("d")));
                    lastAuthor = string.Empty;

                    foreach (var innerChange in innerChanges)
                    {
                        if (innerChange.Author != lastAuthor)
                        {
                            sb.AppendLine("By " + innerChange.Author + ":");
                            lastAuthor = innerChange.Author;
                        }
                        SplitChangeText(sb, innerChange.ChangeText, "; ");
                    }
                }
            }
            return sb.ToString();
        }

        void SplitChangeText(StringBuilder sb, string changeText, string delimiter)
        {
            changeText = changeText.Trim();
            while (changeText.EndsWith(";"))
            {
                changeText = changeText.Substring(0, changeText.Length - 1);
            }
            var lines = changeText.Split(new[] {delimiter}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                sb.AppendLine("- " + line);
            }
        }

        public void UpdateLastChangeDate()
        {
            var lastChange =
                GetNewChangelogEntries(changelogPath)
                    .OrderByDescending(change => change.Date)
                    .FirstOrDefault();
            if (lastChange != null) LastKnownChangeDate = lastChange.Date;
        }

        public void ShowChanges(string changesText)
        {
            var view = new UniversalTextDisplayView();
            view.Text = "Latest changes!";
            view.ContentText = changesText ?? string.Empty;
            view.Show();
        }

        ICollection<Change> GetNewChangelogEntries(string filePath)
        {
            List<Change> newChanges = new List<Change>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    
                    var parts = line.Split('|');
                    if (parts.Length >= 3)
                    {
                        Change change = new Change()
                        {
                            Date = ParseDate(parts[0]),
                            Author = parts[1],
                            ChangeText =
                                parts.Skip(2)
                                     .Aggregate(new StringBuilder(),
                                         (sb, @string) => sb.Append("|").Append(@string))
                                     .ToString().Remove(0, 1)
                        };
                        if (change.Date > LastKnownChangeDate)
                        {
                            newChanges.Add(change);
                        }
                    }
                }
            }
            return newChanges;
        }

        DateTimeOffset ParseDate(string dateString)
        {
            var date = DateTimeOffset.Parse(dateString, CultureInfo.InvariantCulture);
            return date;
        }

        class Change
        {
            public DateTimeOffset Date { get; set; }
            public string Author { get; set; }
            public string ChangeText { get; set; }
        }
    }
}
