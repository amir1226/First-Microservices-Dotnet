using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Potion", "Restore a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        };

        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            ItemDto item = items.Find( item => item.Id == id);

            if(item == null)
            {
                var notFoundPersonalized =  new
                { 
                    Status = 404, 
                    Error = "Couldn't be find an item with that id"
                };
                return NotFound(notFoundPersonalized);
            }


            return item;
        }

        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description,
                        createItemDto.Price, DateTimeOffset.UtcNow);
            return CreatedAtAction(nameof(GetById), new {id = item.Id}, item);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            ItemDto existingItem = items.Find( item => item.Id == id);
            if(existingItem != null)
            {
                var updatedItem = existingItem with
                {
                    Name = updateItemDto.Name,
                    Description = updateItemDto.Description,
                    Price = updateItemDto.Price
                };

                var index = items.FindIndex(item => item.Id == id);
                items[index] = updatedItem;

                return Ok();
            }

            return NotFound("Couldn't be find an item with that id");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);  
            if(index != -1)
            {
                items.RemoveAt(index);
                return Ok();
            }     

            return NotFound("Couldn't be find an item with that id");
        }
    }
}