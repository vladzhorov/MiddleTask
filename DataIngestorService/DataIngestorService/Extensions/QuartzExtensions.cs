using Core.Models;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace DataIngestorService.Extensions;

public static class QuartzExtensions
{
    /// <summary>
    /// Регистрирует Quartz‑джобу с cron‑расписанием на основе секции конфигурации.
    /// Универсальный метод: одна строка на джобу в Program.cs.
    /// </summary>
    public static void AddCronJob<TJob>(
        this IServiceCollectionQuartzConfigurator quartz,
        IConfiguration configuration,
        string configSectionName)
        where TJob : IJob
    {
        var options = configuration
            .GetSection(configSectionName)
            .Get<CronJobOptions>();

        if (options is null)
        {
            throw new InvalidOperationException(
                $"Configuration section '{configSectionName}' for job '{typeof(TJob).Name}' is missing.");
        }

        if (string.IsNullOrWhiteSpace(options.Schedule))
        {
            throw new InvalidOperationException(
                $"Cron schedule is not configured for job '{typeof(TJob).Name}' in section '{configSectionName}'.");
        }

        var jobKey = new JobKey(typeof(TJob).Name);

        quartz.AddJob<TJob>(opts =>
            opts.WithIdentity(jobKey)
                .WithDescription(options.Description ?? typeof(TJob).Name));

        quartz.AddTrigger(opts =>
            opts.ForJob(jobKey)
                .WithIdentity($"{typeof(TJob).Name}-trigger")
                .WithCronSchedule(options.Schedule));
    }
}

