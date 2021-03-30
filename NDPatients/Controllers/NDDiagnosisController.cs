using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NDPatients.Models;

namespace NDPatients.Controllers
{
    public class NDDiagnosisController : Controller
    {
        private readonly PatientsContext _context;

        public NDDiagnosisController(PatientsContext context)
        {
            _context = context;
        }

        // GET: NDDiagnosis
        public async Task<IActionResult> Index()
        {
            var patientsContext = _context.Diagnosis.Include(d => d.DiagnosisCategory);
            return View(await patientsContext.ToListAsync());
        }

        // GET: NDDiagnosis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosis = await _context.Diagnosis
                .Include(d => d.DiagnosisCategory)
                .FirstOrDefaultAsync(m => m.DiagnosisId == id);
            if (diagnosis == null)
            {
                return NotFound();
            }

            return View(diagnosis);
        }

        // GET: NDDiagnosis/Create
        public IActionResult Create()
        {
            ViewData["DiagnosisCategoryId"] = new SelectList(_context.DiagnosisCategory, "Id", "Name");
            return View();
        }

        // POST: NDDiagnosis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiagnosisId,Name,DiagnosisCategoryId")] Diagnosis diagnosis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diagnosis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiagnosisCategoryId"] = new SelectList(_context.DiagnosisCategory, "Id", "Name", diagnosis.DiagnosisCategoryId);
            return View(diagnosis);
        }

        // GET: NDDiagnosis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosis = await _context.Diagnosis.FindAsync(id);
            if (diagnosis == null)
            {
                return NotFound();
            }
            ViewData["DiagnosisCategoryId"] = new SelectList(_context.DiagnosisCategory, "Id", "Name", diagnosis.DiagnosisCategoryId);
            return View(diagnosis);
        }

        // POST: NDDiagnosis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiagnosisId,Name,DiagnosisCategoryId")] Diagnosis diagnosis)
        {
            if (id != diagnosis.DiagnosisId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosisExists(diagnosis.DiagnosisId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiagnosisCategoryId"] = new SelectList(_context.DiagnosisCategory, "Id", "Name", diagnosis.DiagnosisCategoryId);
            return View(diagnosis);
        }

        // GET: NDDiagnosis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosis = await _context.Diagnosis
                .Include(d => d.DiagnosisCategory)
                .FirstOrDefaultAsync(m => m.DiagnosisId == id);
            if (diagnosis == null)
            {
                return NotFound();
            }

            return View(diagnosis);
        }

        // POST: NDDiagnosis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosis = await _context.Diagnosis.FindAsync(id);
            _context.Diagnosis.Remove(diagnosis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiagnosisExists(int id)
        {
            return _context.Diagnosis.Any(e => e.DiagnosisId == id);
        }
    }
}
