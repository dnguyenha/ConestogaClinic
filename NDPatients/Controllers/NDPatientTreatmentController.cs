using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NDPatients.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NDPatients.Controllers
{
    // Controller for PatientTreatment model
    public class NDPatientTreatmentController : Controller
    {
        private readonly PatientsContext _context;

        public NDPatientTreatmentController(PatientsContext context)
        {
            // context is provided by Dependency Injection defined in Startup.cs
            _context = context;
        }

        // GET: NDPatientTreatment
        // Default view when user go to NDCountry controller is Index. The Url for the view would be NDPatientTreatment/
        public async Task<IActionResult> Index(int patientDiagnosisId)
        {
            // URL check
            if (patientDiagnosisId != 0)
            {
                // Save patientDiagnosisId to session
                HttpContext.Session.SetInt32("PatientDiagnosisId", patientDiagnosisId);
            }
            // QueryString check
            else if (Request.Query["patientDiagnosisId"].Any())
            {
                // Save QueryStirng of patientDiagnosisId to session
                HttpContext.Session.SetInt32("PatientDiagnosisId", int.Parse(Request.Query["patientDiagnosisId"]));
            }
            // Session check
            else if (HttpContext.Session.GetInt32("PatientDiagnosisId") != null)
            {
                // Get patientDiagnosisId from session
                patientDiagnosisId = (int)HttpContext.Session.GetInt32("PatientDiagnosisId");
            }
            else
            {
                TempData["message"] = "Please select a patient diagnosis!";
                return RedirectToAction("Index", "NDPatientDiagnosis");
            }

            // Save patientDiagnosisName from QueryString to session
            if (Request.Query["patientDiagnosisName"].Any())
            {
                HttpContext.Session.SetString("PatientDiagnosisName", Request.Query["patientDiagnosisName"]);
            }

            // Save patientFullName from QueryString to session
            if (Request.Query["patientFullName"].Any())
            {
                HttpContext.Session.SetString("PatientFullName", Request.Query["patientFullName"]);
            }

            // Get PatientDiagnosisName from session and send it to View
            if (HttpContext.Session.GetString("PatientDiagnosisName") != null)
            {
                ViewData["PatientDiagnosisName"] = HttpContext.Session.GetString("PatientDiagnosisName").ToString();
            }

            // Get PatientFullName from session and send to View
            if (HttpContext.Session.GetString("PatientFullName") != null)
            {
                ViewData["PatientFullName"] = HttpContext.Session.GetString("PatientFullName").ToString();
            }

            // Get PatientTreatment for selected patient, sort by patientDiagnosisId, then by most-recent-diagnosis for each patient
            var patientsContext = _context.PatientTreatment
                                    .Include(p => p.PatientDiagnosis)
                                    .Include(p => p.Treatment)
                                    .Where(p => p.PatientDiagnosisId == patientDiagnosisId)
                                    .OrderByDescending(p => p.DatePrescribed);

            return View(await patientsContext.ToListAsync());
        }

        // GET: NDPatientTreatment/Details/5
        // Details view would be shown. The Url for the view would be NDPatientTreatment/Details/<id>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Check PatientDiagnosisId, PatientDiagnosisName, PatientFullName from session
            if (HttpContext.Session.GetString("PatientDiagnosisName") == null ||
                HttpContext.Session.GetString("PatientFullName") == null)
            {
                TempData["message"] = "Please select a patient diagnosis!";
                return RedirectToAction("Index", "NDPatientDiagnosis");
            }

            // Get PatientDiagnosisName from session and send it to View
            ViewData["PatientDiagnosisName"] = HttpContext.Session.GetString("PatientDiagnosisName").ToString();

            // Get PatientFullName from session and send to View
            ViewData["PatientFullName"] = HttpContext.Session.GetString("PatientFullName").ToString();

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);

            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // GET: NDPatientTreatment/Create
        // Create view would be shown. The Url for the view would be NDPatientTreatment/Create
        // Empty Create view is shown
        public IActionResult Create()
        {
            // Check PatientDiagnosisId, PatientDiagnosisName, PatientFullName from session
            if (HttpContext.Session.GetInt32("PatientDiagnosisId") == null ||
                HttpContext.Session.GetString("PatientDiagnosisName") == null ||
                HttpContext.Session.GetString("PatientFullName") == null)
            {
                TempData["message"] = "Please select a patient diagnosis!";
                return RedirectToAction("Index", "NDPatientDiagnosis");
            }

            // Get PatientDiagnosisId from session
            var patientDiagnosisId = (int)HttpContext.Session.GetInt32("PatientDiagnosisId");

            // Get PatientDiagnosisName from session and send it to View
            ViewData["PatientDiagnosisName"] = HttpContext.Session.GetString("PatientDiagnosisName").ToString();

            // Get PatientFullName from session and send to View
            ViewData["PatientFullName"] = HttpContext.Session.GetString("PatientFullName").ToString();

            // Get only Treatment for selected Diagnosis and send to View
            var treatment = _context.Treatment
                            .Include(a => a.Diagnosis)
                            .Include(a => a.PatientTreatment)
                            .Where(a => a.Diagnosis.Name == HttpContext.Session.GetString("PatientDiagnosisName").ToString());
            ViewData["TreatmentId"] = new SelectList(treatment, "TreatmentId", "Name");

            return View();
        }

        // POST: NDPatientTreatment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This action is called when user clicks on the submit button of Create view
        // PatientTreatment object is the parameter which holds the values which user has given on the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            // Get PatientDiagnosisId from session
            patientTreatment.PatientDiagnosisId = (int)HttpContext.Session.GetInt32("PatientDiagnosisId");

            // Add current time to DatePrescribed
            patientTreatment.DatePrescribed = DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(patientTreatment);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "New treatment for patient added successfully!";

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
                TempData["message"] = ex.GetBaseException().Message;
            }

            // Get only Treatment for selected Diagnosis and send to View
            var treatment = _context.Treatment
                            .Include(a => a.Diagnosis)
                            .Include(a => a.PatientTreatment)
                            .Where(a => a.Diagnosis.Name == HttpContext.Session.GetString("PatientDiagnosisName").ToString());
            ViewData["TreatmentId"] = new SelectList(treatment, "TreatmentId", "Name");

            return View(patientTreatment);
        }

        // GET: NDPatientTreatment/Edit/5
        // Edit view for a particular item would be shown. The Url for the view would be NDPatientTreatment/Edit/<id>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            // Get PatientDiagnosisId, PatientDiagnosisName, PatientFullName from session
            if (HttpContext.Session.GetString("PatientDiagnosisName") == null ||
                HttpContext.Session.GetString("PatientFullName") == null)
            {
                TempData["message"] = "Please select a patient diagnosis!";
                return RedirectToAction("Index", "NDPatientDiagnosis");
            }

            // Get PatientDiagnosisName from session and send it to View
            ViewData["PatientDiagnosisName"] = HttpContext.Session.GetString("PatientDiagnosisName").ToString();

            // Get PatientFullName from session and send to View
            ViewData["PatientFullName"] = HttpContext.Session.GetString("PatientFullName").ToString();

            // Get only Treatment for selected Diagnosis and send to View
            var treatment = _context.Treatment
                            .Include(a => a.Diagnosis)
                            .Include(a => a.PatientTreatment)
                            .Where(a => a.Diagnosis.Name == HttpContext.Session.GetString("PatientDiagnosisName").ToString());
            ViewData["TreatmentId"] = new SelectList(treatment, "TreatmentId", "Name");

            return View(patientTreatment);
        }

        // POST: NDPatientTreatment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This action is called when user clicks on the submit button of Edit view
        // PatientTreatment object is the parameter which holds the values which user has given on the view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            if (id != patientTreatment.PatientTreatmentId)
            {
                return NotFound();
            }

            // Get PatientDiagnosisId from session since it's deleted from View
            patientTreatment.PatientDiagnosisId = (int)HttpContext.Session.GetInt32("PatientDiagnosisId");

            // Add current time to DatePrescribed
            patientTreatment.DatePrescribed = DateTime.Now;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientTreatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientTreatmentExists(patientTreatment.PatientTreatmentId))
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

            // Get only Treatment for selected Diagnosis and send to View
            var treatment = _context.Treatment
                            .Include(a => a.Diagnosis)
                            .Include(a => a.PatientTreatment)
                            .Where(a => a.Diagnosis.Name == HttpContext.Session.GetString("PatientDiagnosisName").ToString());
            ViewData["TreatmentId"] = new SelectList(treatment, "TreatmentId", "Name");

            return View(patientTreatment);
        }

        // GET: NDPatientTreatment/Delete/5
        // Delete view for a particular item would be shown. The Url for the view would be NDPatientTreatment/Delete/<id>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get PatientDiagnosisId, PatientDiagnosisName, PatientFullName from session
            if (HttpContext.Session.GetString("PatientDiagnosisName") == null ||
                HttpContext.Session.GetString("PatientFullName") == null)
            {
                TempData["message"] = "Please select a patient diagnosis!";
                return RedirectToAction("Index", "NDPatientDiagnosis");
            }

            // Get PatientDiagnosisName from session and send it to View
            ViewData["PatientDiagnosisName"] = HttpContext.Session.GetString("PatientDiagnosisName").ToString();

            // Get PatientFullName from session and send to View
            ViewData["PatientFullName"] = HttpContext.Session.GetString("PatientFullName").ToString();

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);

            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // POST: NDPatientTreatment/Delete/5
        // This action is called when user clicks on the submit button of Delete view
        // id is the parameter which hold the key of the item will be deleted
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            _context.PatientTreatment.Remove(patientTreatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientTreatmentExists(int id)
        {
            return _context.PatientTreatment.Any(e => e.PatientTreatmentId == id);
        }
    }
}
