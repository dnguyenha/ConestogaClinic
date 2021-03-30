using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NDPatients.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NDPatients.Controllers
{
    // Controller for PatientDiagnosis model
    public class NDPatientDiagnosisController : Controller
    {
        private readonly PatientsContext _context;

        public NDPatientDiagnosisController(PatientsContext context)
        {
            // context is provided by Dependency Injection defined in Startup.cs
            _context = context;
        }

        // GET: NDPatientDiagnosis
        // Default view when user go to NDPatientDiagnosis controller is Index. The Url for the view would be NDPatientDiagnosis/
        public async Task<IActionResult> Index(int patientId)
        {
            //URL check
            if (patientId != 0)
            {
                HttpContext.Session.SetInt32("PatientId", patientId);
            }
            //QueryString check
            else if (Request.Query["patientId"].Any())
            {
                HttpContext.Session.SetInt32("PatientId", int.Parse(Request.Query["patientId"]));
            }
            //Session check
            else if (HttpContext.Session.GetInt32("PatientId") != null)
            {
                patientId = (int)HttpContext.Session.GetInt32("PatientId");
            }
            else
            {
                TempData["message"] = "Please select a patient!";
                return RedirectToAction("Index", "NDPatient");
            }

            //patientId to be display on the View
            ViewData["PatientId"] = patientId;

            //Get patient's Full Name as: Last Name, First Name
            var aPatient = _context.Patient.Where(p => p.PatientId == patientId).FirstOrDefault();
            ViewData["PatientFullName"] = aPatient.LastName + ", " + aPatient.FirstName;

            //Get PatientDiagnosis list of a seclected patientId
            var patientsContext = _context.PatientDiagnosis
                                    .Include(p => p.Diagnosis)
                                    .Include(p => p.Patient)
                                    .Where(p => p.PatientId == patientId)
                                    .OrderBy(p => p.Patient.LastName + ", " + p.Patient.FirstName)
                                    .ThenByDescending(p => p.PatientDiagnosisId);

            return View(await patientsContext.ToListAsync());
        }

        // GET: NDPatientDiagnosis/Details/5
        // Details view would be shown. The Url for the view would be NDPatientDiagnosis/Details/<id>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // GET: NDPatientDiagnosis/Create
        // Create view would be shown. The Url for the view would be NDPatientDiagnosis/Create
        // Empty Create view is shown
        public IActionResult Create()
        {
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName");
            return View();
        }

        // POST: NDPatientDiagnosis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This action is called when user clicks on the submit button of Create view
        // PatientDiagnosis object is the parameter which holds the values which user has given on the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientDiagnosis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: NDPatientDiagnosis/Edit/5
        // Edit view for a particular item would be shown. The Url for the view would be NDPatientDiagnosis/Edit/<id>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis.FindAsync(id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // POST: NDPatientDiagnosis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This action is called when user clicks on the submit button of Edit view
        // PatientDiagnosis object is the parameter which holds the values which user has given on the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (id != patientDiagnosis.PatientDiagnosisId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientDiagnosis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientDiagnosisExists(patientDiagnosis.PatientDiagnosisId))
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
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: NDPatientDiagnosis/Delete/5
        // Delete view for a particular item would be shown. The Url for the view would be NDPatientDiagnosis/Delete/<id>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // POST: NDPatientDiagnosis/Delete/5
        // This action is called when user clicks on the submit button of Delete view
        // id is the parameter which hold the key of the item will be deleted
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientDiagnosis = await _context.PatientDiagnosis.FindAsync(id);
            _context.PatientDiagnosis.Remove(patientDiagnosis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientDiagnosisExists(int id)
        {
            return _context.PatientDiagnosis.Any(e => e.PatientDiagnosisId == id);
        }
    }
}
