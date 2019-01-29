using HCL_MiniProject.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HCL_MiniProject.Controllers
{
    public class OrderController : Controller
    {
        // POST: Order/Create
        /// <summary>
        /// Create a new order in the table storage
        /// </summary>
        /// <param name="idClient">Id of the clients who the order belongs</param>
        /// <param name="postedFile">File that correspond to the quotation</param>
        /// <returns>Redirect to the view, if the operation is ok, then will return in the viewbag a tracking id, otherwise will show a error message</returns>
        [HttpPost]
        public ActionResult Create(string idClient, HttpPostedFileBase postedFile)
        {
            try
            {
                //Create and save the new order in the table storage
                OrderModels order = new OrderModels(idClient, Guid.NewGuid().ToString());
                order.ResourceName = postedFile.FileName;
                order.ModificationDate = DateTime.UtcNow;
                order.ResourceURL = order.saveInBlob(postedFile.InputStream, order.RowKey);
                order.saveEntity();

                //Save as result of the operation "true"
                ViewBag.Process = true;

                // TODO: Add insert logic here
                return RedirectToAction("Index", "Home", new { result = "true", idOrder = order.RowKey.ToString() });
            }
            catch
            {
                //If the operation fail the we return and error, and the view will show an error message
                ViewBag.Process = false;
                return RedirectToAction("Index", "Home", new { result = "false" });
            }
        }

        /// <summary>
        /// View for get a list of orders to approve
        /// </summary>
        /// <returns>View approve with data of the orders to approve in the viewbag</returns>
        public ActionResult Approve()
        {
            List<OrderModels> orders = ExecuteQuery(null, null, "GetPartitionsToApprove");
            return View(orders);
        }

        /// <summary>
        /// If there is orders approved save the new status of the orders in the table storage
        /// </summary>
        /// <param name="orders">List with the orders that can be approved</param>
        /// <returns>View Approve, with the list of orders to approve</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(List<OrderModels> orders)
        {
            try {
                //Check if the order is approved and save the new status
                foreach (OrderModels order in orders.Where(m=>m.isAcepted))
                {
                    order.PartitionKey = order.IdClient;
                    order.RowKey = order.IdOrder;
                    order.ETag = "*";
                    UpdatePartitionsToApprove(order);
                }
            }
            catch { 
            }

            //Get orders that are not approved yet
            List<OrderModels> nOrders = ExecuteQuery(null, null, "GetPartitionsToApprove");
            return View(nOrders);
        }

        /// <summary>
        /// View for tracking to orders
        /// </summary>
        /// <returns>View tracking</returns>
        public ActionResult Tracking()
        {
            ViewBag.Orders = new List<OrderModels>();
            return View();
        }

        /// <summary>
        /// Retrive the data of one order
        /// </summary>
        /// <param name="idClient">Id of the client who the order belongs</param>
        /// <param name="IdOrder">Id of the order to track</param>
        /// <returns>If the order exist, the return the data of the order, otherwise return a error message</returns>
        [HttpPost]
        public ActionResult Tracking(string idClient, string IdOrder)
        {
            try {
                List<OrderModels> orders = ExecuteQuery(idClient, IdOrder, "GetPartitionsSearch");
                ViewBag.Orders = orders;
                return View();
            }
            catch {
                ViewBag.MessageFail = true;
                return View();
            }
        }

        /// <summary>
        /// Is not implemented yet, so the method just redirect to the main page
        /// </summary>
        /// <param name="id">The id of the order to proceed to pay</param>
        /// <returns></returns>
        public ActionResult Pay(string id)
        {
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Given and entity of type "OrderModels" we replace the existing data of the entity in the storage table.
        /// </summary>
        /// <param name="om">Entity that will be replaced in the table</param>
        private void UpdatePartitionsToApprove(OrderModels om)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(ConfigurationManager.AppSettings["tableStorageOrders"]);

            //Replace the older regitry with the new data
            var operation = TableOperation.Replace(om);

            // Loop through the results, displaying information about the entity.
            table.Execute(operation);
        }

        /// <summary>
        /// Execute a query for retrive data from storage tables
        /// </summary>
        /// <param name="IdClient">Id of client who made the request</param>
        /// <param name="IdOrder">Id of the order to track</param>
        /// <param name="kindOfQuery">Name of the query to execuete</param>
        /// <returns></returns>
        private List<OrderModels> ExecuteQuery(string IdClient, string IdOrder, string kindOfQuery)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(ConfigurationManager.AppSettings["tableStorageOrders"]);

            // Create the table query.
            TableQuery<OrderModels> rangeQuery = null;

            //Get order information
            if (kindOfQuery == "GetPartitionsSearch")
            {
                rangeQuery = new TableQuery<OrderModels>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, IdClient),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, IdOrder)));
            }
            //Get all orders that aren't approved
            else if (kindOfQuery == "GetPartitionsToApprove")
            {
                rangeQuery = new TableQuery<OrderModels>().Where(
                TableQuery.GenerateFilterConditionForBool("isAcepted", QueryComparisons.Equal, false));
            }
            

            // Loop through the results, displaying information about the entity.
            List<OrderModels> Orders = table.ExecuteQuery(rangeQuery).ToList();

            return Orders;
        }
    }
}
