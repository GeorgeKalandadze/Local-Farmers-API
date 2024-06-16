using System.Linq;
using System.Net;
using System.Web.Http;
using LocalFarmersApi.DTO;
using LocalFarmersApi.Models;
using Microsoft.AspNet.Identity;

namespace LocalFarmersApi.Controllers
{
    [Authorize]
    public class ShoppingCartController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        [Route("api/ShoppingCart")]
        public IHttpActionResult GetCartItems()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems
                              .Where(ci => ci.UserId == userId)
                              .Select(ci => new CartItemDTO
                              {
                                  Id = ci.Id,
                                  ProductId = ci.ProductId,
                                  ProductName = ci.Product.Name,
                                  ProductPrice = ci.Product.Price,
                                  Quantity = ci.Quantity
                              })
                              .ToList();

            return Ok(cartItems);
        }

        [HttpPost]
        [Route("api/ShoppingCart")]
        public IHttpActionResult PostCartItem(int productId, int quantity)
        {
            var userId = User.Identity.GetUserId();

            var product = db.Products.Find(productId);
            if (product == null)
            {
                return BadRequest("Product not found.");
            }

            var cartItem = db.CartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                db.CartItems.Add(cartItem);
                db.SaveChanges();
                return Ok("Product added to cart");
            }
            else
            {
                return Ok("Product already in cart");
            }
        }


        [HttpDelete]
        [Route("api/ShoppingCart/{id}")]
        public IHttpActionResult DeleteCartItem(int id)
        {
            var cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            db.CartItems.Remove(cartItem);
            db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("api/ShoppingCart/IncreaseQuantity")]
        public IHttpActionResult IncreaseQuantity(int cartItemId)
        {
            var cartItem = db.CartItems.Find(cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity += 1;
            db.SaveChanges();

            return Ok("Quantity increased");
        }

        [HttpPost]
        [Route("api/ShoppingCart/DecreaseQuantity")]
        public IHttpActionResult DecreaseQuantity(int cartItemId)
        {
            var cartItem = db.CartItems.Find(cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity -= 1;
            if (cartItem.Quantity <= 0)
            {
                db.CartItems.Remove(cartItem);
            }

            db.SaveChanges();
            return Ok(cartItem.Quantity > 0 ? "Quantity decreased" : "Item removed from cart");
        }
    }
}
