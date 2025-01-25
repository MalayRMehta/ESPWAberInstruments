using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ESPW.Model;
namespace ESPW.Pages.Clients
{
    public class EditModel : PageModel
    {
        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
				String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=AberInstruments;Integrated Security=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					String sql = "SELECT * FROM clients WHERE id = @id";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@id", id);
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								clientInfo.id = "" + reader.GetInt32(0);
								clientInfo.name = reader.GetString(1);
								clientInfo.doses = reader.GetString(2);
								clientInfo.time = reader.GetString(3);
								clientInfo.mass = reader.GetString(4);
							}
						}
					}
				}
			}
            catch (Exception ex) { 
                errorMessage = ex.Message;
            }
        }

        public void OnPost() {
			clientInfo.id = Request.Form["id"];
			clientInfo.name = Request.Form["name"];
			clientInfo.doses = Request.Form["doses"];
			clientInfo.time = Request.Form["time"];
			clientInfo.mass = Request.Form["mass"];

			if (clientInfo.id.Length == 0 || clientInfo.name.Length == 0 || clientInfo.doses.Length == 0 ||
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
					String sql = "UPDATE clients " +
								 " SET name = @name, doses = @doses, time = @time, mass = @mass " +
								 " WHERE id = @id";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@name", clientInfo.name);
						command.Parameters.AddWithValue("@doses", clientInfo.doses);
						command.Parameters.AddWithValue("@time", clientInfo.time);
						command.Parameters.AddWithValue("@mass", clientInfo.mass);
						command.Parameters.AddWithValue("@id", clientInfo.id);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return;
			}

			Response.Redirect("/Clients/Index");
		}
    }
}
