using System.Windows.Forms;

namespace KaizenLang.UI.Components
{
    /// <summary>
    /// Panel que contiene un text box
    /// Adapta el panel y el text box a las dimensiones del contenedor padre.
    /// El texto con modo de solo lectura o escritura
    /// </summary>
    public class TextPanel : Panel
    {
        private readonly TextBox textBox;

        public TextPanel()
        {
            textBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ReadOnly = false,
                BorderStyle = BorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(textBox);
        }

        /// <summary>
        /// Obtiene o establece el texto del TextBox.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string TextContent
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        /// <summary>
        /// Obtiene o establece si el TextBox es de solo lectura.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool ReadOnly
        {
            get => textBox.ReadOnly;
            set => textBox.ReadOnly = value;
        }
    }
}
