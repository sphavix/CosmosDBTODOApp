using Microsoft.AspNetCore.Mvc;
using CosmosDBTODOApp.Services;
using CosmosDBTODOApp.Models;

namespace CosmosDBTODOApp.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ICosmosService _dbcontext;
        public ItemsController(ICosmosService dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dbcontext.GetItemsAsync("SELECT * FROM c"));
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _dbcontext.GetItemAsync(id));
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                //item.Id = Guid.NewGuid().ToString();
                await _dbcontext.AddItemAsync(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
                return BadRequest();

            var item = await _dbcontext.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                await _dbcontext.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Item item = await _dbcontext.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _dbcontext.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }
    }
}
