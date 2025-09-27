namespace KaizenLang.UI.Theme
{
    public static class ThemeManager
    {
        public static Theme CurrentTheme { get; private set; } = new Theme();

        // Evento que se dispara cuando cambia el tema
        public static event EventHandler? ThemeChanged;

        public static void SetTheme(Theme theme)
        {
            CurrentTheme = theme;
            OnThemeChanged();
        }

        private static void OnThemeChanged()
        {
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Aplica el tema a todos los controles de un formulario de manera recursiva
        /// </summary>
        public static void ApplyThemeToAllControls(Control parent)
        {
            if (parent == null) return;

            // Aplicar tema al control padre
            ApplyThemeToControl(parent);

            // Aplicar tema a todos los controles hijos
            foreach (Control child in parent.Controls)
            {
                ApplyThemeToAllControls(child);
            }
        }

        /// <summary>
        /// Aplica el tema a un control específico
        /// </summary>
        public static void ApplyThemeToControl(Control control)
        {
            if (control == null) return;

            // Aplicar usando el método del tema
            CurrentTheme.ApplyTo(control);

            // Aplicaciones específicas para controles personalizados
            if (control is Form form)
            {
                ApplyThemeToForm(form);
            }
            else if (control is MenuStrip menuStrip)
            {
                ApplyThemeToMenuStrip(menuStrip);
            }
            else if (control is ToolStrip toolStrip)
            {
                ApplyThemeToToolStrip(toolStrip);
            }
        }

        /// <summary>
        /// Aplica tema específico a formularios
        /// </summary>
        private static void ApplyThemeToForm(Form form)
        {
            form.BackColor = CurrentTheme.Background;
            form.ForeColor = CurrentTheme.Foreground;
        }

        /// <summary>
        /// Aplica tema específico a MenuStrip
        /// </summary>
        private static void ApplyThemeToMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.BackColor = CurrentTheme.MenuBackground;
            menuStrip.ForeColor = CurrentTheme.MenuForeground;
            menuStrip.Renderer = new ModernMenuStripRenderer();

            foreach (ToolStripItem item in menuStrip.Items)
            {
                ApplyThemeToToolStripItem(item);
            }
        }

        /// <summary>
        /// Aplica tema específico a ToolStrip
        /// </summary>
        private static void ApplyThemeToToolStrip(ToolStrip toolStrip)
        {
            toolStrip.BackColor = CurrentTheme.ToolStripBackground;
            toolStrip.ForeColor = CurrentTheme.ToolStripForeground;
            toolStrip.Renderer = new ModernToolStripRenderer();

            foreach (ToolStripItem item in toolStrip.Items)
            {
                ApplyThemeToToolStripItem(item);
            }
        }

        /// <summary>
        /// Aplica tema a elementos de ToolStrip
        /// </summary>
        private static void ApplyThemeToToolStripItem(ToolStripItem item)
        {
            item.BackColor = CurrentTheme.MenuBackground;
            item.ForeColor = CurrentTheme.MenuForeground;

            if (item is ToolStripMenuItem menuItem)
            {
                foreach (ToolStripItem subItem in menuItem.DropDownItems)
                {
                    ApplyThemeToToolStripItem(subItem);
                }
            }
        }

        /// <summary>
        /// Suscribe un control para recibir actualizaciones automáticas de tema
        /// </summary>
        public static void SubscribeToThemeChanges(Control control)
        {
            ThemeChanged += (sender, e) => ApplyThemeToAllControls(control);
        }

        /// <summary>
        /// Carga un tema desde archivo JSON
        /// </summary>
        public static void LoadThemeFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    // Aquí podrías implementar la deserialización JSON del tema
                    // Por ahora mantenemos el tema por defecto
                    OnThemeChanged();
                }
            }
            catch (Exception ex)
            {
                // Log del error si es necesario
                Console.WriteLine($"Error cargando tema: {ex.Message}");
            }
        }

        /// <summary>
        /// Guarda el tema actual en archivo JSON
        /// </summary>
        public static void SaveThemeToFile(string filePath)
        {
            try
            {
                // Aquí podrías implementar la serialización JSON del tema
                // Por ahora es un placeholder
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando tema: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Renderer personalizado para MenuStrip
    /// </summary>
    public class ModernMenuStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(
                    new SolidBrush(ThemeManager.CurrentTheme.Selection),
                    e.Item.ContentRectangle);
            }
            else
            {
                e.Graphics.FillRectangle(
                    new SolidBrush(ThemeManager.CurrentTheme.MenuBackground),
                    e.Item.ContentRectangle);
            }
        }
    }

    /// <summary>
    /// Renderer personalizado para ToolStrip
    /// </summary>
    public class ModernToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(
                    new SolidBrush(ThemeManager.CurrentTheme.ButtonMouseOver),
                    e.Item.ContentRectangle);
            }
            else if (e.Item.Pressed)
            {
                e.Graphics.FillRectangle(
                    new SolidBrush(ThemeManager.CurrentTheme.ButtonMouseDown),
                    e.Item.ContentRectangle);
            }
        }
    }
}