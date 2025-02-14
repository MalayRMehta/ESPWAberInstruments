using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using ESPW.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web; // For Page class if using Web Forms
using Newtonsoft.Json;  // This is the essential using statement

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

        public async void OnPost()
        {

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



			try
			{
				Guid guid = new Guid();
				ClientInfo obj = new ClientInfo();
                obj.id = guid.ToString();
				obj.time = Request.Form["time"];
				obj.mass = Request.Form["mass"];
                obj.created_at = DateTime.Now.ToString();
                obj.name = "callToWebapi";
                obj.doses = Request.Form["doses"];
                //change url from webapi manually
				string response = await PostDataAsync("http://localhost:5046/ESPW/", obj); // Replace with your API endpoint

				// 3. Handle the Response
				var result = "API Response: " + response; // Display in a label (Web Forms)
															

			}
			catch (Exception ex)
			{
                throw ex;
			}			

        }

		private async Task<string> PostDataAsync(string apiUrl, ClientInfo data)
		{
			using (HttpClient client = new HttpClient())
			{				
				// Serialize the data to JSON
				string json = Newtonsoft.Json.JsonConvert.SerializeObject(data); // Requires Newtonsoft.Json NuGet package
				var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

				// Make the POST request
				HttpResponseMessage response = await client.PostAsync(apiUrl, content);

				// Check for successful response
				if (response.IsSuccessStatusCode)
				{
					// Read and return the response content
					return await response.Content.ReadAsStringAsync();
				}
				else
				{
					// Handle error (e.g., throw an exception)
					string errorContent = await response.Content.ReadAsStringAsync();
					throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
				}
			}

		}

	}


    
}
