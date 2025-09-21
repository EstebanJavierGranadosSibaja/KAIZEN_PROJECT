namespace KaizenLang.UI.Utils
{
    public class ModernTextBoxBase : TextBox
    {
        public ModernTextBoxBase()
        {
            // Configuración básica
            BorderStyle = BorderStyle.None;
            Dock = DockStyle.Fill;
            Multiline = true;
            WordWrap = true;
            ScrollBars = ScrollBars.Vertical;

            // Optimización del rendimiento para mejor scrolling
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true
            );

            // Configuración de colores - evitar negro sobre negro
            BackColor = Color.FromArgb(30, 30, 30); // Fondo oscuro pero no negro
            ForeColor = Color.FromArgb(220, 220, 220); // Texto claro para contraste

            // Fuente monoespaciada por defecto para código
            Font = new Font("Consolas", 11f);

            // Configuración de edición mejorada
            AcceptsTab = true;
            AcceptsReturn = true;
            HideSelection = false; // Mantener selección visible cuando pierde el foco

            // Sin Padding para evitar problemas de scrolling
            // Los márgenes se manejan en el contenedor padre
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            // Asegurar scroll suave al cursor
            if (this.Focused)
            {
                this.ScrollToCaret();
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            // Scroll al cursor cuando el control recibe el foco
            this.ScrollToCaret();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // Asegurar que la posición del cursor sea visible después de hacer clic
            if (this.Focused)
            {
                this.ScrollToCaret();
            }
        }

        // Método para scroll más suave y controlado
        protected override void WndProc(ref Message m)
        {
            // Interceptar mensajes de scroll para mejorar el comportamiento
            const int WM_VSCROLL = 0x115;
            const int WM_HSCROLL = 0x114;

            if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL)
            {
                base.WndProc(ref m);
                this.Invalidate(); // Redibuja para mantener consistencia visual
                return;
            }

            base.WndProc(ref m);
        }
    }
}