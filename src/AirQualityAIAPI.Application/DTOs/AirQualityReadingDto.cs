namespace AirQualityAIAPI.Application.DTOs;

public class AirQualityReadingDto
{
    public Guid Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double AirQualityIndex { get; set; }
    public string Category { get; set; } = string.Empty;
    public double Pm25 { get; set; }
    public double Pm10 { get; set; }
    public double Co2 { get; set; }
    public double No2 { get; set; }
}
