using System.Data;
using ShipIt.Models.ApiModels;

namespace ShipIt.Models.DataModels
{
    public class WarehouseStockDataModel : DataModel
    {
        [DatabaseColumnName("p_id")]
        public int ProductId { get; set; }

        [DatabaseColumnName("w_id")]
        public int WarehouseId { get; set; }

        [DatabaseColumnName("hld")]
        public int Held { get; set; }

        [DatabaseColumnName("gtin_cd")]
        public string Gtin { get; set; }

        [DatabaseColumnName("gcp_cd")]
        public string Gcp { get; set; }

        [DatabaseColumnName("gtin_nm")]
        public string Name { get; set; }

        [DatabaseColumnName("l_th")]
        public int LowerThreshold { get; set; }

        [DatabaseColumnName("ds")]
        public int Discontinued { get; set; }

        [DatabaseColumnName("min_qt")]
        public int MinimumOrderQuantity { get; set; }

        public WarehouseStockDataModel(IDataReader dataReader) : base(dataReader) { }

        public WarehouseStockDataModel() { }

        public WarehouseStockDataModel(WarehouseStock apiModel)
        {
            ProductId = apiModel.ProductId;
            WarehouseId = apiModel.WarehouseId;
            Held = apiModel.Held;
            Gtin = apiModel.Gtin;
            Gcp = apiModel.Gcp;
            Name = apiModel.Name;
            LowerThreshold = apiModel.LowerThreshold;
            Discontinued = apiModel.Discontinued ? 1 : 0;
            MinimumOrderQuantity = apiModel.MinimumOrderQuantity;
        }
    }
}