using System;
using System.Text;
using ShipIt.Models.DataModels;

namespace ShipIt.Models.ApiModels
{
    public class WarehouseStock
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Held { get; set; }
        public string Gtin { get; set; }
        public string Gcp { get; set; }
        public string Name { get; set; }
        public int LowerThreshold { get; set; }
        public bool Discontinued { get; set; }
        public int MinimumOrderQuantity { get; set; }

        public WarehouseStock(WarehouseStockDataModel dataModel)
        {
            ProductId = dataModel.ProductId;
            WarehouseId = dataModel.WarehouseId;
            Held = dataModel.Held;
            Gtin = dataModel.Gtin;
            Gcp = dataModel.Gcp;
            Name = dataModel.Name;
            LowerThreshold = dataModel.LowerThreshold;
            Discontinued = dataModel.Discontinued == 1;
            MinimumOrderQuantity = dataModel.MinimumOrderQuantity;
        }

        //Empty constructor needed for Xml serialization
        public WarehouseStock()
        {
        }

        public override String ToString()
        {
            return new StringBuilder()
                    .AppendFormat("gtin: {0}, ", Gtin)
                    .AppendFormat("gcp: {0}, ", Gcp)
                    .AppendFormat("name: {0}, ", Name)
                    .AppendFormat("lowerThreshold: {0}, ", LowerThreshold)
                    .AppendFormat("discontinued: {0}, ", Discontinued)
                    .AppendFormat("minimumOrderQuantity: {0}, ", MinimumOrderQuantity)
                    .ToString();
        }
    }
}