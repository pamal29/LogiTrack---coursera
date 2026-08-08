using LogiTrack.Data;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;

    public InventoryController(LogiTrackContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .ToListAsync();
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