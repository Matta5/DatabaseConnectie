using DatabaseConnectie.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace DatabaseConnectie.Controllers
{
    public class UserController : Controller
    {
        // GET: UserController
        public ActionResult Index()
        {
            List<User> users = new List<User>();
            string connectionString = "Server = Mathijs\\MSSQLSERVER02; Database=AlbumExchange;User Id=test;Password=test;TrustServerCertificate=True;Encrypt=False;Trusted_Connection=true;";
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [User]", s);
                s.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User u = new User();
                        u.user_id = reader.GetInt32(0);
                        u.username = reader.GetString(1);
                        u.email = reader.GetString(2);
                        u.password = reader.GetString(3);
                        u.profile_picture = reader.GetString(4);
                        users.Add(u);
                    }
                }
                
            }

            return View(users);
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            User user = GetUserById(id);

            return View(user);
        }


        private User GetUserById(int id)
        {
            User user = null;
            string connectionString = "Server=Mathijs\\MSSQLSERVER02;Database=AlbumExchange;User Id=test;Password=test;TrustServerCertificate=True;Encrypt=False;Trusted_Connection=true;";

            using (SqlConnection s = new SqlConnection(connectionString))
            {
                s.Open();

                string selectQuery = "SELECT * FROM [User] WHERE user_id = @UserId";
                SqlCommand cmd = new SqlCommand(selectQuery, s);
                cmd.Parameters.AddWithValue("@UserId", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            user_id = reader.GetInt32(0),
                            username = reader.GetString(1),
                            email = reader.GetString(2),
                            password = reader.GetString(3)
                            // Add other properties as needed
                        };
                    }
                }
            }

            return user;
        }





    // GET: UserController/Create
    public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, IFormFile profilePicture)
        {
            try
            {
                // Check if a file has been uploaded
                if (profilePicture != null)
                {
                    // Check if the file is an image
                    var extension = Path.GetExtension(profilePicture.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".png")
                    {
                        ModelState.AddModelError("", "Invalid file type. Only JPG and PNG file types are allowed.");
                        return View(user);
                    }

                    // Save the profile picture to wwwroot/Images and get the file path
                    var fileName = Path.GetFileName(profilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    // Save the relative file path to the database
                    user.profile_picture = Path.Combine("Images", fileName);
                }

                // User creation logic
                string connectionString = "Server = Mathijs\\MSSQLSERVER02; Database=AlbumExchange;User Id=test;Password=test;TrustServerCertificate=True;Encrypt=False;Trusted_Connection=true;";
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    s.Open();
                    SqlCommand cmd;
                    if (user.profile_picture != null)
                    {
                        string insertQuery = "INSERT INTO [User] (username, email, password, profile_picture) VALUES (@Username, @Email, @Password, @ProfilePicture)";
                        cmd = new SqlCommand(insertQuery, s);
                        cmd.Parameters.AddWithValue("@ProfilePicture", user.profile_picture);
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO [User] (username, email, password) VALUES (@Username, @Email, @Password)";
                        cmd = new SqlCommand(insertQuery, s);
                    }
                    cmd.Parameters.AddWithValue("@Username", user.username);
                    cmd.Parameters.AddWithValue("@Email", user.email);
                    cmd.Parameters.AddWithValue("@Password", user.password);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }


        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            string connectionString = "Server=Mathijs\\MSSQLSERVER02;Database=AlbumExchange;User Id=test;Password=test;TrustServerCertificate=True;Encrypt=False;Trusted_Connection=true;";

            using (SqlConnection s = new SqlConnection(connectionString))
            {
                s.Open();
                string selectQuery = "SELECT username FROM [User] WHERE user_id = @UserId";
                SqlCommand cmd = new SqlCommand(selectQuery, s);
                cmd.Parameters.AddWithValue("@UserId", id);

                string currentUsername = null;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentUsername = reader.GetString(0);
                    }
                    else
                    {
                        return NotFound(); // User not found
                    }
                }

                User viewModel = new User
                {
                    user_id = id,
                    username = currentUsername
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User viewModel)
        {
            try
            {
                
                    string connectionString = "Server=Mathijs\\MSSQLSERVER02;Database=AlbumExchange;User Id=test;Password=test;TrustServerCertificate=True;Encrypt=False;Trusted_Connection=true;";

                    using (SqlConnection s = new SqlConnection(connectionString))
                    {
                        s.Open();
                        string updateQuery = "UPDATE [User] SET username = @Username WHERE user_id = @UserId";
                        SqlCommand cmd = new SqlCommand(updateQuery, s);
                        cmd.Parameters.AddWithValue("@Username", viewModel.username);
                        cmd.Parameters.AddWithValue("@UserId", id);
                        cmd.ExecuteNonQuery();
                    }

                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(viewModel);
            }
        }


        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                string connectionString = "Server=Mathijs\\MSSQLSERVER02;Database=AlbumExchange;User Id=test;Password=test;TrustServerCertificate=True;Encrypt=False;Trusted_Connection=true;";

                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    s.Open();
                    string deleteQuery = "DELETE FROM [User] WHERE user_id = @UserId";
                    SqlCommand cmd = new SqlCommand(deleteQuery, s);
                    cmd.Parameters.AddWithValue("@UserId", id);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return View("Error");
            }
        }


        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
