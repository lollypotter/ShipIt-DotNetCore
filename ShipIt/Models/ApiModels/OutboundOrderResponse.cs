using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace ShipIt.Models.ApiModels
{
    public class OutboundOrderResponse
    {
        public int WarehouseId { get; set; }
        public IEnumerable<Truck> Trucks { get; set;}

        public override String ToString()
        {
            return new StringBuilder()
                .AppendFormat("warehouseId: {0}, ", WarehouseId)
                .AppendFormat("Trucks: {0}", Trucks)
                .ToString();
        }
    }

    public class Truck
    {
        public int TruckId { get; set; }
        public Dictionary<Product, int> Products { get; set; }
        public double TotalWeight { get; set; }

        public Truck(int truckid, Dictionary<Product, int> products, double weight)
        {
            TruckId = truckid;
            Products = new Dictionary<Product, int> (products);
            TotalWeight = weight;
        }
    }
}