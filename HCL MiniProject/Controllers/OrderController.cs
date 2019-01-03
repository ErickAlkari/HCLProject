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
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Approve()
        {
            List<OrderModels> orders = GetPartitionsToApprove();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(List<OrderModels> orders)
        {
            try {
                foreach (OrderModels order in orders)
                {
                    if (order.isAcepted)
                    {
                        order.PartitionKey = order.IdClient;
                        order.RowKey = order.IdOrder;
                        order.ETag = "*";
                        UpdatePartitionsToApprove(order);
                    }
                }
            }
            catch { 
            }

            List<OrderModels> nOrders = GetPartitionsToApprove();
            return View(nOrders);
        }

        private void UpdatePartitionsToApprove(OrderModels om)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(ConfigurationManager.AppSettings["tableStorageOrders"]);

            var operation = TableOperation.Replace(om);

            // Loop through the results, displaying information about the entity.
            table.Execute(operation);
        }

        private List<OrderModels> GetPartitionsToApprove()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(ConfigurationManager.AppSettings["tableStorageOrders"]);

            // Create the table query.
            TableQuery<OrderModels> rangeQuery = new TableQuery<OrderModels>().Where(TableQuery.GenerateFilterConditionForBool("isAcepted", QueryComparisons.Equal, false));

            // Loop through the results, displaying information about the entity.
            List<OrderModels> Orders = table.ExecuteQuery(rangeQuery).ToList();

            return Orders;
        }

        public ActionResult Pay(string id)
        {
            return RedirectToAction("Index","Home");
        }

        public ActionResult Tracking()
        {
            ViewBag.Orders = new List<OrderModels>();
            return View();
        }

        [HttpPost]
        public ActionResult Tracking(string idClient, string IdOrder)
        {
            try {
                List<OrderModels> orders = GetPartitionsSearch(idClient, IdOrder);
                ViewBag.Orders = orders;
                return View();
            }
            catch {
                ViewBag.MessageFail = true;
                return View();
            }
        }

        private List<OrderModels> GetPartitionsSearch(string IdClient, string IdOrder)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("storageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(ConfigurationManager.AppSettings["tableStorageOrders"]);

            // Create the table query.
            TableQuery<OrderModels> rangeQuery = new TableQuery<OrderModels>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, IdClient),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, IdOrder)));

            // Loop through the results, displaying information about the entity.
            List<OrderModels> Orders = table.ExecuteQuery(rangeQuery).ToList();

            return Orders;
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult Create(string idClient, HttpPostedFileBase postedFile)
        {
            try
            {
                OrderModels order = new OrderModels(idClient, Guid.NewGuid().ToString());
                order.ResourceName = postedFile.FileName;
                order.ModificationDate = DateTime.UtcNow;
                order.ResourceURL = order.saveInBlob(postedFile.InputStream, order.RowKey);
                order.saveEntity();

                ViewBag.Process = true;

                // TODO: Add insert logic here
                return RedirectToAction("Index", "Home", new { result = "true", idOrder = order.RowKey.ToString() });
            }
            catch
            {
                ViewBag.Process = false;
                return RedirectToAction("Index", "Home", new { result = "false" });
            }
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Order/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
