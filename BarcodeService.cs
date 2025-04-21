using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BanHang.API.Services
{
    public class BarcodeService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BarcodeService> _logger;
        private readonly string _connectionString;
        private readonly string _apiKey;
        private readonly string _apiBaseUrl;

        public BarcodeService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ILogger<BarcodeService> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _apiKey = _configuration["ExternalApis:BarcodeApi:ApiKey"];
            _apiBaseUrl = _configuration["ExternalApis:BarcodeApi:BaseUrl"];
        }

        /// <summary>
        /// Kiểm tra mã barcode trong cơ sở dữ liệu
        /// </summary>
        public async Task<SanPhamBarcodeInfo> CheckBarcodeInDatabaseAsync(string barcode, string userId = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        "SELECT Id_SanPham, TenSanPham, GiaBan, HinhAnh, MoTa, SoLuongTon, TrangThai " +
                        "FROM SanPhams WHERE Barcode = @Barcode", connection))
                    {
                        command.Parameters.AddWithValue("@Barcode", barcode);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var product = new SanPhamBarcodeInfo
                                {
                                    Id_SanPham = reader["Id_SanPham"].ToString(),
                                    TenSanPham = reader["TenSanPham"].ToString(),
                                    GiaBan = Convert.ToDouble(reader["GiaBan"]),
                                    HinhAnh = reader["HinhAnh"]?.ToString(),
                                    MoTa = reader["MoTa"]?.ToString(),
                                    SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                                    TrangThai = Convert.ToBoolean(reader["TrangThai"]),
                                    Barcode = barcode,
                                    FoundInDatabase = true
                                };

                                // Ghi lại lịch sử quét thành công
                                await RecordBarcodeScanAsync(barcode, userId, true, product.Id_SanPham);
                                return product;
                            }
                        }
                    }
                }

                // Không tìm thấy trong DB, ghi lại lịch sử
                await RecordBarcodeScanAsync(barcode, userId, false);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi kiểm tra barcode trong database: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gọi API bên ngoài để lấy thông tin về mã barcode
        /// </summary>
        public async Task<BarcodeApiResponse> LookupBarcodeFromApiAsync(string barcode, string userId = null)
        {
            try
            {
                var client = _clientFactory.CreateClient("BarcodeAPI");
                var response = await client.GetAsync($"{_apiBaseUrl}products?barcode={barcode}&key={_apiKey}");
                
                string jsonResponse = null;
                if (response.IsSuccessStatusCode)
                {
                    jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<BarcodeApiResponse>(jsonResponse);
                    
                    // Ghi lại lịch sử quét API
                    await RecordBarcodeScanAsync(barcode, userId, false, null, jsonResponse);
                    
                    return apiResponse;
                }
                
                // API không trả về kết quả thành công
                await RecordBarcodeScanAsync(barcode, userId, false, null, 
                    $"API Error: {(int)response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gọi API barcode lookup: {ex.Message}");
                await RecordBarcodeScanAsync(barcode, userId, false, null, $"Exception: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Ghi lại lịch sử quét barcode
        /// </summary>
        private async Task<int> RecordBarcodeScanAsync(string barcode, string userId, bool foundInDatabase, 
            string productId = null, string apiResponse = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_RecordBarcodeScan", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Barcode", barcode);
                        command.Parameters.AddWithValue("@User_Id", string.IsNullOrEmpty(userId) ? DBNull.Value : (object)userId);
                        command.Parameters.AddWithValue("@FoundInDatabase", foundInDatabase);
                        command.Parameters.AddWithValue("@Id_SanPham", string.IsNullOrEmpty(productId) ? DBNull.Value : (object)productId);
                        command.Parameters.AddWithValue("@ApiResponse", string.IsNullOrEmpty(apiResponse) ? DBNull.Value : (object)apiResponse);

                        object result = await command.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi ghi lịch sử quét barcode: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Cập nhật thông tin khi thêm sản phẩm vào cơ sở dữ liệu từ API
        /// </summary>
        public async Task UpdateBarcodeScanAddedAsync(int scanId, string productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_UpdateBarcodeScanAdded", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ScanId", scanId);
                        command.Parameters.AddWithValue("@Id_SanPham", productId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật trạng thái thêm sản phẩm: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy lịch sử quét barcode
        /// </summary>
        public async Task<List<BarcodeScanHistory>> GetScanHistoryAsync(string userId = null, int limit = 50)
        {
            var history = new List<BarcodeScanHistory>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT TOP (@Limit) bs.Id, bs.Barcode, bs.ScanDate, bs.FoundInDatabase, " +
                                "bs.IsAdded, bs.DateAdded, sp.TenSanPham, u.UserName " +
                                "FROM BarcodeScans bs " +
                                "LEFT JOIN SanPhams sp ON bs.Id_SanPham = sp.Id_SanPham " +
                                "LEFT JOIN AspNetUsers u ON bs.User_Id = u.Id ";

                    if (!string.IsNullOrEmpty(userId))
                    {
                        sql += "WHERE bs.User_Id = @UserId ";
                    }

                    sql += "ORDER BY bs.ScanDate DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Limit", limit);
                        
                        if (!string.IsNullOrEmpty(userId))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                        }

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                history.Add(new BarcodeScanHistory
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Barcode = reader["Barcode"].ToString(),
                                    ScanDate = Convert.ToDateTime(reader["ScanDate"]),
                                    FoundInDatabase = Convert.ToBoolean(reader["FoundInDatabase"]),
                                    IsAdded = Convert.ToBoolean(reader["IsAdded"]),
                                    DateAdded = reader["DateAdded"] != DBNull.Value 
                                        ? Convert.ToDateTime(reader["DateAdded"]) 
                                        : (DateTime?)null,
                                    TenSanPham = reader["TenSanPham"] != DBNull.Value 
                                        ? reader["TenSanPham"].ToString() 
                                        : null,
                                    UserName = reader["UserName"] != DBNull.Value 
                                        ? reader["UserName"].ToString() 
                                        : null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy lịch sử quét barcode: {ex.Message}");
            }

            return history;
        }
    }

    #region Models
    
    public class SanPhamBarcodeInfo
    {
        public string Id_SanPham { get; set; }
        public string TenSanPham { get; set; }
        public double GiaBan { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public int SoLuongTon { get; set; }
        public bool TrangThai { get; set; }
        public string Barcode { get; set; }
        public bool FoundInDatabase { get; set; }
    }
    
    public class BarcodeApiResponse
    {
        public List<BarcodeProduct> Products { get; set; }
    }
    
    public class BarcodeProduct
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public List<string> Images { get; set; }
        public List<Store> Stores { get; set; }
    }
    
    public class Store
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Price { get; set; }
    }
    
    public class BarcodeScanHistory
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public DateTime ScanDate { get; set; }
        public bool FoundInDatabase { get; set; }
        public bool IsAdded { get; set; }
        public DateTime? DateAdded { get; set; }
        public string TenSanPham { get; set; }
        public string UserName { get; set; }
    }
    
    #endregion
} 