using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LearningContext _context;
        private readonly PaymentService _paymentService;
        public HomeController(LearningContext context, PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }
        int? userId; // Declare userId as nullable int to handle conversion

        public IActionResult Index()
        {
            
            if (TempData.ContainsKey("userId"))
            {
                var userIdString = TempData["userId"]?.ToString(); // Use null conditional operator to handle null value
                if (!string.IsNullOrEmpty(userIdString))
                {
                    string use = HttpContext.Session.GetString("UserId");
                    userId = Convert.ToInt32(use); // Convert to int if not null or empty
                }
            }

            // Use userId as needed, handle null value appropriately

            var data = _context.Courses.ToList();
            return View(data);
        }

        public async Task<IActionResult> Content(int id)
        {

            var enrollment = _context.Enrollments.FirstOrDefault(e => e.UserId == userId && e.CourseId == id);

            if (enrollment != null && enrollment.PaymentId != null)
            {
                var data = _context.Contents.Where(m => m.Id == id).ToList();
                return View(data);
            }
            else
            {
                string paymentId = await _paymentService.CreatePayPalPaymentAsync("49"); // Pass the amount or use a predefined amount

                if (!string.IsNullOrEmpty(paymentId))
                {
                    if (enrollment == null)
                    {
                        enrollment = new Enrollment {
                            
                            UserId = userId, CourseId = id, PaymentId = paymentId };
                        _context.Enrollments.Add(enrollment);
                    }
                    else
                    {
                        enrollment.PaymentId = paymentId;
                    }
                    await _paymentService.StorePaymentDetailsAsync(paymentId, "Your-Payer-Id"); // Pass the payer ID if available
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Content", new { id = id });
                }
                else
                {
                    return RedirectToAction("PaymentError", "Payment");
                }
            }
        }


        /*private string ProcessPayment(int userId, int courseId)
        {

            // Simulate payment processing and return a PaymentId
            // In a real-world scenario, this method would interact with a payment gateway or service
            // For demonstration purposes, let's generate a random PaymentId
            return Guid.NewGuid().ToString(); // Generate a unique PaymentId
        }*/


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
