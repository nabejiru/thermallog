using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThermalLog.Models
{
    /// <summary>
    /// 温湿度計測情報
    /// </summary>
    [Table("thermals")]
    public class Thermal
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 計測日時
        /// </summary>
        [Required]
        public DateTime MeasuredAt { get; set; }

        /// <summary>
        /// 計測場所のホスト名
        /// </summary>
        [MaxLength(128)]
        public string? HostName { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public double? Humidity { get; set; }

        /// <summary>
        /// 登録日時
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }

}
