using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment_04.Models;

namespace Assignment_04.Controllers
{
    public class InsuranceQuotesController : Controller
    {
        private readonly InsuranceContext _context;

        public InsuranceQuotesController(InsuranceContext context)
        {
            _context = context;
        }

        // GET: InsuranceQuotes
        public async Task<IActionResult> Index()
        {
            var insuranceContext = _context.InsuranceQuotes.Include(i => i.Customer);
            return View(await insuranceContext.ToListAsync());
        }

        // GET: InsuranceQuotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceQuote = await _context.InsuranceQuotes
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceQuote == null)
            {
                return NotFound();
            }

            return View(insuranceQuote);
        }

        // GET: InsuranceQuotes/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            return View();
        }

        // POST: InsuranceQuotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: InsuranceQuotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId")] InsuranceQuote insuranceQuote)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the customer associated with this quote
                var customer = await _context.Customers.FindAsync(insuranceQuote.CustomerId);
                if (customer == null)
                {
                    return NotFound("Customer not found.");
                }

                // Initialize the base price
                decimal monthlyQuote = 50m;

                // Apply age-based charges
                int age = DateTime.Now.Year - customer.DateOfBirth.Year;
                if (customer.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--; // Adjust for birth date
                if (age <= 18)
                {
                    monthlyQuote += 100;
                }
                else if (age >= 19 && age <= 25)
                {
                    monthlyQuote += 50;
                }
                else if (age > 25)
                {
                    monthlyQuote += 25;
                }

                // Apply car year-based charges
                if (customer.CarYear < 2000)
                {
                    monthlyQuote += 25;
                }
                else if (customer.CarYear > 2015)
                {
                    monthlyQuote += 25;
                }

                // Apply car make/model-based charges
                if (customer.CarMake == "Porsche")
                {
                    monthlyQuote += 25;
                    if (customer.CarModel == "911 Carrera")
                    {
                        monthlyQuote += 25; // Additional charge for specific model
                    }
                }

                // Add charges for speeding tickets
                monthlyQuote += customer.SpeedingTickets * 10;

                // Apply DUI-based charge
                if (customer.HasDUI)
                {
                    monthlyQuote *= 1.25m;
                }

                // Apply full coverage charge
                if (customer.IsFullCoverage)
                {
                    monthlyQuote *= 1.5m;
                }

                // Set the calculated quote
                insuranceQuote.MonthlyQuote = monthlyQuote;

                // Add the new insurance quote to the database and save changes
                _context.Add(insuranceQuote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, reload the customer list for the dropdown and show the view again
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", insuranceQuote.CustomerId);
            return View(insuranceQuote);
        }

        // GET: InsuranceQuotes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceQuote = await _context.InsuranceQuotes.FindAsync(id);
            if (insuranceQuote == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", insuranceQuote.CustomerId);
            return View(insuranceQuote);
        }

        // POST: InsuranceQuotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,MonthlyQuote")] InsuranceQuote insuranceQuote)
        {
            if (id != insuranceQuote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceQuote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceQuoteExists(insuranceQuote.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", insuranceQuote.CustomerId);
            return View(insuranceQuote);
        }

        // GET: InsuranceQuotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceQuote = await _context.InsuranceQuotes
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceQuote == null)
            {
                return NotFound();
            }

            return View(insuranceQuote);
        }

        // POST: InsuranceQuotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuranceQuote = await _context.InsuranceQuotes.FindAsync(id);
            if (insuranceQuote != null)
            {
                _context.InsuranceQuotes.Remove(insuranceQuote);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceQuoteExists(int id)
        {
            return _context.InsuranceQuotes.Any(e => e.Id == id);
        }
    }
}
