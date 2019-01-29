using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace HCL_MiniProject.Models
{
    public class OrderModels : TableEntity
    {
        /// <summary>
        /// Unique id of the client that the order belongs
        /// </summary>
        public string IdClient { get; set; }

        /// <summary>
        /// Unique id of the order
        /// </summary>
        public string IdOrder { get; set; }

        /// <summary>
        /// Name of the resource
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// URL where we can download the resource from the blob container
        /// </summary>
        public string ResourceURL { get; set; }

        /// <summary>
        /// Flag that indicates if this order was accepted or is pending yet
        /// </summary>
        public bool isAcepted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idClient">Id of the client who is requested the service</param>
        /// <param name="idOrder">Unique id of the client's order</param>
        public OrderModels(string idClient, string idOrder)
        {
            this.PartitionKey = idClient;
            this.RowKey = idOrder;
            this.IdClient = idClient;
            this.IdOrder = idOrder;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public OrderModels()
        {
        }

        /// <summary>
        /// Method for save in a blob, a given file
        /// </summary>
        /// <param name="fileStream">Stream of the file</param>
        /// <param name="FileName">Name of the file</param>
        /// <returns></returns>
        public string saveInBlob(Stream fileStream, string FileName)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["blobStorageQuotation"]);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            //New CloudBlockBlob
            CloudBlockBlob blockBlob;

            // Retrieve reference to a blob named "myblob".
            blockBlob = container.GetBlockBlobReference(FileName.ToLower());

            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadFromStream(fileStream);

            return blockBlob.Uri.ToString();
        }

        /// <summary>
        /// Save in the storage table this entity
        /// </summary>
        public void saveEntity()
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference(ConfigurationManager.AppSettings["tableStorageOrders"]);

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