using FinstarTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace FinstarTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyObjectsController : ControllerBase
    {
        private readonly ILogger<MyObjectsController> _logger;
        private readonly IConfiguration _configuration;

        public MyObjectsController(ILogger<MyObjectsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddMyObjects(List<Dictionary<string, string>> data)
        {
            if (data == null)
            {
                return BadRequest();
            }

            List<MyObject> myObjects = ParseToMyObjectsList(data);
            await CreateOrClearTable();

            myObjects.ForEach(async myObject =>
            {
                await AddObjectToDB(myObject);

            });

            return Ok();
        }

        //Из-за требуемого формата для JSON данные принимаются как массив словарей с одной парой,
        //поэтому приходится парсить его в список экземпляров класса для дальнейшей нормальной работы
        private List<MyObject> ParseToMyObjectsList(List<Dictionary<string, string>> data)
        {
            List<MyObject> myObjects = new List<MyObject>();
            int id = 0;
            data = data.OrderBy(x => int.Parse(x.First().Key)).ToList();
            data.ForEach(x =>
                {
                    myObjects.Add(new MyObject
                    {
                        Id = ++id,
                        Code = int.Parse(x.First().Key),
                        Value = x.First().Value
                    });
                }
            );
            return myObjects;
        }

        private async Task CreateOrClearTable()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
                    END
                    ELSE
                    BEGIN
                        DELETE FROM Objects
                    END
                    ";
                var command = new SqlCommand(query, sqlConnection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task AddObjectToDB(MyObject myObject)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                var query = $"INSERT INTO Objects(Id, Code, Value) VALUES({myObject.Id}, {myObject.Code}, '{myObject.Value}')";
                var command = new SqlCommand(query, sqlConnection);
                await command.ExecuteNonQueryAsync();
            }
        }


        [HttpGet("")]
        public ActionResult GetMyObjects([FromQuery] int? id, [FromQuery] int? code, [FromQuery] string? value)
        {
            List<MyObject> myObjects = GetObjectsFromDB(id, code, value);
            return Ok(myObjects);
        }

        private List<MyObject> GetObjectsFromDB(int? id, int? code, string? value)
        {
            List<MyObject> myObjects = new List<MyObject>();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                string query = @"SELECT Id, Code, Value 
                    FROM Objects 
                    WHERE 1=1";
                query += id != null ? $" AND Id = @Id" : "";
                query += code != null ? $" AND Code = @Code" : "";
                query += !value.IsNullOrEmpty() ? $" AND Value = @Value" : "";
                query += " ORDER by Id";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Code", code ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Value", value.IsNullOrEmpty() ? (object)DBNull.Value : value);
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
            return myObjects;
        }
    }
}