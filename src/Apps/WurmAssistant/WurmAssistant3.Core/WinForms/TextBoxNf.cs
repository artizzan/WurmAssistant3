using System.Windows.Forms;

// Original author: http://stackoverflow.com/users/2090/stormenet

namespace AldursLab.WurmAssistant3.Core.WinForms
{
    public class TextBoxNf : System.Windows.Forms.TextBox
    {
        public TextBoxNf()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}