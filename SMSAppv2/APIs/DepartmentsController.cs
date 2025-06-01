﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMSAppv2.CommandQueries;
using SMSAppv2.Data;
using SMSAppv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSAppv2.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]

        [Route("/api/Departments/GetHistory")]
        public async Task<ActionResult<IEnumerable<Departments>>> GetHistory()
        {
            return await _context.Departments
                .Where(c => !c.isDeleted)
                .OrderByDescending(c => c.Id)
                .Take(3)
                .ToListAsync();
        }
        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentQuery>>> GetDepartments()
        {
            var entities = await _context.Departments
                .Where(c=>!c.isDeleted)
                .ToListAsync();
            var result= new List<DepartmentQuery>();
            foreach (var entity in entities)
            {
                var r = new DepartmentQuery()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Code = entity.Code,
                    Short_name = entity.Short_name,
                };
                result.Add(r);
            }
            return result;
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentQuery>> GetDepartments(int id)
        {
            
            var entity = await _context.Departments.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }
            var result = new DepartmentQuery()
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Short_name = entity.Short_name

            };

            return result;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartments(int id, DepartmentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            var entity = new Departments()
            {
                Id=command.Id,
                Name=command.Name,
                Code = command.Code,
                Short_name = command.Short_name,
                editedAt=DateTime.Now
            };

            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Departments>> PostDepartments(DepartmentCommand command)
        {
            var entity = new Departments()
            {
                Id= command.Id,
                Name= command.Name,
                Code= command.Code,
                Short_name= command.Short_name,
                createdAt= DateTime.Now
            };
            _context.Departments.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(entity.Id);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartments(int id)
        {
            var entity = await _context.Departments.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.isDeleted = true;
            entity.deletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentsExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
