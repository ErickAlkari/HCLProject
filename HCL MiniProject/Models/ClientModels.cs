using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using System;
using System.Web;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace HCL_MiniProject.Models
{
    public class ClientModels : TableEntity
    {
        /// <summary>
        /// First name of the client
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the client
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Where the client belongs
        /// </summary>
        public string CultureInfo { get; set; }

        /// <summary>
        /// Constructor for new clients
        /// </summary>
        /// <param name="country">Region where the client belongs</param>
        /// <param name="idClient">Unique id of the client</param>
        public ClientModels(string country, string idClient)
        {
            this.PartitionKey = country;
            this.RowKey = idClient;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public ClientModels()
        { }

        /// <summary>
        /// Method for save this entity in the table of clients
        /// </summary>
        private void saveEntity()
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference(ConfigurationManager.AppSettings["tableStorageClients"]);

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(this);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }
            catch { }
        }
    }
}