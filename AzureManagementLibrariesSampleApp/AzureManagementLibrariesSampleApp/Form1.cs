using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AzureManagementLibrariesSampleApp
{
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
            var a = new AzureHelper();
            if (String.IsNullOrWhiteSpace(WebSiteName))
            {
                MessageBox.Show("Имя сайта не указано.");
                return;
            }

            string info;
            bool success;
            if (newOrRemove)
            {
                success = a.NewDemoWebSite(WebSiteName, out info);
            }
            else
            {
                success = a.RemoveDemoWebSite(WebSiteName, out info);
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
