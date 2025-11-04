using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ParadigmasLang;

namespace KaizenLang.UI.Theming
{
    /// <summary>
    /// Proporciona syntax highlighting (resaltado de sintaxis) para el lenguaje KaizenLang.
    /// Aplica colores específicos a diferentes elementos del código para mejorar la legibilidad.
    /// </summary>
    public static class SyntaxHighlighter
    {
        // Cache de expresiones regulares para mejor rendimiento
        private static readonly Regex _commentRegex = new Regex(@"//.*", RegexOptions.Multiline);
        private static readonly Regex _stringRegex = new Regex(@"""(?:\\""|[^""])*""", RegexOptions.Multiline);
        private static readonly Regex _numberRegex = new Regex(@"\b\d+\.?\d*\b", RegexOptions.Multiline);
    private static readonly Regex _genericTypeRegex = new Regex(@"\b(chainsaw|hogyoku)\s*<\s*(\w+)\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex _functionRegex = new Regex(@"\b[a-zA-Z_][a-zA-Z0-9_]*\s*(?=\()", RegexOptions.Multiline);

        // Lista de operadores como regex compilado
        private static readonly Regex _operatorRegex = new Regex(
            @"(?<operator>\+\+|--|==|!=|<=|>=|&&|\|\||[+\-*/%=<>!(){}\[\];,.:])",
            RegexOptions.Multiline);

        /// <summary>
        /// Aplica syntax highlighting a un RichTextBox basado en los tokens del lenguaje KaizenLang.
        /// </summary>
        public static void ApplySyntaxHighlighting(RichTextBox richTextBox)
        {
            ApplySyntaxHighlighting(richTextBox, false);
        }

        /// <summary>
        /// Aplica syntax highlighting a un RichTextBox basado en los tokens del lenguaje KaizenLang.
        /// </summary>
        /// <param name="richTextBox">El RichTextBox al que aplicar highlighting</param>
        /// <param name="forceHighlight">Si es true, aplica highlighting aunque el control tenga foco</param>
        public static void ApplySyntaxHighlighting(RichTextBox richTextBox, bool forceHighlight)
        {
            if (richTextBox == null || richTextBox.IsDisposed || !richTextBox.Visible)
                return;

            if (string.IsNullOrEmpty(richTextBox.Text))
                return;

            // Guardar posición del cursor y selección
            var selectionStart = richTextBox.SelectionStart;
            var selectionLength = richTextBox.SelectionLength;
            var scrollPosition = GetScrollPosition(richTextBox);

            try
            {
                // Deshabilitar temporalmente eventos y redraw para evitar parpadeo
                richTextBox.SuspendLayout();

                // Resetear formato a color base
                int originalStart = richTextBox.SelectionStart;
                int originalLength = richTextBox.SelectionLength;

                richTextBox.SelectAll();
                richTextBox.SelectionColor = ThemeManager.CurrentTheme.Foreground;
                richTextBox.SelectionBackColor = ThemeManager.CurrentTheme.Background;
                richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Regular);

                // Restaurar selección antes de aplicar highlighting
                richTextBox.SelectionStart = originalStart;
                richTextBox.SelectionLength = originalLength;

                // Obtener rangos de comentarios una sola vez para reutilizar
                var commentRanges = GetCommentRanges(richTextBox.Text);

                // Aplicar highlighting por categorías en orden de prioridad
                ApplyStringHighlighting(richTextBox, commentRanges);
                ApplyNumberHighlighting(richTextBox, commentRanges);
                ApplyOperatorHighlighting(richTextBox, commentRanges);
                ApplyKeywordHighlighting(richTextBox, commentRanges);
                ApplyTypeHighlighting(richTextBox, commentRanges);
                ApplyFunctionHighlighting(richTextBox, commentRanges);
                // Los comentarios se aplican al final para máxima prioridad
                ApplyCommentHighlighting(richTextBox);

                // Restaurar posición del cursor y selección
                richTextBox.SelectionStart = selectionStart;
                richTextBox.SelectionLength = selectionLength;
                SetScrollPosition(richTextBox, scrollPosition);
            }
            catch (Exception ex)
            {
                // Log del error (en producción podrías usar un logger)
                System.Diagnostics.Debug.WriteLine($"Error en syntax highlighting: {ex.Message}");
            }
            finally
            {
                // Asegurarse de restaurar el estado del control
                try
                {
                    richTextBox.ResumeLayout();
                }
                catch
                {
                    // Ignorar errores al restaurar el estado
                }
            }
        }

        #region Highlighting Methods

        /// <summary>
        /// Obtiene los rangos de todos los comentarios en el texto para excluirlos del procesamiento de otros elementos
        /// </summary>
        private static List<(int start, int end)> GetCommentRanges(string text)
        {
            var ranges = new List<(int start, int end)>();

            foreach (Match match in _commentRegex.Matches(text))
            {
                ranges.Add((match.Index, match.Index + match.Length));
            }

            return ranges;
        }

        /// <summary>
        /// Verifica si una posición está dentro de un comentario
        /// </summary>
        private static bool IsInComment(int position, List<(int start, int end)> commentRanges)
        {
            foreach (var (start, end) in commentRanges)
            {
                if (position >= start && position < end)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica si un rango completo está dentro de un comentario
        /// </summary>
        private static bool IsRangeInComment(int start, int length, List<(int start, int end)> commentRanges)
        {
            int end = start + length;
            foreach (var (commentStart, commentEnd) in commentRanges)
            {
                if (start >= commentStart && end <= commentEnd)
                    return true;
            }
            return false;
        }

        private static void ApplyCommentHighlighting(RichTextBox richTextBox)
        {
            var theme = ThemeManager.CurrentTheme;

            foreach (Match match in _commentRegex.Matches(richTextBox.Text))
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = theme.Comments;
                richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Italic);
            }
        }

        private static void ApplyStringHighlighting(RichTextBox richTextBox, List<(int start, int end)> commentRanges)
        {
            var theme = ThemeManager.CurrentTheme;

            foreach (Match match in _stringRegex.Matches(richTextBox.Text))
            {
                if (!IsRangeInComment(match.Index, match.Length, commentRanges))
                {
                    richTextBox.Select(match.Index, match.Length);
                    richTextBox.SelectionColor = theme.Strings;
                    richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Regular);
                }
            }
        }

