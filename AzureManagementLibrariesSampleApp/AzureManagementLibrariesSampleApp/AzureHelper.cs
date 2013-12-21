namespace AzureManagementLibrariesSampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.Sql;
    using Microsoft.WindowsAzure.Management.Sql.Models;
    using Microsoft.WindowsAzure.Management.WebSites;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    /// <summary>
    /// Вся логика по работе с Windows Azure инкапсулирована в этом классе.
    /// </summary>
    public class AzureHelper
    {
        /// <summary>
        /// Строка, содержашая закодированный в Base64String сертификат.
        /// TODO: укажите свой сертификат.
        /// </summary>
        private const string Certificate = "";

        /// <summary>
        /// Идентификатор подписки.
        /// TODO: укажите свой идентификатор подписки.
        /// </summary>
        private const string SubscriptionId = "";

        /// <summary>
        /// Ключ в конфигурационном файле, который отвечает за имя SQL Server.
        /// </summary>
        private const string AppConfigSqlServerName = "SqlServerName";

        /// <summary>
        /// Клиент доступа к управлению веб-сайтами Windows Azure.
        /// </summary>
        private WebSiteManagementClient _websitesClient;

        /// <summary>
        /// Клиент доступа к управлению SQL Azure.
        /// </summary>
        private SqlManagementClient _sqlClient;

        /// <summary>
        /// Конструктор без параметров.
        /// </summary>
        public AzureHelper()
        {
            CertificateCloudCredentials certificate = new CertificateCloudCredentials(SubscriptionId, new X509Certificate2(Convert.FromBase64String(Certificate)));
            _websitesClient = CloudContext.Clients.CreateWebSiteManagementClient(certificate);
            _sqlClient = CloudContext.Clients.CreateSqlManagementClient(certificate);
        }

        /// <summary>
        /// Получение доступных зон.
        /// </summary>
        /// <returns>Названия доступных зон.</returns>
        public string[] GetWebSpaces()
        {
            var task = _websitesClient.WebSpaces.ListAsync();
            task.Wait();
            WebSpacesListResponse webSpaces = task.Result;
            var ret = webSpaces.Select(w => w.Name);
            return ret.ToArray();
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

            task.Wait();
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

            task.Wait();
        }

        /// <summary>
        /// Создать БД SQLAzure.
        /// </summary>
        /// <param name="dbName">Имя базы данных.</param>
        public void CreateDb(string dbName)
        {
            string serverName = ConfigurationManager.AppSettings[AppConfigSqlServerName];
            var parameters = new DatabaseCreateParameters
            {
                Name = dbName,
                CollationName = "SQL_Latin1_General_CP1_CI_AS",
                Edition = "Web",
                MaximumDatabaseSizeInGB = 1
            };

            var task = _sqlClient.Databases.CreateAsync(serverName, parameters);

            task.Wait();
        }

        /// <summary>
        /// Удалить БД SQLAzure.
        /// </summary>
        /// <param name="dbName">Имя базы данных.</param>
        public void RemoveDb(string dbName)
        {
            string serverName = ConfigurationManager.AppSettings[AppConfigSqlServerName];

            var task = _sqlClient.Databases.DeleteAsync(serverName, dbName);

            task.Wait();
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
                CreateDb(webSiteName);

                // TODO: Опубликуем сайт, свяжем сайт и БД.
            }
            catch (Exception ex)
            {
                operationDetails = ex.ToString();

                // Удалим всё что удалось создать, т.к. в целом операция была неуспешной.
                string removeInfo;
                RemoveDemoWebSite(webSiteName, out removeInfo);
                operationDetails += removeInfo;

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
            string exceptionsDetails = string.Empty;
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
                // Удалим SQL Azure БД.
                RemoveDb(webSiteName);
            }
            catch (Exception ex)
            {
                ret = false;
                exceptionsDetails += ex;
            }

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
