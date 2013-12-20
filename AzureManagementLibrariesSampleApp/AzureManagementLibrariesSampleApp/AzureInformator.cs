using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.WindowsAzure.Common;
using Microsoft.WindowsAzure.Common.Internals;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.WebSites;
using Microsoft.WindowsAzure.Management.WebSites.Models;
using Microsoft.WindowsAzure.Management.Sql;
using Microsoft.WindowsAzure.Management.Sql.Models;
using System.Configuration;

namespace AzureManagementLibrariesSampleApp
{
    /// <summary>
    /// Вся логика по работе с Windows Azure инкапсулирована в этом классе.
    /// </summary>
    public class AzureHelper
    {
        private const string AppConfigSqlServerName = "SqlServerName";
        private WebSiteManagementClient _websitesClient;
        private SqlManagementClient _sqlClient;
        private CertificateCloudCredentials _certificate;

        // TODO: укажите свой сертификат и идентификатор подписки.
        private const string certificate = "";
        private const string id = "";

        public AzureHelper()
        {
            _certificate = new CertificateCloudCredentials(id, new X509Certificate2(Convert.FromBase64String(certificate)));
            _websitesClient = CloudContext.Clients.CreateWebSiteManagementClient(_certificate);
            _sqlClient = CloudContext.Clients.CreateSqlManagementClient(_certificate);
        }

