# HCLProject
HCL Mini Project

Problem to solve:
In an organization ABC, the quotation is received for providing e-filing service. 
An order is created internally to serve the requested service. 
The order is approved by the senior person and service is provided to the requester. 
An invoice is generated against the service order and ask to release the payment for the same. 
The payment is done online by the party with thanks.

More Information
* Requesters are across the globe and interacting through different devices in different language
* The users are in range – 5000 – 10000 concurrent users
* The number of requests may be at peaks during December-March
* Azure and Microsoft stack is strategic decision to develop the solution
* Partners could leverage solution capability for automation

Technologies employed
  - Bootstrap 3
  - JavaScript
  - JQuery
  - Azure Tables
  - Azure Blobs
  - MVC
  - C#
  - Rest Services
  - Azure App Service
  
 Requirements for run the project:
 - We need an Azure Conection String. We can put this in the Web Config file in the App Settings seccion. The name of the setting is "storageConnectionString"
 - The system creates the blobs and the tables if there doesnt exist.
 - The public access level of the blob "quotation" needs to be "Public Read Access".
  
Architecture Challeges:
Some of the requirements of the system are so specific and they were solved by the architecture of the aplication
  - Requesters over the globe[Language and Device]: We solved this point using bootstrap and responsive desing, so the views are aviable to see in a confortable way in any device were the final user decide to use the service. Talking about the language, we are using resoursing files for adapt the view to the local language of the client, we take the culture info in the request.[In the proyect is only aviable in English and Spanish].
  - Concurrrent users and request peaks: Using Azure App Service we can scale the solution in vertical form. So we can provide service to all the clients in dinamicall form using autoscaling, also one of the benefits of use an app service is that we can scal in horizontal way increasing the number of instances.
  - For store the data we are using Azure Tables, the desing of the tables was thinking also in the performance of the app, so we are using two tables. Table A is called clients, and storage the clients information, using as Partition Key the region were the clients belongs, and the row key is the unique id of the clients. The table B is called orders, and is used for save the clients request, using as Partition Key the Id of the client, and as Row Key the Id of the order, we have a high performace queries.
  
  Work Flow(How the project works):
  1. As the problem says, we start in the "Home" view, for introduce a new quotation. This scren will request the Id of the client, and the quotation to send. Once we upload the quotation and fill the fields we send the petition, and the system return to us the id for track the order if all its ok, otherwise will return a error message.
  2. With the given ID we can track the order in the "Tracing" section.
  3. In the "Tracing" section, we need to provide the client id, and the id of the order that we want to track.If the given data exist the system will show us a table, with the resource name(Link to the uploaded quotation), the last time that the order was modfied, and the status of the same(Pending means that order is not attended yet). If the order or the client doesn´t exist then table will be empty
  4. We also count with the section "Approve", this section should be accessed only by the senior person, who is the one that can approve the order.
  In this section we found all the orders in status pendding to accept. We shows in a table the client Id, the order id, the resource name(link to the document), the last modified date, and a check box for accept the order(if we want to accept the order we check this box).
  5. Once we decide what order we are going to approve we press the button "Save" and the orders that was selected will pass to the state accepted.
  6. Again in the "Tracing" section if we consult again an order that was previously accepted then we can found in the column "Status" a button that says "Pay".
  7. End of the flow.
