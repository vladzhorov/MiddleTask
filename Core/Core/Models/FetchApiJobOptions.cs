namespace Core.Models;

/// <summary>
/// Универсальные настройки cron‑джобы для Quartz.
/// Можно использовать для любых джоб с расписанием.
/// </summary>
public class CronJobOptions
{
    public string Schedule { get; set; }
    public string Description { get; set; }
}