        /// <summary>
        /// Получение доступных зон.
        /// </summary>
        /// <returns>Названия доступных зон.</returns>
        public string[] GetWebSpaces()
        {
            var task = _websitesClient.WebSpaces.ListAsync();
            try
            {
                task.Wait();
                WebSpacesListResponse webSpaces = task.Result;
                var ret = webSpaces.Select(w => w.Name);
                return ret.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Проверка на существование сайта во всех зонах в рамках подписки.
        /// </summary>
        /// <param name="siteName">Название сайта.</param>
        /// <returns>Существует ли сайт в моей подписке.</returns>
        public bool CheckSiteExists(string siteName)
        {
            string[] webSpaceNames = GetWebSpaces();

            bool siteExist = false;
            foreach (var webSpaceName in webSpaceNames)
            {
                siteExist = CheckSiteExists(siteName, webSpaceName);
                if (siteExist)
                {
                    break;
                }
            }
            return siteExist;
        }

        /// <summary>
        /// Проверка существования сайта в рамках указанной зоны и текущей подписки.
        /// </summary>
        /// <param name="siteName">Название сайта.</param>
        /// <param name="webSpaceName">Зона расположения сайта.</param>
        /// <returns>Существует ли сайт в зоне и текущей подписке.</returns>
        public bool CheckSiteExists(string siteName, string webSpaceName)
        {
            var task = _websitesClient.WebSites.GetAsync(webSpaceName, siteName, new WebSiteGetParameters());

            try
            {
                task.Wait();
                return task.Result != null;
            }
            catch (AggregateException exception)
            {
                var webSiteCloudException = exception.InnerException as WebSiteCloudException;

                if (webSiteCloudException != null &&
                    (webSiteCloudException.ExtendedErrorCode == WebSiteExtendedErrorCodes.SiteNotFound || webSiteCloudException.ExtendedErrorCode == WebSiteExtendedErrorCodes.CannotFindEntity))
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        /// Создание веб-сайта.
        /// </summary>
        /// <param name="siteName">Название веб-сайта.</param>
        /// <param name="webSpaceName">Название зоны.</param>
        public void CreateSite(string siteName, string webSpaceName = null)
        {
            if (webSpaceName == null)
                webSpaceName = WebSpaceNames.WestEuropeWebSpace;

            var parameters = new WebSiteCreateParameters
                {
                    HostNames = new List<string> { siteName + ".azurewebsites.net" },
                    Name = siteName,
                    WebSpaceName = webSpaceName,
                };

            var task = _websitesClient.WebSites.CreateAsync(webSpaceName, parameters);

            try
            {
                task.Wait();
            }
            catch (AggregateException exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Удаление веб-сайта.
        /// </summary>
        /// <param name="siteName">Название веб-сайта.</param>
        /// <param name="webSpaceName">Название зоны.</param>
        public void RemoveSite(string siteName, string webSpaceName = null)
        {
            if (webSpaceName == null)
                webSpaceName = WebSpaceNames.WestEuropeWebSpace;

            var task = _websitesClient.WebSites.DeleteAsync(webSpaceName, siteName, true, true);

            try
            {
                task.Wait();
            }
            catch (AggregateException exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Создать БД SQLAzure.
        /// </summary>
        /// <param name="DBName">Имя базы данных.</param>
        public void CreateDB(string DBName)
        {
            string serverName = ConfigurationManager.AppSettings[AppConfigSqlServerName];
            var parameters = new DatabaseCreateParameters
            {
                Name = DBName,
                CollationName = "SQL_Latin1_General_CP1_CI_AS",
                Edition = "Web",
                MaximumDatabaseSizeInGB = 1
            };

            var task = _sqlClient.Databases.CreateAsync(serverName, parameters);

            try
            {
                task.Wait();
            }
            catch (AggregateException exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Удалить БД SQLAzure.
        /// </summary>
        /// <param name="DBName">Имя базы данных.</param>
        public void RemoveDB(string DBName)
        {
            string serverName = ConfigurationManager.AppSettings[AppConfigSqlServerName];

            var task = _sqlClient.Databases.DeleteAsync(serverName, DBName);

            try
            {
                task.Wait();
            }
            catch (AggregateException exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Создание демонстрационного веб-сайта.
        /// </summary>
        /// <param name="webSiteName">Название веб-сайта.</param>
        /// <param name="operationDetails">Сообщение пользователю.</param>
        /// <returns>Информация об успешности выполнения операции для вывода пользователю.</returns>
        public bool NewDemoWebSite(string webSiteName, out string operationDetails)
        {
            try
            {
                // Проверим нет ли у нас уже такого сайта.
                bool siteExist = CheckSiteExists(webSiteName);
                if (siteExist)
                {
                    operationDetails = "Сайт с таким именем уже существует.";
                    return false;
                }

                // Создадим веб-сайт.
                CreateSite(webSiteName);

                // Создадим SQL Azure БД.
                CreateDB(webSiteName);

                // TODO: Опубликуем сайт, свяжем сайт и БД.

            }
            catch (Exception ex)
            {
                operationDetails = ex.ToString();

                // Удалим всё что удалось создать, т.к. в целом операция была неуспешной.
                RemoveDemoWebSite(webSiteName, out operationDetails);

                return false;
            }
            operationDetails = "Операция создания выполнена успешно.";
            return true;
        }

        /// <summary>
        /// Удаление демонстрационного веб-сайта и базы данных.
        /// </summary>
        /// <param name="webSiteName">Название веб-сайта.</param>
        /// <param name="operationDetails">Сообщение пользователю.</param>
        /// <returns>Информация об успешности выполнения операции для вывода пользователю.</returns>
        public bool RemoveDemoWebSite(string webSiteName, out string operationDetails)
        {
            bool ret = true;
            string exceptionsDetails = "";
            try
            {
                // Проверим есть ли что удалять.
                bool siteExist = CheckSiteExists(webSiteName);
                if (siteExist)
                {
                    // Удалим веб-сайт.
                    RemoveSite(webSiteName);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                exceptionsDetails += ex;
            }

            try
            {
                // Создадим SQL Azure БД.
                RemoveDB(webSiteName);
            }
            catch (Exception ex)
            {
                ret = false;
                exceptionsDetails += ex;
            }

            // TODO: Опубликуем сайт, свяжем сайт и БД.

            // Запишем информацию об исключения.
            if (!string.IsNullOrWhiteSpace(exceptionsDetails))
            {
                operationDetails = exceptionsDetails;
            }
            else
            {
                operationDetails = "Операция удаления выполнена успешно.";
            }
            return ret;
        }
    }
}
