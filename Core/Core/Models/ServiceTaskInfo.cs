namespace Core.Models;

/// <summary>
/// Describes a scheduled service task configured from appsettings.
/// </summary>
public class ServiceTaskInfo
{
    public int Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GroupName { get; set; } = "Default";
    public string Description { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public string StartDelay { get; set; } = "00:00:00";
    public bool Enabled { get; set; } = true;
}
