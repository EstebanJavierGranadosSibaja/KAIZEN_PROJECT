namespace KaizenLang.UI.Components
{
    public class MenuBar : MenuStrip
    {
        public MenuBar()
        {
            InsertButtons();
        }

        private void InsertButtons()
        {
            // * Archivo Menu *
            var archivoMenu = new ToolStripMenuItem("Archivo");

            var nuevoItem = new ToolStripMenuItem("Nuevo", null, OnNuevoClick);
            var abrirItem = new ToolStripMenuItem("Abrir", null, OnAbrirClick);
            var guardarItem = new ToolStripMenuItem("Guardar", null, OnGuardarClick);

            archivoMenu.DropDownItems.Add(nuevoItem);
            archivoMenu.DropDownItems.Add(abrirItem);
            archivoMenu.DropDownItems.Add(guardarItem);

            // * Compilar y Ejecutar Buttons *
            var compileButton = new ToolStripButton("Compilar", null, CompileButton_Click);
            var executeButton = new ToolStripButton("Ejecutar", null, ExecuteButton_Click);

            // * Ayuda Menu *
            var ayudaMenu = new ToolStripMenuItem("Ayuda");

            var acercaDeItem = new ToolStripMenuItem("Acerca de", null, OnAcercaDeClick);
            var guiaItem = new ToolStripMenuItem("Guia", null, OnGuiaClick);

            ayudaMenu.DropDownItems.Add(acercaDeItem);
            ayudaMenu.DropDownItems.Add(guiaItem);

            // * Add Menus and Buttons to MenuBar *
            this.Items.Add(archivoMenu);
            this.Items.Add(compileButton);
            this.Items.Add(executeButton);
            this.Items.Add(ayudaMenu);
        }

        // * Event Handlers *
        private void OnNuevoClick(object? sender, EventArgs? e) { /* Lógica para Nuevo */ }
        private void OnAbrirClick(object? sender, EventArgs? e) { /* Lógica para Abrir */ }
        private void OnGuardarClick(object? sender, EventArgs? e) { /* Lógica para Guardar */ }

        private async void CompileButton_Click(object? sender, EventArgs? e)
        {
        }

        private async void ExecuteButton_Click(object? sender, EventArgs? e)
        {
        }

        private void CodeBox_TextChanged(object? sender, EventArgs? e)
        {
        }

        private void OnAcercaDeClick(object? sender, EventArgs? e)
        {
            MessageBox.Show("KaizenLang IDE\nVersión 1.0", "Acerca de");
        }

        private void OnGuiaClick(object? sender, EventArgs? e)
        {
            var helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }
    }
}
