using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using LocalFarmersApi.DTO;
using LocalFarmersApi.Models;

namespace LocalFarmersApi.Controllers
{
    [Authorize]
    public class ProductsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult GetProducts(int page = 1, int pageSize = 5, int? categoryId = null, bool organicOnly = false, decimal? minPrice = null, decimal? maxPrice = null, int? minStockQuantity = null, int? maxStockQuantity = null)
        {
            IQueryable<Product> query = db.Products.Include(p => p.Category);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (organicOnly)
            {
                query = query.Where(p => p.Organic);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (minStockQuantity.HasValue)
            {
                query = query.Where(p => p.StockQuantity >= minStockQuantity.Value);
            }

            if (maxStockQuantity.HasValue)
            {
                query = query.Where(p => p.StockQuantity <= maxStockQuantity.Value);
            }

            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            int skip = (page - 1) * pageSize;

            var products = query.OrderBy(p => p.Id)
                                .Skip(skip)
                                .Take(pageSize)
                                .Select(p => new ProductDTO
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    Description = p.Description,
                                    Price = p.Price,
                                    StockQuantity = p.StockQuantity,
                                    CreatedAt = p.CreatedAt,
                                    CategoryId = p.CategoryId,
                                    Organic = p.Organic,
                                    Category = new CategoryDTO
                                    {
                                        Id = p.Category.Id,
                                        Name = p.Category.Name,
                                        Description = p.Category.Description
                                    }
                                })
                                .ToList();

            var paginationHeader = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = page
            };

            System.Web.HttpContext.Current.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

            return Ok(products);
        }


        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return Content(HttpStatusCode.BadRequest, new { Message = "Product id mismatch." });
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.Message);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }


        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok("product deleted successfully");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}
