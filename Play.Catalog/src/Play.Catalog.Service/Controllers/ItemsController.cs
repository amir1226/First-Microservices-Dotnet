using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private readonly IRepository<Item> itemsRepository;
        private static int requestCounter = 0;

        public ItemsController(IRepository<Item> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> Get()
        {
            requestCounter++;
            Console.WriteLine($"Request {requestCounter}: Starting...");
            if(requestCounter<=2) {
                Console.WriteLine($"Request {requestCounter}: Delaying...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if(requestCounter<=4) {
                Console.WriteLine($"Request {requestCounter}: 500(Internal server error)");
                return StatusCode(500);
            }
            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());
            Console.WriteLine($"Request {requestCounter}: 200(Ok)");
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetById(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if(item == null)
            {
                var notFoundPersonalized =  new
                { 
                    Status = 404, 
                    Error = "Couldn't be find an item with that id"
                };
                return NotFound(notFoundPersonalized);
            }


            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name, 
                Description = createItemDto.Description,
                Price = createItemDto.Price, 
                CreatedDate = DateTimeOffset.UtcNow
            };
            await itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetById), new {id = item.Id}, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if(existingItem != null)
            {
                existingItem.Name = updateItemDto.Name;
                existingItem.Description = updateItemDto.Description;
                existingItem.Price = updateItemDto.Price;
                
                await itemsRepository.UpdateAsync(existingItem);
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingItem = await itemsRepository.GetAsync(id);
            if(existingItem == null)
            {
                return NotFound();
            }     
            await this.itemsRepository.RemoveAsync(id);
            return Ok();
        }
    }
}