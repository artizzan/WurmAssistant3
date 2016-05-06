using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Granger.Views
{
    public partial class FormChoosePlayers : ExtendedForm
    {
        public string[] Result = new string[0];

        public FormChoosePlayers(string[] currentPlayers, IWurmApi wurmApi)
        {
            InitializeComponent();
            string[] allPlayers = wurmApi.Characters.All.Select(character => character.Name.Capitalized)
                                         .Union(currentPlayers).Distinct().ToArray();
            foreach (var player in allPlayers)
            {
                checkedListBoxPlayers.Items.Add(player, currentPlayers.Contains(player));
            }
            BuildResult();
        }

        void BuildResult()
        {
            List<string> items = new List<string>();
            foreach (var item in checkedListBoxPlayers.CheckedItems)
            {
                items.Add((string)item);
            }
            Result = items.ToArray();
        }

        private void checkedListBoxPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            BuildResult();
        }
    }
}
