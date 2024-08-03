﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_temp.Data;
using ecommerce_temp.Models.Product;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace ecommerce_temp.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ecommerce_tempContext _context;
        private readonly IMemoryCache _cache;
        public ProductController(ILogger<ProductController> logger, ecommerce_tempContext context, IMemoryCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1)
        {
            int pageSize = 6;
            string cacheKey = $"Products_Page_{page}";

            if (!_cache.TryGetValue(cacheKey, out List<ProductViewModel> productViewModels))
            {
                var products = await _context.Products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                productViewModels = products.Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl
                }).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, productViewModels, cacheEntryOptions);
            }

            var totalProduct = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProduct / (double)pageSize);

            var viewModel = new ProductListViewModel
            {
                Products = productViewModels,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // GET: Products/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string cacheKey = $"Product_Details_{id}";
            if (!_cache.TryGetValue(cacheKey, out ProductViewModel viewModel))
            {

                var product = await _context.Products
                    .FirstOrDefaultAsync(m => m.ProductId == id);
                if (product == null)
                {
                    return NotFound();
                }

                viewModel = new ProductViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price
                };
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, viewModel, cacheEntryOptions);
            }
            return View(viewModel);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string item, int page = 1)
        {
            // TODO : lỗi null khi không nhập giá trị nào hêt.
            if (string.IsNullOrEmpty(item) || item == null || item.Length == 0)
            {
                ViewBag.Message = "Please enter a valid keyword (at least 1 characters)";
                return View("Index", new ProductListViewModel());
            }

            var productListViewModel = await SearchProductsAsync(item, page);
            if (productListViewModel == null || !productListViewModel.Products.Any())
            {
                ViewBag.Message = "No products found";
            }

            return Ok(productListViewModel);
        }

        private async Task<ProductListViewModel> SearchProductsAsync(string search, int page)
        {
            int pageSize = 6;
            var input = search.ToLower();
            var products = await _context.Products
                .Where(p => p.ProductName.ToLower().Contains(input))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            var totalProducts = await _context.Products.CountAsync(p => p.ProductName.ToLower().Contains(input));
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            return new ProductListViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }
    }


}
