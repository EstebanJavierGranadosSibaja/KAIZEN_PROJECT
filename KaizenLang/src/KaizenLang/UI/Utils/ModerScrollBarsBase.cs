// namespace KaizenLang.UI.Utils
// {
//     public class ModernScrollBar : VScrollBar
//     {
//         // Colores modernos inspirados en Windows 11
//         private Color _thumbColor = Color.FromArgb(120, 120, 120);
//         private Color _thumbHoverColor = Color.FromArgb(90, 90, 90);
//         private Color _trackColor = Color.FromArgb(240, 240, 240);

//         private bool _hovering = false;

//         public ModernScrollBar()
//         {
//             SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
//             this.Width = 12;
//         }

//         protected override void OnMouseEnter(EventArgs e)
//         {
//             base.OnMouseEnter(e);
//             _hovering = true;
//             Invalidate();
//         }

//         protected override void OnMouseLeave(EventArgs e)
//         {
//             base.OnMouseLeave(e);
//             _hovering = false;
//             Invalidate();
//         }

//         protected override void OnPaint(PaintEventArgs e)
//         {
//             // Fondo (track)
//             using (SolidBrush trackBrush = new SolidBrush(_trackColor))
//                 e.Graphics.FillRectangle(trackBrush, this.ClientRectangle);

//             // Thumb (deslizador)
//             Rectangle thumbRect = GetThumbRectangle();
//             Color thumbColor = _hovering ? _thumbHoverColor : _thumbColor;
//             using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
//                 e.Graphics.FillRoundedRectangle(thumbBrush, thumbRect, 6);

//             // Opcional: dibujar borde
//             using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 200)))
//                 e.Graphics.DrawRoundedRectangle(borderPen, thumbRect, 6);
//         }

//         // Calcula la posición y tamaño del thumb
//         private Rectangle GetThumbRectangle()
//         {
//             int trackHeight = this.Height;
//             int thumbHeight = Math.Max(30, (int)((float)LargeChange / (Maximum - Minimum + LargeChange) * trackHeight));
//             int thumbTop = (int)((float)(Value - Minimum) / (Maximum - Minimum) * (trackHeight - thumbHeight));
//             return new Rectangle(2, thumbTop, this.Width - 4, thumbHeight);
//         }

//         // Permite gestos táctiles (scroll con dos dedos)
//         protected override void WndProc(ref Message m)
//         {
//             const int WM_POINTERDOWN = 0x0246;
//             const int WM_POINTERUP = 0x0247;
//             const int WM_POINTERUPDATE = 0x0245;
//             const int WM_GESTURE = 0x0119;

//             base.WndProc(ref m);

//             // Permite gestos táctiles nativos
//             if (m.Msg == WM_POINTERDOWN || m.Msg == WM_POINTERUP || m.Msg == WM_POINTERUPDATE || m.Msg == WM_GESTURE)
//             {
//                 // No bloquear gestos
//             }
//         }
//     }

//     // Métodos de extensión para dibujar rectángulos redondeados
//     public static class GraphicsExtensions
//     {
//         public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
//         {
//             using (var path = RoundedRect(bounds, radius))
//                 g.FillPath(brush, path);
//         }

//         public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle bounds, int radius)
//         {
//             using (var path = RoundedRect(bounds, radius))
//                 g.DrawPath(pen, path);
//         }

//         private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
//         {
//             var path = new System.Drawing.Drawing2D.GraphicsPath();
//             int d = radius * 2;
//             path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
//             path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
//             path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
//             path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
//             path.CloseFigure();
//             return path;
//         }
//     }
// }