        private static void ApplyNumberHighlighting(RichTextBox richTextBox, List<(int start, int end)> commentRanges)
        {
            var theme = ThemeManager.CurrentTheme;

            foreach (Match match in _numberRegex.Matches(richTextBox.Text))
            {
                if (!IsRangeInComment(match.Index, match.Length, commentRanges))
                {
                    richTextBox.Select(match.Index, match.Length);
                    richTextBox.SelectionColor = theme.Numbers;
                    richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Regular);
                }
            }
        }

        private static void ApplyKeywordHighlighting(RichTextBox richTextBox, List<(int start, int end)> commentRanges)
        {
            var theme = ThemeManager.CurrentTheme;
            var keywords = new[]
            {
                // Palabras clave de control
                ReservedWords.IF, ReservedWords.ELSE, ReservedWords.WHILE, ReservedWords.FOR, ReservedWords.DO, ReservedWords.RETURN,
                // Valores literales
                LiteralWords.TRUE, LiteralWords.FALSE,
                // Funciones builtin
                ReservedWords.INPUT, ReservedWords.OUTPUT, "length",
                // Bloques KaizenLang específicos
                DelimiterWords.BLOCK_START, DelimiterWords.BLOCK_END
            };

            foreach (var keyword in keywords)
            {
                var keywordRegex = new Regex($@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase);
                foreach (Match match in keywordRegex.Matches(richTextBox.Text))
                {
                    if (!IsRangeInComment(match.Index, match.Length, commentRanges))
                    {
                        richTextBox.Select(match.Index, match.Length);
                        richTextBox.SelectionColor = theme.Keywords;
                        richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Bold);
                    }
                }
            }
        }

        private static void ApplyTypeHighlighting(RichTextBox richTextBox, List<(int start, int end)> commentRanges)
        {
            var theme = ThemeManager.CurrentTheme;
            var types = new[]
            {
                // Tipos básicos de KaizenLang
                TypeWords.GEAR, TypeWords.GRIMOIRE, TypeWords.SHIN, TypeWords.SHINKAI, TypeWords.BANKAI, "char", ReservedWords.VOID,
                // Tipos compuestos
                TypeWords.CHAINSAW, TypeWords.HOGYOKU
            };

            foreach (var type in types)
            {
                var typeRegex = new Regex($@"\b{Regex.Escape(type)}\b", RegexOptions.IgnoreCase);
                foreach (Match match in typeRegex.Matches(richTextBox.Text))
                {
                    if (!IsRangeInComment(match.Index, match.Length, commentRanges))
                    {
                        richTextBox.Select(match.Index, match.Length);
                        richTextBox.SelectionColor = theme.Functions;
                        richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Bold);
                    }
                }
            }

            // Tipos genéricos
            foreach (Match match in _genericTypeRegex.Matches(richTextBox.Text))
            {
                if (!IsRangeInComment(match.Index, match.Length, commentRanges))
                {
                    richTextBox.Select(match.Index, match.Length);
                    richTextBox.SelectionColor = theme.Functions;
                    richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Bold);
                }
            }
        }

        private static void ApplyOperatorHighlighting(RichTextBox richTextBox, List<(int start, int end)> commentRanges)
        {
            var theme = ThemeManager.CurrentTheme;

            foreach (Match match in _operatorRegex.Matches(richTextBox.Text))
            {
                var operatorGroup = match.Groups["operator"];
                if (operatorGroup.Success && !IsRangeInComment(operatorGroup.Index, operatorGroup.Length, commentRanges))
                {
                    richTextBox.Select(operatorGroup.Index, operatorGroup.Length);
                    richTextBox.SelectionColor = theme.Operators;
                    richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Regular);
                }
            }
        }

        private static void ApplyFunctionHighlighting(RichTextBox richTextBox, List<(int start, int end)> commentRanges)
        {
            var theme = ThemeManager.CurrentTheme;

            foreach (Match match in _functionRegex.Matches(richTextBox.Text))
            {
                var funcName = match.Value.Trim();
                if (!IsRangeInComment(match.Index, funcName.Length, commentRanges))
                {
                    // Verificar que no sea una palabra clave
                    var keywords = new HashSet<string>
                    {
                        ReservedWords.IF,
                        ReservedWords.ELSE,
                        ReservedWords.WHILE,
                        ReservedWords.FOR,
                        ReservedWords.DO,
                        ReservedWords.RETURN,
                        LiteralWords.TRUE,
                        LiteralWords.FALSE,
                        ReservedWords.INPUT,
                        ReservedWords.OUTPUT,
                        "length",
                        DelimiterWords.BLOCK_START,
                        DelimiterWords.BLOCK_END
                    };
                    if (!keywords.Contains(funcName.ToLower()))
                    {
                        richTextBox.Select(match.Index, funcName.Length);
                        richTextBox.SelectionColor = theme.Functions;
                        richTextBox.SelectionFont = new Font("Consolas", 12F, FontStyle.Regular);
                    }
                }
            }
        }

        #endregion

        #region Scroll Position Helpers

        private static Point GetScrollPosition(RichTextBox richTextBox)
        {
            try
            {
                return new Point(
                    NativeMethods.GetScrollPos(richTextBox.Handle, 0), // SB_HORZ
                    NativeMethods.GetScrollPos(richTextBox.Handle, 1)  // SB_VERT
                );
            }
            catch
            {
                return Point.Empty;
            }
        }

        private static void SetScrollPosition(RichTextBox richTextBox, Point position)
        {
            try
            {
                NativeMethods.SetScrollPos(richTextBox.Handle, 0, position.X, true); // SB_HORZ
                NativeMethods.SetScrollPos(richTextBox.Handle, 1, position.Y, true); // SB_VERT
                NativeMethods.SendMessage(richTextBox.Handle, NativeMethods.WM_HSCROLL, NativeMethods.SB_THUMBPOSITION + 0x10000 * position.X, 0);
                NativeMethods.SendMessage(richTextBox.Handle, NativeMethods.WM_VSCROLL, NativeMethods.SB_THUMBPOSITION + 0x10000 * position.Y, 0);
            }
            catch
            {
                // Ignorar errores de scroll
            }
        }

        #endregion
    }

    /// <summary>
    /// Métodos nativos para manejo del scroll
    /// </summary>
    internal static class NativeMethods
    {
        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;
        public const int SB_THUMBPOSITION = 4;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
    }
}
