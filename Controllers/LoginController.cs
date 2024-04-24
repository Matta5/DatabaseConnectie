using DatabaseConnectie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace DatabaseConnectie.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = "Server = Mathijs\\MSSQLSERVER02; Database=AlbumExchange;User Id=test;Password=test;TrustServerCertificate=True;Encrypt=False;Trusted_Connection=true;";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "SELECT COUNT(*) FROM [User] WHERE username = '@Username' AND password = '@Password'";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Username", model.username);
                        cmd.Parameters.AddWithValue("@Password", model.password);

                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        conn.Close();

                        if (count > 0)
                        {
                            // Authentication successful
                            // Redirect to a dashboard or home page
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // Authentication failed
                            ModelState.AddModelError(string.Empty, "Invalid username or password");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    ModelState.AddModelError(string.Empty, $"An error occurred while processing your request: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
            return View("Index", model);
        }
    }
}
