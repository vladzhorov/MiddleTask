using Core.Models;
using DataIngestorService.BackgroundJob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DataIngestorService.Extensions;

public static class QuartzSchedulerExtensions
{
    private static readonly IReadOnlyDictionary<string, Action<IServiceCollectionQuartzConfigurator, ServiceTaskInfo>> TaskRegistrations =
        new Dictionary<string, Action<IServiceCollectionQuartzConfigurator, ServiceTaskInfo>>(StringComparer.Ordinal)
        {
            [nameof(FetchApiJob)] = RegisterFetchApiJob
        };

    public static IServiceCollection AddConfiguredQuartzScheduler(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ServiceTaskOptions>(options =>
            configuration.GetSection(nameof(ServiceTaskOptions)).Bind(options));

        services.AddQuartz(quartz =>
        {
            var options = new ServiceTaskOptions();
            configuration.GetSection(nameof(ServiceTaskOptions)).Bind(options);

            var enabledSchedules = options.Schedules.Where(x => x.Enabled).ToList();

            if (enabledSchedules.Count == 0)
            {
                throw new InvalidOperationException($"No enabled schedules found in {nameof(ServiceTaskOptions)}.{nameof(ServiceTaskOptions.Schedules)}.");
            }

            foreach (var schedule in enabledSchedules)
            {
                if (string.IsNullOrWhiteSpace(schedule.TypeName))
                {
                    throw new InvalidOperationException("Service task TypeName cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(schedule.Schedule))
                {
                    throw new InvalidOperationException($"Schedule is not configured for task '{schedule.TypeName}'.");
                }

                if (!CronExpression.IsValidExpression(schedule.Schedule))
                {
                    throw new InvalidOperationException(
                        $"Invalid cron expression '{schedule.Schedule}' for task '{schedule.TypeName}'.");
                }

                RegisterTask(quartz, schedule);
            }
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }

    private static void RegisterTask(IServiceCollectionQuartzConfigurator quartz, ServiceTaskInfo schedule)
    {
        if (!TaskRegistrations.TryGetValue(schedule.TypeName, out var register))
        {
            throw new InvalidOperationException($"Unknown scheduled task type '{schedule.TypeName}'.");
        }

        register(quartz, schedule);
    }

    private static void RegisterFetchApiJob(IServiceCollectionQuartzConfigurator quartz, ServiceTaskInfo schedule)
    {
        var groupName = string.IsNullOrWhiteSpace(schedule.GroupName) ? nameof(ServiceTaskOptions) : schedule.GroupName;
        var jobKey = new JobKey(schedule.TypeName, groupName);
        var triggerKey = new TriggerKey($"{schedule.TypeName}-trigger", groupName);

        quartz.AddJob<FetchApiJob>(opts =>
            opts.WithIdentity(jobKey)
                .WithDescription(string.IsNullOrWhiteSpace(schedule.Description) ? schedule.TypeName : schedule.Description));

        quartz.AddTrigger(opts =>
        {
            opts.ForJob(jobKey)
                .WithIdentity(triggerKey)
                .WithCronSchedule(schedule.Schedule);

            if (TimeSpan.TryParse(schedule.StartDelay, out var delay) && delay > TimeSpan.Zero)
            {
                opts.StartAt(DateTimeOffset.UtcNow.Add(delay));
            }
            else
            {
                opts.StartNow();
            }
        });
    }
}
