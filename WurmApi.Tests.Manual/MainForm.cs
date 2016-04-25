using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmApi;
using JetBrains.Annotations;

namespace WindowsFormsApplication3
{
    public partial class MainForm : Form
    {
        IWurmApi wurmApi;

        public MainForm()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            wurmApi = WurmApiFactory.Create(
                new WurmApiCreationOptions()
                {
                    WurmApiLogger = new WumrApiLogger(textBox1),
                    WurmApiEventMarshaller = new Marshaller(this),     
                    WurmApiConfig = new WurmApiConfig()
                    {
                        PersistenceMethod = WurmApiPersistenceMethod.FlatFiles
                    }
                });
        }

        async void button1_Click(object sender, EventArgs e)
        {
            SearchTasker tasker = new SearchTasker(textBox1, wurmApi);
            try
            {
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 100; i++)
                {
                    tasks.Add(tasker.DoAsync());
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception exception)
            {
                textBox1.Text += "Error: " + exception;
            }
        }
    }

    class SearchTasker
    {
        readonly TextBox textBox;
        readonly IWurmApi wurmApi;

        readonly string[] chars = {"Aldur", "Aldurr", "Uldur", "Eldur"};
        readonly string[] skills = {"carpentry", "saw", "animal husbandry", "nature", "masonry", "faith", "favor"};

        public SearchTasker(TextBox textBox, IWurmApi wurmApi)
        {
            this.textBox = textBox;
            this.wurmApi = wurmApi;
        }

        public async Task DoAsync()
        {
            var c = wurmApi.Characters.All.ChooseRandom();

            var args = new LogSearchParameters()
            {
                CharacterName = chars.ChooseRandom(),
                MinDate = new DateTime(2014, 1, 1),
                MaxDate = new DateTime(2016, 1, 1),
                LogType = wurmApi.LogDefinitions.AllLogTypes.ChooseRandom()
            };

            var t1 = wurmApi.LogsHistory.ScanAsync(args);

            var t2 = c.Logs.ScanLogsServerGroupRestrictedAsync(new DateTime(2014, 1, 1),
                        new DateTime(2016, 1, 1),
                        wurmApi.LogDefinitions.AllLogTypes.ChooseRandom(),
                        wurmApi.ServerGroups.AllKnown.ChooseRandom());

            var t3 = c.Skills.TryGetCurrentSkillLevelAsync(skills.ChooseRandom(),
                        wurmApi.ServerGroups.AllKnown.ChooseRandom(),
                        TimeSpan.FromDays(365));

            var r1 = await t1;
            var r2 = await t2;
            var r3 = await t3;

            textBox.Text += "Scan complete, result count: " + r1.Count + "\r\n";
            textBox.Text += "SG-Scan complete, result count: " + r2.Count + "\r\n";
            textBox.Text += "Skill-Scan complete, skill: " + r3?.NameNormalized + " result: " + r3?.Value + "\r\n";
        }

        string Convert(LogSearchParameters args)
        {
            return $"{args.CharacterName}, {args.LogType}";
        }
    }

    public static class QuickExtensions
    {
        static readonly Random Random = new Random();

        public static T ChooseRandom<T>(this IEnumerable<T> enumerable)
        {
            var all = enumerable.ToArray();
            if (!all.Any()) throw new InvalidOperationException("enumerable is empty");
            return all[Random.Next(0, all.Length)];
        }
    }

    class WumrApiLogger : IWurmApiLogger
    {
        readonly TextBox textBox;

        public WumrApiLogger(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public void Log(LogLevel level, string message, object source, Exception exception)
        {
            Action action = () => { textBox.Text += level + " > " + message + "\r\n"; };
            //textBox.BeginInvoke(action);
        }
    }

    class Marshaller : IWurmApiEventMarshaller
    {
        readonly Control control;

        public Marshaller([NotNull] Control control)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            this.control = control;
        }

        public void Marshal(Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
        }
    }
}
