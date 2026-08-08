using LogiTrack.Data;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace LogiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;

    public InventoryController(LogiTrackContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
    {
        const string cacheKey = "inventory-list";

        if (_cache.TryGetValue(cacheKey, out List<InventoryItem>? items))
        {
            Console.WriteLine("Inventory returned from cache.");
            return items!;
        }

        var stopwatch = Stopwatch.StartNew();

        items = await _context.InventoryItems
            .AsNoTracking()
            .ToListAsync();

        stopwatch.Stop();

        Console.WriteLine(
            $"Database query took {stopwatch.ElapsedMilliseconds} ms");

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

        _cache.Set(cacheKey, items, cacheOptions);

        return items;
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
    {
        var item = await _context.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.ItemId == id);

        if (item == null)
            return NotFound();

        return item;
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> CreateInventoryItem(
        InventoryItem item)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetInventoryItem),
            new { id = item.ItemId },
            item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInventoryItem(
        int id,
        InventoryItem item)
    {
        if (id != item.ItemId)
            return BadRequest();

        _context.Entry(item).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.InventoryItems.Remove(item);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}