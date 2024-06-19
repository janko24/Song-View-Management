using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SongViewManagement.Data;
using SongViewManagement.Helper;
using Topshelf;

namespace SongViewUpdaterService
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            HostFactory.Run(x =>
            {
                // Set up dependency injection
                var serviceProvider = new ServiceCollection()
                    .AddDbContext<SongDbContext>(options =>
                        options.UseSqlServer(connectionString), ServiceLifetime.Scoped)

                    .AddSingleton<YouTubeViewCountService>(provider =>
                    {
                        // Retrieve the API key from configuration
                        string apiKey = "AxxxxxxxxxxxxxxxxxxE";
                        return new YouTubeViewCountService(apiKey);
                    })
                    .AddScoped<SongViewHelper>()
                    .AddTransient<SongViewService>()
                    .BuildServiceProvider();

                // Resolve SongViewService instance
                var songViewService = serviceProvider.GetService<SongViewService>();

                x.Service<SongViewService>(s =>
                {
                    s.ConstructUsing(name => songViewService);
                    s.WhenStarted(tc =>
                    {
                        tc.Start();
                    });
                    s.WhenStopped(tc =>
                    {
                        tc.Stop();
                    });
                });

                x.RunAsLocalSystem();
                x.SetServiceName("SongViewUpdaterService");
                x.SetDisplayName("Song View Updater Service");
                x.SetDescription("A service to update song view data every day at a given time.");
            });
        }
    }

    public class SongViewService
    {
        private readonly SongViewHelper _songViewHelper;
        private System.Threading.Timer _timer;

        public SongViewService(SongViewHelper songViewHelper)
        {
            _songViewHelper = songViewHelper ?? throw new ArgumentNullException(nameof(songViewHelper));
        }

        public void Start()
        {
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 14, 42, 0);
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1); // Move to next day if already past 06:06 am
            }
            var delay = nextRun - now;
            // Start the service daily at 06.06 
            _timer = new System.Threading.Timer(async _ => await RunJob(), null, delay, TimeSpan.FromHours(24));

            // Start the service at one minute gap
            //_timer = new System.Threading.Timer(async _ => await RunJob(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            // Stop the service
            _timer?.Change(Timeout.Infinite, 0);
        }

        private async Task RunJob()
        {
            try
            {
                // Call AddNewSongViewRowsAsync method from SongViewHelper
                await _songViewHelper.AddNewSongViewRowsAsync();
                Console.WriteLine("New song view rows added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
