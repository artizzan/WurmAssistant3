using System.Drawing;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Utils.WinForms
{
    public class ExtendedForm : Form
    {
        /// <summary>
        /// Shows this window centered at parent, additionally fits the window into work area if it's outside bounds
        /// </summary>
        /// <param name="form"></param>
        public void ShowCenteredOnForm(Form form)
        {
            SetCenteredOnParentOnLoadWorkAreaBoundEx(this, form);
            this.Show();
        }

        public DialogResult ShowDialogCenteredOnForm(Form form)
        {
            SetCenteredOnParentOnLoadWorkAreaBoundEx(this, form);
            return this.ShowDialog();
        }

        /// <summary>
        /// Restores shape of the form from saved rectangle, 
        /// additionally fits the window into work area if it's outside bounds or too large
        /// </summary>
        /// <param name="desiredShape"></param>
        public void SetShape(Rectangle desiredShape)
        {
            SetFormShapeWorkAreaBoundEx(this, desiredShape);
        }

        /// <summary>
        /// Returns shape of the form, which is Form.Location and Form.Size when visible and Form.RestoreBounds when hidden / minimized
        /// </summary>
        /// <returns></returns>
        public Rectangle GetShape()
        {
            return GetFormRealBoundsEx(this);
        }

        public void ShowAndBringToFront()
        {
            if (this.Visible)
            {
                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
            else
            {
                this.Show();
                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;
            }
            FitWindowIntoWorkAreaEx(this);
        }

        static Point GetCenteredChildPositionRelativeToParentWorkAreaBoundEx(Form parent, Form child)
        {
            Size newFormSize = child.Size;
            Size parentFormSize = parent.Size;
            Point parentFormLocation = parent.Location;
            Point parentCenter = new Point(parentFormSize.Width / 2, parentFormSize.Height / 2);
            Point parentAdjLocation = new Point(parentFormLocation.X + parentCenter.X, parentFormLocation.Y + parentCenter.Y);

            Point newFormOffset = new Point(newFormSize.Width / 2, newFormSize.Height / 2);
            var newLoc = new Point(parentAdjLocation.X - newFormOffset.X, parentAdjLocation.Y - newFormOffset.Y);

            var workingArea = Screen.FromControl(parent).WorkingArea;

            if (newLoc.X < workingArea.X)
                newLoc.X = workingArea.X;
            else if (newLoc.X + child.Width > workingArea.Right)
                newLoc.X = workingArea.Right - child.Width;

            if (newLoc.Y < workingArea.Y)
                newLoc.Y = workingArea.Y;
            else if (newLoc.Y + child.Height > workingArea.Bottom)
                newLoc.Y = workingArea.Bottom - child.Height;

            return newLoc;
        }

        static void FitWindowIntoWorkAreaEx(Form form)
        {
            var workingArea = Screen.FromControl(form).WorkingArea;

            // make sure form is not bigger than working area
            if (workingArea.Width < form.Size.Width)
                form.Size = new Size(workingArea.Width, form.Size.Height);
            if (workingArea.Height < form.Size.Height)
                form.Size = new Size(form.Size.Width, workingArea.Height);

            Point newLoc = form.Location;

            // make sure it is fully visible in X axis
            if (form.Location.X < workingArea.X)
                newLoc.X = workingArea.X;
            else if (form.Location.X + form.Size.Width > workingArea.Right)
                newLoc.X = workingArea.Right - form.Size.Width;

            // make sure it is fully visible in Y axis
            if (form.Location.Y < workingArea.Y)
                newLoc.Y = workingArea.Y;
            else if (form.Location.Y + form.Size.Height > workingArea.Bottom)
                newLoc.Y = workingArea.Bottom - form.Size.Height;

            form.Location = newLoc;
        }

        static void SetCenteredOnParentOnLoadWorkAreaBoundEx(Form child, Form parent)
        {
            child.Load += (sender, args) => child.Location = GetCenteredChildPositionRelativeToParentWorkAreaBoundEx(parent, child);
        }

        static void SetFormShapeWorkAreaBoundEx(Form form, Rectangle shape)
        {
            form.Location = new Point(shape.X, shape.Y);
            form.Size = new Size(shape.Width, shape.Height);
            FitWindowIntoWorkAreaEx(form);
        }

        static Rectangle GetFormRealBoundsEx(Form form)
        {
            if (form.Visible)
                return new Rectangle(form.Location.X, form.Location.Y, form.Size.Width, form.Size.Height);
            else
                return form.RestoreBounds;
        }

        public void SetupHideInsteadOfCloseIfUserReason()
        {
            FormClosing -= FormCalendar_FormClosing;
            FormClosing += FormCalendar_FormClosing;
        }

        private void FormCalendar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
