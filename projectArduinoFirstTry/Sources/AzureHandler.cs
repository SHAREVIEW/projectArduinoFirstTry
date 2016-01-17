using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace projectArduinoFirstTry.Sources
{
    public static class AzureHandler
    {
        static private CloudStorageAccount _storageAccount = null;
        static private CloudTableClient _tableClient;
        static private CloudTable _table;


        public static void InitializeAzureStorage()
        {
            _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Create the table client.
            _tableClient = _storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            _table = _tableClient.GetTableReference("medicine");
            _table.CreateIfNotExists();
        }

        public static void InsertToTable(Medicine medicine)
        {
            MedicineEntity medicineToInsert = new MedicineEntity(medicine.Code.ToString(), medicine.Name);
            medicineToInsert.Date = medicine.Date.ToString("d");
            medicineToInsert.DangersDesc = medicine.DangersDesc;
            medicineToInsert.UserDesc = medicine.UserDesc;
            medicineToInsert.ImagePath = medicine.ImagePath;
            medicineToInsert.Price = medicine.Price;

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(medicineToInsert);

            // Execute the insert operation.
            _table.Execute(insertOperation);
        }

        public static MedicineList ReadFromTable()
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<MedicineEntity> query = new TableQuery<MedicineEntity>();//.Where(/*TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith")*/);

            MedicineList medicineList = new MedicineList();
            // Print the fields for each customer.
            foreach (MedicineEntity entity in _table.ExecuteQuery(query))
            {
                Medicine medicineToAdd = new Medicine();
                medicineToAdd.Code = long.Parse(entity.RowKey);
                medicineToAdd.Name = entity.PartitionKey;
                medicineToAdd.DangersDesc = entity.DangersDesc;
                medicineToAdd.UserDesc = entity.UserDesc;
                medicineToAdd.Price = entity.Price;
                medicineToAdd.ImagePath = entity.ImagePath;
                medicineToAdd.Date = DateTime.Parse(entity.Date);
                medicineList.MedicineVal.Add(medicineToAdd);
            }

            return medicineList;
        }

        public static void DeleteEntryFromTable()
        {
            // Create a retrieve operation that expects a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<MedicineEntity>("Omega3", "9721356");

            // Execute the operation.
            TableResult retrievedResult = _table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity.
            MedicineEntity deleteEntity = (MedicineEntity)retrievedResult.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                // Execute the operation.
                _table.Execute(deleteOperation);
                Console.WriteLine("Entity deleted.");
            }
            else
                Console.WriteLine("Could not retrieve the entity.");
        }
    }
}
