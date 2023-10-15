namespace ThermalLog.Models
{
    /// <summary>
    /// 集計した温湿度情報を表します
    /// </summary>
    public class ThermalSummary
    {
        /// <summary>計測日時</summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 最高温度
        /// </summary>
        public double MaxTemperature { get; set; }
        /// <summary>
        /// 平均温度
        /// </summary>
        public double AvgTemperature { get; set; }
        /// <summary>
        /// 最低温度
        /// </summary>
        public double MinTemperature { get; set; }
        /// <summary>
        /// 最高湿度
        /// </summary>
        public double MaxHumidity { get; set; }
        /// <summary>
        /// 平均湿度
        /// </summary>
        public double AvgHumidity { get; set; }
        /// <summary>
        /// 最低湿度
        /// </summary>
        public double MinHumidity { get; set; }
    }
}
