using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NDPatients.Models;

namespace NDPatients.Controllers
{
    public class NDMedicationController : Controller
    {
        private readonly PatientsContext _context;

        public NDMedicationController(PatientsContext context)
        {
            // context is provided by Dependency Injection defined in Startup.cs
            _context = context;
        }

        // GET: NDMedication
        // Default view when user go to NDMedication controller is Index
        public async Task<IActionResult> Index(int? medicationTypeId, string? medicationTypeName)
        {
            // If medicationTypeId is passed to the URL (NDMedication/Index?medicationTypeId=<id>),
            // display list of Medications which is filtered by medicationTypeId.
            if (medicationTypeId != null)
            {
                // Save medicationTypeId and medicationTypeName to session variables
                HttpContext.Session.SetInt32("MedicationTypeId", (int)medicationTypeId);
                HttpContext.Session.SetString("MedicationTypeName", (string)medicationTypeName);

                // Filter the list of Medications with medicationTypeId, 
                // sort by Name and then by Concentration within Name
                var medications = _context.Medication.Include(m => m.ConcentrationCodeNavigation)
                                                    .Include(m => m.DispensingCodeNavigation)
                                                    .Include(m => m.MedicationType)
                                                    .Where(m => m.MedicationTypeId == medicationTypeId)
                                                    .OrderBy(m => m.Name)
                                                    .ThenBy(m => m.Concentration);

                return View(await medications.ToListAsync());
            }
            else // No medicationTypeId is passed in the URL
            {
                // Retrieve old session variable
                int MedicationTypeId = Convert.ToInt32(HttpContext.Session.GetInt32("MedicationTypeId"));

                // Check if session variable exists
                if (MedicationTypeId == 0)
                {
                    // There is no MedicationTypeId saved in the session, return to NDMedicationType controller with error message
                    return RedirectToAction("Index", "NDMedicationType", 
                        TempData["MedicationTypeIdError"] = "Please select a Medication Type to view its Medications!");
                }
                else
                {
                    // MedicationTypeId exists, display the list of Medications for MedicationTypeId saved in the session,
                    // sort by Name and then by Concentration within Name
                    var medications = _context.Medication.Include(m => m.ConcentrationCodeNavigation)
                                                         .Include(m => m.DispensingCodeNavigation)
                                                         .Include(m => m.MedicationType)
                                                         .Where(m => m.MedicationTypeId == MedicationTypeId)
                                                         .OrderBy(m => m.Name)
                                                         .ThenBy(m => m.Concentration);

                    return View(await medications.ToListAsync());
                }
            }
        }

        // GET: NDMedication/Details/5
        // Details view would be shown. The Url for the view would be NDMedication/Details/<id>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);

            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // GET: NDMedication/Create
        // Create view would be shown. The Url for the view would be NDMedication/Create
        // Empty Create view is shown
        public IActionResult Create()
        {
            // Retrieve session variable
            int MedicationTypeId = Convert.ToInt32(HttpContext.Session.GetInt32("MedicationTypeId"));

            // Check if session variable exists
            if (MedicationTypeId == 0)
            {
                // There is no MedicationTypeId saved in the session, return to NDMedicationType controller with error message
                return RedirectToAction("Index", "NDMedicationType",
                    TempData["MedicationTypeIdError"] = "Please select a Medication Type to view its Medications!");
            }
            else
            {
                // In ConcentrationCode drop down, display it with ascending order
                ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(a => a.ConcentrationCode), 
                                                "ConcentrationCode", "ConcentrationCode");
                // In DispensingCode drop down, display it with ascending order
                ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(a => a.DispensingCode), 
                                                "DispensingCode", "DispensingCode");

                return View();
            }
        }

        // POST: NDMedication/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This action is called when user clicks on the submit button of Create view
        // Medication object is the parameter which holds the values which user has given on the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (ModelState.IsValid)
            {
                // Do not allow user to create a duplication of Name, Concentration and ConcentrationCode medication
                var duplicatedMed = _context.Medication
                                    .Include(m => m.ConcentrationCodeNavigation)
                                    .Include(m => m.DispensingCodeNavigation)
                                    .Include(m => m.MedicationType)
                                    .FirstOrDefault(a => a.Name == medication.Name 
                                        && a.Concentration == medication.Concentration 
                                        && a.ConcentrationCode == medication.ConcentrationCode);

                if (duplicatedMed != null)
                {
                    TempData["DuplicatedMedication"] = "Medication exists (same Name, Concentration and ConcentrationCode)!";
                }
                else
                {
                    _context.Add(medication);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);

            return View(medication);
        }

        // GET: NDMedication/Edit/5
        // Edit view for a particular item would be shown. The Url for the view would be NDMedication/Edit/<id>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }

            // In ConcentrationCode drop down, display it with ascending order
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(a => a.ConcentrationCode), 
                                            "ConcentrationCode", 
                                            "ConcentrationCode", 
                                            medication.ConcentrationCode);
            // In DispensingCode drop down, display it with ascending order
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(a => a.DispensingCode), 
                                        "DispensingCode", 
                                        "DispensingCode", 
                                        medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, 
                                            "MedicationTypeId", 
                                            "Name", 
                                            medication.MedicationTypeId);

            return View(medication);
        }

        // POST: NDMedication/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This action is called when user clicks on the submit button of Edit view
        // Medication object is the parameter which holds the values which user has given on the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (id != medication.Din)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
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

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);

            return View(medication);
        }

        // GET: NDMedication/Delete/5
        // Delete view for a particular item would be shown. The Url for the view would be NDMedication/Delete/<id>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);

            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // POST: NDMedication/Delete/5
        // This action is called when user clicks on the submit button of Delete view
        // id is the parameter which hold the key of the item will be deleted
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medication = await _context.Medication.FindAsync(id);
            _context.Medication.Remove(medication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(string id)
        {
            return _context.Medication.Any(e => e.Din == id);
        }
    }
}
