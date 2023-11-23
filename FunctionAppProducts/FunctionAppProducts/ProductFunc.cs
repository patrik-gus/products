using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace FunctionAppProducts
{   
    public class Product
    {
        public string PartitionKey { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public string Name { get; set; }
         public double Price { get; set; }
         public int Qty { get; set; }
         public bool IsBlocket { get; set; } 
    }

    public class ProductTableEntity : TableEntity
    {      
        public string Name { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public bool IsBlocket { get; set; }
    }

    public static class Mappings
     {
        public static ProductTableEntity ToTableEntity(this Product product)
        {
            return new ProductTableEntity()
            {
                PartitionKey = product.PartitionKey,
                RowKey = product.Id,
                Name = product.Name,
                Price = product.Price,
                Qty = product.Qty,
                IsBlocket = product.IsBlocket                
            };
        }

        public static Product ToProduct(this ProductTableEntity product)
        {
            return new Product()
            {
                PartitionKey = product.PartitionKey,
                Id = product.RowKey,
                Name = product.Name,
                Price = product.Price,
                Qty = product.Qty,
                IsBlocket = product.IsBlocket
            };
        }
    }

    public static class ProductFuncApi    
    {
        [FunctionName("CreateProduct")]
        public static async Task<IActionResult> CreateProduct(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "product")] HttpRequest req,
           [Table("products", Connection = "AzureWebJobsStorage")] IAsyncCollector<ProductTableEntity> productTable,
           ILogger log)
        {
            log.LogInformation("Creating a new product list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Product>(requestBody);

            var product = new Product()
            {        
                PartitionKey = input.PartitionKey,
                Id = input.Id,
                Name = input.Name,
                Price = input.Price,
                Qty = input.Qty,
                IsBlocket = input.IsBlocket                
             };

            await productTable.AddAsync(product.ToTableEntity());
            return new OkObjectResult(product);
        }

        [FunctionName("GetProducts")]
        public static async Task<IActionResult> GetProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "product")] HttpRequest req,
            [Table("products", Connection = "AzureWebJobsStorage")] CloudTable productTable,
                  ILogger log)
        {
            log.LogInformation("Getting products list items");
            var query = new TableQuery<ProductTableEntity>();
            var segment = await productTable.ExecuteQuerySegmentedAsync(query, null);

            return new OkObjectResult(segment.Select(Mappings.ToProduct));
        }
    }
}