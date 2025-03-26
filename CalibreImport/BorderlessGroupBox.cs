using System;
using System.Drawing;
using System.Windows.Forms;

namespace CalibreImport
{
    // This control was created to have a groupbox without the border.
    // De gustibus non est disputandum.
    public class BorderlessGroupBox : GroupBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the background and text, but skip the border
            e.Graphics.Clear(this.BackColor);
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, new Point(0, 0), this.ForeColor);
        }
    }
}
