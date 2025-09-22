using AutoMapper;
using School.API.Extensions;
using School.API.Models;
using School.Application.DTOs;
using School.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/periodos")]
public class PeriodsController : ControllerBase
{
    private readonly SchoolDbContext _db;
    private readonly IMapper _mapper;

    public PeriodsController(SchoolDbContext db, IMapper mapper)
    {
        _db = db; _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PeriodResponseDTO>>> GetAll(
        [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var query = _db.Periods.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(p => p.Name.Contains(q));
        }

        var (items, total) = await query
            .OrderByDescending(p => p.StartDate)
            .Select(p => new PeriodResponseDTO(p.Id, p.Name, p.StartDate, p.EndDate, p.Status))
            .ToPagedAsync(page, pageSize, ct);

        return Ok(new PagedResult<PeriodResponseDTO>(items, page, pageSize, total));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PeriodResponseDTO>> GetById(int id, CancellationToken ct = default)
    {
        var p = await _db.Periods.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p is null) return NotFound();
        return Ok(new PeriodResponseDTO(p.Id, p.Name, p.StartDate, p.EndDate, p.Status));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] PeriodRequestDTO dto, CancellationToken ct = default)
    {
        if (await _db.Periods.AnyAsync(x => x.Name == dto.Name, ct))
            return Conflict("Ya existe un Periodo con ese Name.");

        var entity = _mapper.Map<School.Domain.Entities.Period>(dto);
        _db.Periods.Add(entity);
        await _db.SaveChangesAsync(ct);
        return Ok(entity.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PeriodRequestDTO dto, CancellationToken ct = default)
    {
        var entity = await _db.Periods.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        if (entity.Name != dto.Name && await _db.Periods.AnyAsync(x => x.Name == dto.Name && x.Id != id, ct))
            return Conflict("Name ya está en uso.");

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var e = await _db.Periods.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return NotFound();

        var used = await _db.SubjectOfferings.AnyAsync(x => x.PeriodId == id, ct);
        if (used) return Conflict("No se puede eliminar: hay Materias asociadas a este Periodo.");

        _db.Periods.Remove(e);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
