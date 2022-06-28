using Microsoft.EntityFrameworkCore;
using ScreenCaptureService.Db;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCaptureService
{
    public class ScreenCapture: IHostedService, IDisposable
    {
        private readonly ILogger<ScreenCapture> _logger;
        private readonly IConfiguration _config;
        private ScreenCaptureDbContext _dbCtx;
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        private Timer? _timer = null;
        private ScreenCaptureLog? _scLog;
        private int _screenshotsTaken = 0;

        public ScreenCapture(ILogger<ScreenCapture> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            var dbOptBuilder = new DbContextOptionsBuilder<ScreenCaptureDbContext>()
                .UseSqlite(_config.GetConnectionString("SQLite"));
                
            _dbCtx = new ScreenCaptureDbContext(dbOptBuilder.Options);

            _screenWidth = _config.GetValue<int>("Resolution:Width");
            _screenHeight = _config.GetValue<int>("Resolution:Height");

            _scLog = new ScreenCaptureLog();

            //_dbCtx.Database.EnsureDeleted();
            _dbCtx.Database.EnsureCreated();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {


            _logger.LogInformation("Timed Hosted Service running.");

            // Write to database start time
            _scLog.StartedAt = DateTime.Now;
            _dbCtx.ScreenCaptureLogs.Add(_scLog);

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            using var bitmap = new Bitmap(_screenWidth, _screenHeight);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
            }

            _screenshotsTaken++;

            lock (ImageResource.lastScreenshotLock)
            {
                ImageResource.lastScreenshot = new Bitmap(bitmap);
            }

            Console.WriteLine($"Screenshot {_screenshotsTaken} taken at {DateTime.Now}.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            // Write to database stop time
            _scLog.StoppedAt = DateTime.Now;

            // Write to database number of screenshots taken
            _scLog.NumberOfScreenshots = _screenshotsTaken;

            _dbCtx.SaveChangesAsync();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
