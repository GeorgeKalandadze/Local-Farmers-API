using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using LocalFarmersApi.DTO;
using LocalFarmersApi.Models;
using Microsoft.AspNet.Identity;

namespace LocalFarmersApi.Controllers
{
    [Authorize]
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult GetOrders(int page = 1, int pageSize = 5, DateTime? startDate = null, DateTime? endDate = null, decimal? minAmount = null, decimal? maxAmount = null)
        {
            var userId = User.Identity.GetUserId();
            IQueryable<Order> query = db.Orders.Where(o => o.UserId == userId);

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value);
            }

            if (minAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= maxAmount.Value);
            }

            int skip = (page - 1) * pageSize;

            var orders = query.OrderByDescending(o => o.OrderDate)
                              .Skip(skip)
                              .Take(pageSize)
                              .Select(o => new OrderDTO
                              {
                                  Id = o.Id,
                                  OrderDate = o.OrderDate,
                                  TotalAmount = o.TotalAmount,
                                  OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                                  {
                                      ProductId = oi.ProductId,
                                      ProductName = oi.Product.Name,
                                      UnitPrice = oi.UnitPrice,
                                      Quantity = oi.Quantity
                                  }).ToList()
                              })
                              .ToList();

            return Ok(orders);
        }

        public IHttpActionResult GetOrder(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = db.Orders
                          .Where(o => o.Id == id && o.UserId == userId)
                          .Select(o => new OrderDTO
                          {
                              Id = o.Id,
                              OrderDate = o.OrderDate,
                              TotalAmount = o.TotalAmount,
                              OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                              {
                                  ProductId = oi.ProductId,
                                  ProductName = oi.Product.Name,
                                  UnitPrice = oi.UnitPrice,
                                  Quantity = oi.Quantity
                              }).ToList()
                          })
                          .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        public IHttpActionResult PostOrder()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems.Where(ci => ci.UserId == userId).ToList();

            if (!cartItems.Any())
            {
                return BadRequest("Cart is empty.");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };

            db.Orders.Add(order);
            db.CartItems.RemoveRange(cartItems);
            db.SaveChanges();

            return Ok("Order placed successfully.");
        }

    }
}
