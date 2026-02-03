using Hangfire;

namespace Streamline.Infrastructure.BackgroundWorkers.Hangfire
{
    public static class HangfireSettings
    {
        public static void ConfigureJobs(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<RetryFaildedOrdersWorker>(
                "RetryFaildedOrdersWorker", 
                job => job.Run(),           
                "*/5 * * * *"              // Cron: a cada 5 minutos
            );
        }
    }
}
