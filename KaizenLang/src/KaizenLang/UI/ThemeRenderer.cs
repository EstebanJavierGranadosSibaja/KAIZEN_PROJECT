using System.Drawing;
using System.Windows.Forms;

namespace KaizenLang.UI.ThemeRenderer
{
    // A small helper to provide a color table for ToolStrip/menus matching UIConstants
    public class ThemeColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelectedGradientBegin => UIConstants.Colors.MenuBackground;
        public override Color MenuItemSelectedGradientEnd => UIConstants.Colors.MenuBackground;
        public override Color MenuItemPressedGradientBegin => UIConstants.Colors.PanelBackground;
        public override Color MenuItemPressedGradientEnd => UIConstants.Colors.PanelBackground;
        public override Color ToolStripDropDownBackground => UIConstants.Colors.MenuBackground;
        public override Color ImageMarginGradientBegin => UIConstants.Colors.MenuBackground;
        public override Color ImageMarginGradientEnd => UIConstants.Colors.MenuBackground;
        public override Color MenuItemSelected => UIConstants.Colors.MenuForeground;
        public override Color MenuItemPressedGradientMiddle => UIConstants.Colors.MenuForeground;
    }
}
