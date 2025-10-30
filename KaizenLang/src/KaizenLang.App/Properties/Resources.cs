using System.Drawing;
using System.IO;

namespace KaizenLang.App.Properties;

public static class Resources
{
    public static Icon AppIcon
    {
        get
        {
            var iconPath = Path.Combine("Resources", "icon.ico");
            if (File.Exists(iconPath))
            {
                using var stream = File.OpenRead(iconPath);
                return new Icon(stream);
            }

            return SystemIcons.Application;
        }
    }
}
