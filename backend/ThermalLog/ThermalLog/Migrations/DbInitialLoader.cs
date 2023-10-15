using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Globalization;
using ThermalLog.Models;

namespace ThermalLog.Migrations
{
    public class DbInitialLoader
    {
        private DbContext _context;
        private const int COMMIT_SPAN = 500;
        public DbInitialLoader(ThermalDbContext context)
        {
            _context = context;
        }

        public void Load(string directory)
        {
            var csvfiles =Directory.EnumerateFiles(directory)
                            .Where(f => f.EndsWith(".csv", true, CultureInfo.InvariantCulture));
            foreach (var csvfile in csvfiles)
            {
                loadFromCsv(csvfile);
            }
        }
        

        private void loadFromCsv(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvHelper.CsvReader(reader , CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ThermalMap>();
                var thermals = csv.GetRecords<Thermal>();

                var records = 0;
                foreach (var thermal in thermals)
                {
                    thermal.CreatedAt = DateTime.Now;
                    _context.Add(thermal);
                    records++;
                    if(records % COMMIT_SPAN == 0)
                    {
                        _context.SaveChanges();
                    }
                }
                _context.SaveChanges();
                
            }
        }

        internal class ThermalMap : ClassMap<Thermal>
        {
            public ThermalMap()
            {
                Map(m => m.Id).Optional();
                Map(m => m.HostName);
                Map(m => m.MeasuredAt);
                Map(m => m.Temperature);
                Map(m => m.Humidity);
                Map(m => m.CreatedAt).Optional();
            }
        }
    }
}
