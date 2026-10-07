using System.Drawing;
using System.Windows.Forms;

namespace Vista.Overlay
{
    public partial class Overlay : Form
    {
        public Overlay()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(45, 45, 45);
            this.Opacity = 0.5;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
        }
    }
}