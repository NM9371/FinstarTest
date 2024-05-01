using FinstarTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FinstarTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyObjectsController : ControllerBase
    {

        private readonly ILogger<MyObjectsController> _logger;

        public MyObjectsController(ILogger<MyObjectsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddMyObjects(List<Dictionary<string, string>> myObjects)
        {
            if (myObjects == null)
            {
                return BadRequest();
            }

            var sqlManager = new SqlConnectionManager();
            using (SqlConnection sqlConnection = sqlManager.OpenSqlConnection())
            {
                sqlConnection.Open();
                string query = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Objects')
                    BEGIN
	                    CREATE TABLE [dbo].[Objects](
		                    [Id] [int] NULL,
		                    [Code] [int] NULL,
		                    [Value] [nvarchar](max) NULL
	                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                    END;
                    ";
                var command = new SqlCommand(query, sqlConnection);
                await command.ExecuteNonQueryAsync();

                query = "DELETE FROM Objects";
                command = new SqlCommand(query, sqlConnection);
                await command.ExecuteNonQueryAsync();

                int id = 0;
                foreach (Dictionary<string, string> myObject in myObjects)
                {
                    query = $"INSERT INTO Objects(Id, Code, Value) VALUES({++id}, {myObject.First().Key}, '{myObject.First().Value}')";
                    command = new SqlCommand(query, sqlConnection);
                    await command.ExecuteNonQueryAsync();
                };

            }
            return Ok();
        }

        [HttpGet("")]
        public async Task<ActionResult> GetMyObjects()
        {
            Console.WriteLine("hello");
            List<MyObject> myObjects = new List<MyObject>();
            var sqlManager = new SqlConnectionManager();
            using (SqlConnection sqlConnection = sqlManager.OpenSqlConnection())
            {
                sqlConnection.Open();
                string query = "SELECT Id, Code, Value FROM Objects ORDER by Id";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            myObjects.Add(new MyObject
                            {
                                Id = reader.GetInt32(0),
                                Code = reader.GetInt32(1),
                                Value = reader.GetString(2)
                            });
                        }
                    }
                }

            }

            return Ok(myObjects);
        }
   }
}