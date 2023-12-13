using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using MailKit.Search;
using ExcelDataReader;

namespace GameStatsApp.Common.Extensions
{
    public static class FileExtensions
    {
        public static async Task<List<string>> ReadAsListAsync(this IFormFile file)
        {
            var results = new List<string>();
            var extension = Path.GetExtension(file.FileName);
            
            switch(extension)
            {
                case ".txt":
                    results = await file.ReadTextAsListAsync();
                    break;
                case ".xls":
                case ".xlsx":
                    results = await Task.Run(() => file.ReadExcelAsListAsync());
                    break; 
                case ".csv":
                    results = await Task.Run(() => file.ReadCSVAsListAsync());
                    break;                                           
            }

            return results;
        }

        public static async Task<List<string>> ReadTextAsListAsync(this IFormFile file)
        {
            var results = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    results.Add(await reader.ReadLineAsync()); 
            }

            return results;
        }

        public static List<string> ReadExcelAsListAsync(this IFormFile file)
        {
            var results = new List<string>();
            
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        results.Add(reader.GetValue(0).ToString());
                    }
                }
            }

            return results;
        }  

        public static List<string> ReadCSVAsListAsync(this IFormFile file)
        {
            var results = new List<string>();
            
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    while (reader.Read())
                    {
                        results.Add(reader.GetValue(0).ToString());
                    }
                }
            }

            return results;
        }              
    }
}
