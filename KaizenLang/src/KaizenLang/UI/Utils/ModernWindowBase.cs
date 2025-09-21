using System.Runtime.InteropServices;
using KaizenLang.UI.Theme;

namespace KaizenLang.UI.Components
{
    public partial class ModernWindowBase : Form
    {
        // Windows 11 DWM Constants
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_CORNER_PREFERENCE = 33;

        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected virtual bool AutoApplyTheme { get; set; } = true;
        protected virtual bool UseModernStyling { get; set; } = true;

        public ModernWindowBase()
        {
            InitializeBaseForm();

            this.HandleCreated += OnHandleCreated;
            this.Load += OnFormLoad;
        }

        private void InitializeBaseForm()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Icon = Properties.Resources.AppIcon;
        }

        private void OnHandleCreated(object? sender, EventArgs e)
        {
            if (UseModernStyling)
            {
                ApplyModernStyling();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (AutoApplyTheme)
            {
                ApplyTheme();
            }
        }

        private void OnFormLoad(object? sender, EventArgs e)
        {
            if (AutoApplyTheme)
            {
                ApplyTheme();
            }
        }

        protected virtual void ApplyModernStyling()
        {
            if (Environment.OSVersion.Version.Major >= 10 &&
                Environment.OSVersion.Version.Build >= 22000) // Windows 11
            {
                // Habilitar modo oscuro
                EnableDarkMode(true);

                // Personalizar colores basados en el tema actual
                var theme = GetCurrentTheme();
                SetTitleBarColor(theme.SecondaryBackground);
                SetTitleTextColor(theme.Foreground);
                SetBorderColor(theme.Border);

                // Esquinas redondeadas (característica de Windows 11)
                SetCornerPreference(DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
            }
        }

        protected virtual void ApplyTheme()
        {
            var theme = GetCurrentTheme();
            ThemeManager.ApplyThemeToAllControls(this);

            // Reaplicar estilos modernos si están habilitados
            if (UseModernStyling && this.IsHandleCreated)
            {
                ApplyModernStyling();
            }
        }

        protected virtual KaizenLang.UI.Theme.Theme GetCurrentTheme()
        {
            // Por defecto retorna una nueva instancia del tema
            // Las clases derivadas pueden sobrescribir esto para usar su propio tema
            return new KaizenLang.UI.Theme.Theme();
        }

        public void EnableDarkMode(bool enable)
        {
            if (!this.IsHandleCreated) return;

            int value = enable ? 1 : 0;
            DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
        }

        public void SetTitleBarColor(Color color)
        {
            if (!this.IsHandleCreated) return;

            int colorValue = ColorTranslator.ToWin32(color);
            DwmSetWindowAttribute(this.Handle, DWMWA_CAPTION_COLOR, ref colorValue, sizeof(int));
        }

        public void SetTitleTextColor(Color color)
        {
            if (!this.IsHandleCreated) return;

            int colorValue = ColorTranslator.ToWin32(color);
            DwmSetWindowAttribute(this.Handle, DWMWA_TEXT_COLOR, ref colorValue, sizeof(int));
        }

        public void SetBorderColor(Color color)
        {
            if (!this.IsHandleCreated) return;

            int colorValue = ColorTranslator.ToWin32(color);
            DwmSetWindowAttribute(this.Handle, DWMWA_BORDER_COLOR, ref colorValue, sizeof(int));
        }

        public void SetCornerPreference(DWM_WINDOW_CORNER_PREFERENCE preference)
        {
            if (!this.IsHandleCreated) return;

            int value = (int)preference;
            DwmSetWindowAttribute(this.Handle, DWMWA_CORNER_PREFERENCE, ref value, sizeof(int));
        }

        /// <summary>
        /// Método que pueden sobrescribir las clases derivadas para inicializar controles específicos
        /// </summary>
        protected virtual void InitializeCustomControls()
        {
            // Implementación específica en clases derivadas
        }

        /// <summary>
        /// Método que pueden sobrescribir las clases derivadas para configurar eventos específicos
        /// </summary>
        protected virtual void InitializeCustomEvents()
        {
            // Implementación específica en clases derivadas
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.HandleCreated -= OnHandleCreated;
                this.Load -= OnFormLoad;
            }
            base.Dispose(disposing);
        }
    }
}