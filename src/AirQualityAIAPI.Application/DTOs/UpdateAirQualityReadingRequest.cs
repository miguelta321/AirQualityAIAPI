using System.ComponentModel.DataAnnotations;

namespace AirQualityAIAPI.Application.DTOs;

public class UpdateAirQualityReadingRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Location { get; set; } = string.Empty;

    [Range(0, 500)]
    public double AirQualityIndex { get; set; }

    [Range(0, double.MaxValue)]
    public double Pm25 { get; set; }

    [Range(0, double.MaxValue)]
    public double Pm10 { get; set; }

    [Range(0, double.MaxValue)]
    public double Co2 { get; set; }

    [Range(0, double.MaxValue)]
    public double No2 { get; set; }
}
