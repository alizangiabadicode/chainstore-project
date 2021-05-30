using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Data.Traditional;
using ChainStore.Models;
using ChainStore.Models.ViewModel;
using ChainStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Administrator")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;
        private static Qdatabase qdb;
        private static int orm;
        [BindProperty]
        public ProductsViewModel ProductsVm { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            qdb = new Qdatabase();
            _db = db;
            orm = 0;
            _hostingEnvironment = hostingEnvironment;
            ProductsVm = new ProductsViewModel()
            {
                Products = new Products(),
                SpecialTags = _db.SpecialTags.ToList(),
                ProductTypes = _db.ProductTypes.ToList()
            };
        }

        public async Task<IActionResult> Index()
        {
            if (Convert.ToInt32(TempData["edit"]) == 1)
            {
                ViewBag.edit = true;
            }
            else if (Convert.ToInt32(TempData["delete"]) == 1)
            {
                ViewBag.delete = true;
            }
            else if (Convert.ToInt32(TempData["create"]) == 1)
            {
                ViewBag.create = true;
            }

            List<Products> product;
            if (orm == 1)
            {
                product = qdb.retProduct();
                qdb.include_pt_st(product);
            }
            else
            {
                product = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).ToListAsync();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            return View("Create", ProductsVm);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()

        {
            if (!ModelState.IsValid)
            {
                List<Special_Tags> specialtags = null;
                List<ProductTypes> producttypes = null;
                if (orm == 1)
                {
                    specialtags = qdb.retSpecialTag();
                    producttypes = qdb.retProductType();
                }
                else
                {
                    specialtags = _db.SpecialTags.ToList();
                    producttypes = _db.ProductTypes.ToList();
                }
                ProductsVm.SpecialTags = specialtags;
                ProductsVm.ProductTypes = producttypes;
                return View("Create", ProductsVm);
            }

            Products obj = null;
            if (orm == 1)
            {
                obj = ProductsVm.Products;
                obj.Id = qdb.inProduct(ProductsVm.Products);
                //obj = qdb.retProduct(ProductsVm.Products);
            }
            else
            {
                _db.Products.Add(ProductsVm.Products);
                await _db.SaveChangesAsync();
                obj = await _db.Products.FindAsync(ProductsVm.Products.Id);
            }

            

            string path = _hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;


            //if the user uploads image :
            if (files.Count != 0)
            {
                string extension = Path.GetExtension(files[0].FileName);
                string newName = path + @"\" + SD.ImageFolder + @"\" + ProductsVm.Products.Id + extension;
                using (FileStream fs = new FileStream(newName, FileMode.Create))
                {
                    files[0].CopyTo(fs);
                }

                obj.Image = @"\" + SD.ImageFolder + @"\" + ProductsVm.Products.Id + extension;
            }
            //if user doesn't upload any image :
            else
            {
                string newName = path + @"\" + SD.ImageFolder + @"\" + SD.DefaultProductImage;
                System.IO.File.Copy(newName, path + @"\" + SD.ImageFolder + @"\" + ProductsVm.Products.Id + ".jpg");
                obj.Image = @"\" + SD.ImageFolder + @"\" + ProductsVm.Products.Id +".jpg";
            }

            if (orm == 1)
            {
                qdb.upProduct(obj);
            }
            else
            {
                await _db.SaveChangesAsync();
            }

            TempData["create"] = 1;
            return RedirectToAction(nameof(Index));
        }

        //Get : Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (orm == 1)
            {
                ProductsVm.Products = qdb.retProduct((int) id);
            }
            else
            {
                ProductsVm.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes)
                    .FirstAsync(m => m.Id == id);
            }


            if (ProductsVm.Products == null)
            {
                return NotFound();
            }

            List<Special_Tags> specialtags = null;
            List<ProductTypes> producttypes = null;
            if (orm == 1)
            {
                specialtags = qdb.retSpecialTag();
                producttypes = qdb.retProductType();
            }
            else
            {
                specialtags = _db.SpecialTags.ToList();
                producttypes = _db.ProductTypes.ToList();
            }

            ProductsVm.SpecialTags = specialtags;
            ProductsVm.ProductTypes = producttypes;
            return View(ProductsVm);
        }

        // Post : Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            if (ModelState.IsValid)
            {
                var webRoot = _hostingEnvironment.WebRootPath;
                Products oldProduct = null;

                if (orm == 1)
                {
                    oldProduct = qdb.retProduct(id);
                }
                else
                {
                    oldProduct = await _db.Products.FirstAsync(x => x.Id == ProductsVm.Products.Id);
                }
                
                if(oldProduct == null)
                {
                    return NotFound();
                }

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0 && files[0] != null && files[0].Length > 0)
                {
                    var folderPath = Path.Combine(webRoot, SD.ImageFolder);
                    var extensionNew = Path.GetExtension(files[0].FileName);
                    var extensionOld = Path.GetExtension(oldProduct.Image);
                    if (System.IO.File.Exists(folderPath + @"\" + ProductsVm.Products.Id + extensionOld))
                    {
                        System.IO.File.Delete(folderPath + @"\" + ProductsVm.Products.Id + extensionOld);
                    }

                    using (FileStream fs =
                        new FileStream(folderPath + @"\" + ProductsVm.Products.Id + extensionNew,
                            FileMode.Create))
                    {
                        files[0].CopyTo(fs);
                    }

                    ProductsVm.Products.Image =
                        @"\" + SD.ImageFolder + @"\" + ProductsVm.Products.Id + extensionNew;
                }

                if (ProductsVm.Products.Image != null)
                {
                    oldProduct.Image = ProductsVm.Products.Image; 
                }

                oldProduct.Name = ProductsVm.Products.Name;
                oldProduct.SpecialTagId = ProductsVm.Products.SpecialTagId;
                oldProduct.Price = ProductsVm.Products.Price;
                oldProduct.ProductTypeId = ProductsVm.Products.ProductTypeId;
                oldProduct.Available = ProductsVm.Products.Available;
                oldProduct.Count = ProductsVm.Products.Count;

                if (orm == 1)
                {
                    qdb.upProduct(oldProduct);
                }
                else
                {
                    await _db.SaveChangesAsync();
                }

                TempData["edit"] = 1;
                return RedirectToAction(nameof(Index));
            }

            return View(ProductsVm);
        }

        // Get :  Detail
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (orm == 1)
            {
                ProductsVm.Products = qdb.retProduct((int)id);
                List<Products> ls = new List<Products>(); ls.Add(ProductsVm.Products);
                qdb.include_pt_st(ls);
            }
            else
            {
                ProductsVm.Products = await _db.Products.Include(e => e.ProductTypes).Include(e => e.SpecialTags).FirstAsync(e => e.Id == (int)id);
            }

            if (ProductsVm.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVm);
        }

        // Get : Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (orm == 1)
            {
                ProductsVm.Products = qdb.retProduct((int) id);
                List<Products> ls = new List<Products>();ls.Add(ProductsVm.Products);
                qdb.include_pt_st(ls);
            }
            else
            {
                ProductsVm.Products = await _db.Products.FirstAsync(m => m.Id == id);
            }

            if (ProductsVm.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVm);
        }

        // Post : Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var webRoot = _hostingEnvironment.WebRootPath;
            Products p = null;
            if (orm == 1)
            {
                p = qdb.retProduct(id);
            }
            else
            {
                p = await _db.Products.FindAsync(id);
            }

            if (p == null)
            {
                return NotFound();
            }
            else
            {
                var path = Path.Combine(webRoot, SD.ImageFolder);
                var path2 = Path.Combine(path, p.Id.ToString()) + Path.GetExtension(p.Image);
                if (System.IO.File.Exists(path2))
                {
                    System.IO.File.Delete(path2);
                }

                if (orm == 1)
                {
                    qdb.rmproduct(id);
                }
                else
                {
                    _db.Remove(p);
                    await _db.SaveChangesAsync();
                }

                TempData["delete"] = 1;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
