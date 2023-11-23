# products
FuncAppProducts is a solution for a Azure Function that exposes two HTTP API endpoints.

1. HTTP endpoint that receives an JSON Product structure and savs in a Azure table storage named "products"
   
{
   "PartitionKey" : "Sweden", 
   "Id"           : "10020",
   "Name"		: "HYDRANEX",
   "Price" 		:  99.20,
   "Qty"		:  10,
   "IsBlocket" 	:  "true"  }

2. HTTP enpoint that retrives all the created products from the Azure table storege "products"
   The result is listed in the same way as A JSON structure.
  
