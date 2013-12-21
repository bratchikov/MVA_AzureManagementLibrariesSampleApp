namespace AzureManagementLibrariesSampleApp
{
    using System;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Название веб-сайта.
        /// </summary>
        public string WebSiteName
        {
            get
            {
                return "Contoso-" + siteNameTextBox.Text;
            }
        }

        /// <summary>
        /// Выполнить операцию создания или удаления сайта.
        /// </summary>
        /// <param name="newOrRemove">true - создание, false - удаление.</param>
        private void ProcessOperation(bool newOrRemove)
        {
            if (String.IsNullOrWhiteSpace(WebSiteName))
            {
                MessageBox.Show("Имя сайта не указано.");
                return;
            }

            string info;
            bool success;

            try
            {
                var a = new AzureHelper();
                if (newOrRemove)
                {
                    success = a.NewDemoWebSite(WebSiteName, out info);
                }
                else
                {
                    success = a.RemoveDemoWebSite(WebSiteName, out info);
                }
            }
            catch (Exception ex)
            {
                info = ex.ToString();
                success = false;
            }

            MessageBox.Show(info, "Внимание!", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку создания нового веб-сайта.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void NewDemoWebSiteBtn_Click(object sender, EventArgs e)
        {
            ProcessOperation(true);
        }

        /// <summary>
        /// Обрыботчик события нажатия на кнопку удаления веб-сайта.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void RemoveDemoWebSiteBtn_Click(object sender, EventArgs e)
        {
            ProcessOperation(false);
        }
    }
}
