namespace API.Models;

public class MetricTypeAggregation
{
    public string Type { get; set; } = string.Empty;
    public long Count { get; set; }
    public double? AvgEnergy { get; set; }
}

