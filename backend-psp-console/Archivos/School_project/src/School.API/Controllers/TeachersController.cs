using AutoMapper;
using School.API.Extensions;
using School.API.Models;
using School.Application.DTOs;
using School.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/profesores")]
public class TeachersController : ControllerBase
{
    private readonly SchoolDbContext _db;
    private readonly IMapper _mapper;

    public TeachersController(SchoolDbContext db, IMapper mapper)
    {
        _db = db; _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TeacherResponseDTO>>> GetAll(
        [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var query = _db.Teachers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(s =>
                s.DocumentNumber.Contains(q) ||
                s.FirstName.Contains(q) ||
                s.LastName.Contains(q) ||
                s.Email.Contains(q));
        }

        var (items, total) = await query
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .Select(t => new TeacherResponseDTO(t.Id, t.DocumentNumber, t.FirstName + " " + t.LastName, t.Email, t.Specialty, t.Status))
            .ToPagedAsync(page, pageSize, ct);

        return Ok(new PagedResult<TeacherResponseDTO>(items, page, pageSize, total));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeacherResponseDTO>> GetById(int id, CancellationToken ct = default)
    {
        var t = await _db.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (t is null) return NotFound();
        return Ok(new TeacherResponseDTO(t.Id, t.DocumentNumber, t.FirstName + " " + t.LastName, t.Email, t.Specialty, t.Status));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] TeacherRequestDTO dto, CancellationToken ct = default)
    {
        if (await _db.Teachers.AnyAsync(x => x.DocumentNumber == dto.DocumentNumber, ct))
            return Conflict("DocumentNumber ya existe.");

        if (await _db.Teachers.AnyAsync(x => x.Email == dto.Email, ct))
            return Conflict("Email ya existe.");

        var entity = _mapper.Map<School.Domain.Entities.Teacher>(dto);
        _db.Teachers.Add(entity);
        await _db.SaveChangesAsync(ct);
        return Ok(entity.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TeacherRequestDTO dto, CancellationToken ct = default)
    {
        var entity = await _db.Teachers.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        if (entity.DocumentNumber != dto.DocumentNumber &&
            await _db.Teachers.AnyAsync(x => x.DocumentNumber == dto.DocumentNumber && x.Id != id, ct))
            return Conflict("DocumentNumber ya está en uso.");

        if (entity.Email != dto.Email &&
            await _db.Teachers.AnyAsync(x => x.Email == dto.Email && x.Id != id, ct))
            return Conflict("Email ya está en uso.");

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var e = await _db.Teachers.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return NotFound();

        var used = await _db.SubjectOfferings.AnyAsync(x => x.TeacherId == id, ct);
        if (used) return Conflict("No se puede eliminar: el docente está asignado a Materia-Periodo.");

        _db.Teachers.Remove(e);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
