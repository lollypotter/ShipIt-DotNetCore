﻿using System;
using System.Collections.Generic;
using System.Linq;
 using Microsoft.AspNetCore.Mvc;
 using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Repositories;
using System.Text;

namespace ShipIt.Controllers
{
    [Route("orders/outbound")]
    public class OutboundOrderController : ControllerBase
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;

        public OutboundOrderController(IStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        [HttpPost("")]
        public OutboundOrderResponse Post([FromBody] OutboundOrderRequestModel request)
        {
            Log.Info(String.Format("Processing outbound order: {0}", request));

            var gtins = new List<String>();
            foreach (var orderLine in request.OrderLines)
            {
                if (gtins.Contains(orderLine.gtin))
                {
                    throw new ValidationException(String.Format("Outbound order request contains duplicate product gtin: {0}", orderLine.gtin));
                }
                gtins.Add(orderLine.gtin);
            }

            var productDataModels = _productRepository.GetProductsByGtin(gtins);
            var products = productDataModels.ToDictionary(p => p.Gtin, p => new Product(p));

            var lineItems = new List<StockAlteration>();
            var productIds = new List<int>();
            var errors = new List<string>();

            foreach (var orderLine in request.OrderLines)
            {
                if (!products.ContainsKey(orderLine.gtin))
                {
                    errors.Add(string.Format("Unknown product gtin: {0}", orderLine.gtin));
                }
                else
                {
                    var product = products[orderLine.gtin];
                    lineItems.Add(new StockAlteration(product.Id, orderLine.quantity));
                    productIds.Add(product.Id);
                    //Console.WriteLine("ID = " +product.Id +" Quantity= " + orderLine.quantity + " Weight = "+ product.Weight);

                    //Console.WriteLine("Number of trucks = " + lineItems.Sum(lineItem => lineItem.ProductWeight)/20000000+"\n");
                }
            }

            if (errors.Count > 0)
            {
                throw new NoSuchEntityException(string.Join("; ", errors));
            }

            var stock = _stockRepository.GetStockByWarehouseAndProductIds(request.WarehouseId, productIds);

            var orderLines = request.OrderLines.ToList();
            errors = new List<string>();

            for (int i = 0; i < lineItems.Count; i++)
            {
                var lineItem = lineItems[i];
                var orderLine = orderLines[i];

                if (!stock.ContainsKey(lineItem.ProductId))
                {
                    errors.Add(string.Format("Product: {0}, no stock held", orderLine.gtin));
                    continue;
                }

                var item = stock[lineItem.ProductId];
                if (lineItem.Quantity > item.held)
                {
                    errors.Add(
                        string.Format("Product: {0}, stock held: {1}, stock to remove: {2}", orderLine.gtin, item.held,
                            lineItem.Quantity));
                }
            }

            if (errors.Count > 0)
            {
                throw new InsufficientStockException(string.Join("; ", errors));
            }
            
            
            //Console.WriteLine("Number of Trucks = " + numberofTrucks);

            _stockRepository.RemoveStock(request.WarehouseId, lineItems);
            return GetNumberofTrucks(lineItems, request.WarehouseId);
        }

        private OutboundOrderResponse GetNumberofTrucks(List<StockAlteration> lineItems, int warehouseId)
        {
            var trucks = new List<Truck>();
            var truckTotalweight = 0.0;
           // var productTotalweight = 0.0;
            var products = new Dictionary<Product, int>();
            var truckid = 0;
            foreach(var lineItem in lineItems)
            {
                var product = _productRepository.GetProductById(lineItem.ProductId);

                //productTotalweight += product.Weight * lineItem.Quantity;//500*3,500*3,1000*1,100*3

                while(lineItem.Quantity > 0)
                {
                    if (truckTotalweight < 2000.0 && (product.Weight * lineItem.Quantity) + truckTotalweight <= 2000.0)
                    {
                        products.Add(new Product(product), lineItem.Quantity); 
                        truckTotalweight += product.Weight * lineItem.Quantity;//1500,1500,//2000
                        lineItem.Quantity = 0;
                    }
                    else
                    {
                        var minQuantityAddedToTruck = Convert.ToInt32(Math.Floor((2000 - truckTotalweight)/product.Weight));//2000 -1500/500

                        if(minQuantityAddedToTruck > 0)
                        {
                            products.Add(new Product(product), minQuantityAddedToTruck);

                            truckTotalweight += minQuantityAddedToTruck * product.Weight;
                            lineItem.Quantity = lineItem.Quantity - minQuantityAddedToTruck;
                        }
                        trucks.Add(new Truck(truckid = truckid + 1, products, truckTotalweight));
                        truckTotalweight = 0;
                        products.Clear();
                    }      
                } 
            }

            if (products.Count() > 0 && truckTotalweight <= 2000)
                trucks.Add(new Truck(truckid = truckid + 1, products, truckTotalweight));


            Console.WriteLine(trucks.Count());
            foreach(var truck in trucks)
            {
                Console.WriteLine("Truck Id = {0} TotalWeight = {1}", truck.TruckId, truck.TotalWeight);
                foreach(var product in truck.Products)
                    Console.WriteLine("Product Id = {0} Weight = {1} Quantity = {2}", product.Key.Id, product.Key.Weight, product.Value);
            }
            return new OutboundOrderResponse()
            {
                WarehouseId = warehouseId,
                Trucks = trucks
            };
        }
    }
}