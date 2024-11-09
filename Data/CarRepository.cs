using MySql.Data.MySqlClient;
using CarManagement.Models;
using System.Data;

namespace CarManagement.Data
{
    public class CarRepository
    {
        private readonly string _connectionString;
        public CarRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MysqlDbConnection");

            // Kiểm tra kết nối
            using (var conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    Console.WriteLine("Success");
                }  
                catch (Exception ex)
                {
                    Console.WriteLine("Err " + ex);
                }
            }
        }
        public string CreateID()
        {
            string alphaBet = "ABCDEFGHIJKLMNOPQRSTUVWSYZabcdefghijklmnopqrstuvwsyz0123456789";
            string id = this.GetRandomString(alphaBet, 6) + DateTime.Today.ToString("dd") + this.GetRandomString(alphaBet, 6) + DateTime.Today.ToString("MM") + this.GetRandomString(alphaBet, 4);
            return id;
        }
        public string GetRandomString(string alphaBet, int stringLenght)
        {
            string randomString = string.Empty;

            Random random = new Random();

            for (int i = 0; i < stringLenght; i++)
            {
                int randomIndex = random.Next(alphaBet.Length);
                randomString += alphaBet[randomIndex];
            }

            return randomString;
        }
        public async Task<List<Car>> GetCarsAsync(int page, int pageSize)
        {
            List<Car> cars = new List<Car>();

            int offset = (page - 1) * pageSize;

            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand
                {
                    // lấy số lượng PageSize các data từ vị trí Offset 
                    CommandText = "SELECT * FROM loai_xe LIMIT @PageSize OFFSET @Offset",
                    Connection = conn
                };

                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", offset);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cars.Add(new Car
                        {
                            id = reader.GetString("id"),
                            phienban = reader.GetString("phienban"),
                            gia = reader.GetDouble("gia"),
                            mau_id = reader.GetString("mau_id"),
                            dongco_id = reader.GetString("dongco_id"),
                            kichthuoc_id = reader.GetString("kichthuoc_id")
                        });
                    }
                }
            }

            return cars;
        }
        public async Task<int> GetTotalCarsAsync()
        {
            int total = 0;

            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "SELECT count(*) from loai_xe",
                    Connection = conn
                };

                total = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            };

            return total;
        }
        public async Task<Car> GetHighestPriceCarAsync()
        {
            Car car = new Car();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "Select * FROM loai_xe WHERE gia = (SELECT MAX(gia) FROM loai_xe)",
                    Connection = conn
                };

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Car
                        {
                            id = reader.GetString("id"),
                            phienban = reader.GetString("phienban"),
                            gia = reader.GetDouble("gia"),
                            mau_id = reader.GetString("mau_id"),
                            dongco_id = reader.GetString("dongco_id"),
                            kichthuoc_id = reader.GetString("kichthuoc_id")
                        };
                    }
                }
            }
            return car;
        }
        public async Task<List<Car>> GetAllCar()
        {
            List<Car> cars = new List<Car>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "Select * from loai_xe",
                    Connection = conn
                };

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cars.Add(new Car
                        {
                            id = reader.GetString("id"),
                            phienban = reader.GetString("phienban"),
                            gia = reader.GetDouble("gia"),
                            mau_id = reader.GetString("mau_id"),
                            dongco_id = reader.GetString("dongco_id"),
                            kichthuoc_id = reader.GetString("kichthuoc_id")
                        });
                    }
                }
            }
            return cars;
        }
        public async Task CreateAsync(Car car)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "INSERT INTO loai_xe (id, phienban, gia, mau_id, dongco_id, kichthuoc_id) VALUES (@id, @phienban, @gia, @mau_id ,@dongco_id, @kichthuoc_id)",
                    Connection = conn
                };

                // Thêm các tham số vào
                cmd.Parameters.AddWithValue("@id", this.CreateID());
                cmd.Parameters.AddWithValue("@phienban", car.phienban);
                cmd.Parameters.AddWithValue("@gia", car.gia);
                cmd.Parameters.AddWithValue("@mau_id", car.mau_id);
                cmd.Parameters.AddWithValue("@dongco_id", car.dongco_id);
                cmd.Parameters.AddWithValue("@kichthuoc_id", car.kichthuoc_id);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateCarAsync (Car car)
        {
            using (var conn = new MySqlConnection(_connectionString))
            { 
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "UPDATE loai_xe SET phienban = @Phienban, gia = @Gia, mau_id = @MauId, dongco_id = @DongcoId, kichthuoc_id = @KichthuocId WHERE id = @Id",
                    Connection = conn
                };

                cmd.Parameters.AddWithValue("@Phienban", car.phienban);
                cmd.Parameters.AddWithValue("@Gia", car.gia);
                cmd.Parameters.AddWithValue("@MauId", car.mau_id);
                cmd.Parameters.AddWithValue("@DongcoId", car.dongco_id);
                cmd.Parameters.AddWithValue("@KichthuocId", car.kichthuoc_id);
                cmd.Parameters.AddWithValue("@Id", car.id);

                await cmd.ExecuteNonQueryAsync();
            }    
        }
        public async Task DeleteCarAsync (Car car)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand
                { 
                    CommandText = "DELETE FROM loai_xe WHERE id=@id",
                    Connection= conn
                };

                cmd.Parameters.AddWithValue("@id", car.id);

                await cmd.ExecuteNonQueryAsync();
            }    
        }
    }
}
