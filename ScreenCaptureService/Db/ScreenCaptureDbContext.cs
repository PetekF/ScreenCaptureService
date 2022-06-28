using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCaptureService.Db
{
    public class ScreenCaptureDbContext: DbContext
    {
        public ScreenCaptureDbContext(DbContextOptions<ScreenCaptureDbContext> options)
            : base(options) { }

        public DbSet<ScreenCaptureLog> ScreenCaptureLogs { get; set; } 
    }
}
