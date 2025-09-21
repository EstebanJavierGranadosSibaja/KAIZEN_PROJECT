using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KaizenLang.UI.Utils
{
    public class ModernScrollBar : VScrollBar
    {
        private Color thumbColor = Color.FromArgb(121, 121, 121);        // Color normal del "thumb" (similar a VS Code)
        private Color thumbHoverColor = Color.FromArgb(166, 166, 166);   // Color al pasar el mouse
        private Color thumbPressedColor = Color.FromArgb(96, 96, 96);    // Color al presionar
        private Color trackColor = Color.FromArgb(30, 30, 30);           // Color del fondo (similar a VS Code dark theme)
        private int thumbBorderRadius = 4;                               // Radio de borde del thumb

        private bool isThumbHovered = false;
        private bool isThumbPressed = false;

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ThumbColor
        {
            get => thumbColor;
            set { thumbColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ThumbHoverColor
        {
            get => thumbHoverColor;
            set { thumbHoverColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ThumbPressedColor
        {
            get => thumbPressedColor;
            set { thumbPressedColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TrackColor
        {
            get => trackColor;
            set { trackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ThumbBorderRadius
        {
            get => thumbBorderRadius;
            set { thumbBorderRadius = Math.Max(0, value); Invalidate(); }
        }

        public ModernScrollBar()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dibuja el fondo (track)
            using (var brush = new SolidBrush(trackColor))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            // Calcula la posición y tamaño del thumb
            Rectangle thumbRect = GetThumbRect();

            if (thumbRect.Height > 0)
            {
                // Determina el color basado en el estado
                Color currentThumbColor = thumbColor;
                if (isThumbPressed)
                    currentThumbColor = thumbPressedColor;
                else if (isThumbHovered)
                    currentThumbColor = thumbHoverColor;

                // Dibuja el thumb con esquinas redondeadas
                using (var path = RoundedRect(thumbRect, thumbBorderRadius))
                using (var brush = new SolidBrush(currentThumbColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private Rectangle GetThumbRect()
        {
            // Calcula el tamaño y posición del thumb basado en el rango y el valor
            int range = Maximum - Minimum;
            if (range == 0) return Rectangle.Empty;

            int thumbHeight = Math.Max((int)((float)Height / (range + LargeChange) * LargeChange), 20);
            int thumbPosition = (range == 0) ? 0 : (int)((float)(Value - Minimum) / range * (Height - thumbHeight));

            return new Rectangle(2, thumbPosition, Width - 4, thumbHeight);
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Rectangle thumbRect = GetThumbRect();

            if (thumbRect.Contains(e.Location))
            {
                isThumbPressed = true;
                Invalidate();
            }
            else
            {
                // Clic fuera del thumb (en el track)
                int newValue = 0;
                if (e.Y < thumbRect.Y)
                    newValue = Value - LargeChange; // Clic arriba del thumb
                else
                    newValue = Value + LargeChange; // Clic debajo del thumb

                Value = Math.Max(Minimum, Math.Min(Maximum, newValue));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isThumbPressed = false;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Rectangle thumbRect = GetThumbRect();
            bool wasHovered = isThumbHovered;

            isThumbHovered = thumbRect.Contains(e.Location);

            if (wasHovered != isThumbHovered)
                Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isThumbHovered = false;
            Invalidate();
        }
    }
}
