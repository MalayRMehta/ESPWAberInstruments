using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using ESPW.Model;
namespace ESPW.Pages.Clients
{
    public class CreateModel : PageModel
    {
        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
        }

        public void OnPost() { 
        
            clientInfo.name = Request.Form["name"];
            clientInfo.doses = Request.Form["doses"];
            clientInfo.time = Request.Form["time"];
            clientInfo.mass = Request.Form["mass"];

            if (clientInfo.name.Length == 0 || clientInfo.doses.Length == 0 ||
                clientInfo.time.Length == 0 || clientInfo.mass.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }
            
            try
            {
                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=AberInstruments;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO clients " +
                                 "(name,doses,time,mass) VALUES " +
                                 "(@name,@doses,@time,@mass);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", clientInfo.name);
						command.Parameters.AddWithValue("@doses", clientInfo.doses);
						command.Parameters.AddWithValue("@time", clientInfo.time);
						command.Parameters.AddWithValue("@mass", clientInfo.mass);

                        var result = command.ExecuteNonQuery();
					}
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            clientInfo.name = ""; clientInfo.doses = ""; clientInfo.time = ""; clientInfo.mass = "";
            successMessage = "New Client Added Correctly";

            Response.Redirect("/Clients/Index");
        }
    }
}
