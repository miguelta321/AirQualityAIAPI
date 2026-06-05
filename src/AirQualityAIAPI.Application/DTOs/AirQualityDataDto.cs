using System.ComponentModel.DataAnnotations;

namespace AirQualityIAAPI.Application.DTOs;

public class AirQualityDataDto
{
    [Required]
    public double PM25 { get; set; }

    [Required]
    public double PM10 { get; set; }

    [Required]
    public double CO { get; set; }

    [Required]
    public double O3 { get; set; }

    [Required]
    public double Temperature { get; set; }
}