using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Npgsql;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;

namespace ShipIt.Repositories
{
    public interface IWarehouseStockRepository
    {
        IEnumerable<WarehouseStockDataModel> GetStockProductsByWarehouseId(int id);
    }

    public class WarehouseStockRepository : RepositoryBase, IWarehouseStockRepository
    {

        public IEnumerable<WarehouseStockDataModel> GetStockProductsByWarehouseId(int id)
        {
            string sql = 
                "SELECT gtin.p_id, w_id, hld, gtin_cd, gcp_cd, gtin_nm, l_th, ds, min_qt " + 
                "FROM gtin " +
                "JOIN stock ON stock.p_id = gtin.p_id " +
                "WHERE stock.w_id = @warehouseId " + 
                "AND hld < l_th " +
                "AND ds = 0";
            var parameter = new NpgsqlParameter("@warehouseId", id);
            string noProductWithIdErrorMessage = string.Format("No stock found with w_id: {0}", id);

            try
            {
                return base.RunGetQuery(sql, reader => new WarehouseStockDataModel(reader), noProductWithIdErrorMessage, parameter).ToList();
            }
            catch (NoSuchEntityException)
            {
                return new List<WarehouseStockDataModel>();
            }
        }

    }
}