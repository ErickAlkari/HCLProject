using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HCL_MiniProject.Models
{
    public class ClientModels : TableEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CultureInfo { get; set; }

        public ClientModels(string country, string idClient)
        {
            this.PartitionKey = country;
            this.RowKey = idClient;
        }

        public ClientModels()
        { }

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