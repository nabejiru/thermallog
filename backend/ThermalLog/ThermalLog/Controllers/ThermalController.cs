using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ThermalLog.Models;

namespace ThermalLog.Controllers
{
    /// <summary>
    /// 温湿度情報API
    /// </summary>
    [ApiController]
    [Route("")]
    //    [Route("[controller]")]
    public class ThermalController : ControllerBase
    {
        private readonly ThermalDbContext _context;

        private readonly ILogger<ThermalController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public ThermalController(ILogger<ThermalController> logger, ThermalDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        /// <summary>
        /// 計測があるホスト名を取得します。
        /// </summary>
        /// <returns>すべてのホスト名</returns>
        [HttpGet("hosts")]
        public IEnumerable<string> GetHosts()
        {
            return _context.Thermals
                        .Select(t => t.HostName ?? "")
                        .Where(t => !string.IsNullOrEmpty(t))
                        .Distinct();
        }


        /// <summary>
        /// 計測記録がある年を取得します。
        /// </summary>
        /// <param name="hostName">取得対象のホスト名</param>
        /// <returns>取得した年</returns>
        [HttpGet("available_years")]
        public IEnumerable<int> GetAvailableYears([Required]string hostName)
        {
            var years = _context.Thermals
                            .Where(t => t.HostName == hostName)
                            .Select(t=>t.MeasuredAt.Year)
                            .Distinct();
            return years;
        }

        /// <summary>
        /// 温湿度の計測記録を取得します。
        /// </summary>
        /// <param name="hostName">取得対象のホスト名</param>
        /// <param name="y">取得対象の年</param>
        /// <param name="m">取得対象の月</param>
        /// <param name="d">取得対象の日</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet("temperatures/{hostName}")]
        public IEnumerable<ThermalSummary> Gettemperatures([Required]string hostName, [Range(0, 9998)] int? y, [Range(1, 12)] int? m, [Range(1, 31)] int? d) {

            var thermals = _context.Thermals.Where(t => t.HostName == hostName);
            IQueryable<IGrouping<DateTime, Thermal>> grouped;

            if (y.HasValue && m.HasValue && d.HasValue)
            {
                grouped = thermals
                        .Where(t=>t.MeasuredAt.Year == y.Value && t.MeasuredAt.Month == m.Value && t.MeasuredAt.Day == d.Value)
                        .GroupBy(t => new DateTime(t.MeasuredAt.Year, t.MeasuredAt.Month, t.MeasuredAt.Day, t.MeasuredAt.Hour, 0, 0));
            } else if(y.HasValue && m.HasValue)
            {
                grouped = thermals
                        .Where(t => t.MeasuredAt.Year == y.Value && t.MeasuredAt.Month == m.Value)
                        .GroupBy(t => new DateTime(t.MeasuredAt.Year, t.MeasuredAt.Month, t.MeasuredAt.Day));
            }
            else if(y.HasValue)
            {
                grouped = thermals
                        .Where(t => t.MeasuredAt.Year == y.Value)
                        .GroupBy(t => new DateTime(t.MeasuredAt.Year, t.MeasuredAt.Month, 1));
            } else
            {
                throw new ArgumentException("値が正しく指定されていません。");
            }
            return grouped
                .Select(t => new ThermalSummary() { 
                Date = t.Key ,

                AvgHumidity = t.Average(x => x.Humidity ?? 0d),
                MaxHumidity = t.Max(x => x.Humidity ?? 0d),
                MinHumidity = t.Min(x => x.Humidity ?? 0d),
                AvgTemperature = t.Average(x => x.Temperature ?? 0d),
                MaxTemperature = t.Max(x => x.Temperature ?? 0d),
                MinTemperature = t.Min(x => x.Temperature ?? 0d),
            }).ToList().OrderBy(t => t.Date);
        }


        /// <summary>
        /// 最新の計測情報を取得します。
        /// </summary>
        /// <param name="hostName">ホスト名</param>
        /// <returns></returns>
        [HttpGet("temperature/{hostName}")]
        public Thermal? GetNewest([Required] string hostName)
        {
            var newest = _context.Thermals.Where(t => t.HostName == hostName)
                                          .OrderByDescending(t => t.MeasuredAt)
                                          .FirstOrDefault();
            return newest;
        }

        /// <summary>
        /// 温湿度の計測記録を登録します。
        /// </summary>
        /// <param name="thermal">登録する計測情報</param>
        /// <returns></returns>
        [HttpPost("store")]
        public Thermal store(Thermal thermal)
        {
            thermal.CreatedAt = DateTime.Now;
            _context.Thermals.Add(thermal);
            _context.SaveChanges();
            return thermal;
        }
    }
}