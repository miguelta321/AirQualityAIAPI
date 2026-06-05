using System.Text;
using AirQualityIAAPI.Application.DTOs;
using AirQualityIAAPI.Domain.Entities;

namespace AirQualityIAAPI.Application.Services;

public static class PromptBuilder
{
    public static string Build(
        ChatRequestDto request,
        List<ChatMessage> history)
    {
        var sb = new StringBuilder();

        sb.AppendLine(""""
            Eres un asistente virtual especializado en calidad del aire en Bogotá.

            Reglas:

            - Responde siempre en español.
            - Usa un lenguaje claro y profesional.
            - Explica los contaminantes PM2.5, PM10, CO y O3 cuando sea relevante.
            - Da recomendaciones de salud cuando la calidad del aire sea desfavorable.
            - Utiliza los datos proporcionados para responder.
            - Mantén el contexto de la conversación.
            - Si el usuario pregunta algo fuera de calidad del aire, responde brevemente e intenta relacionarlo con el tema ambiental.
            """");

        sb.AppendLine();

        sb.AppendLine("DATOS ACTUALES");

        sb.AppendLine(
            $"PM2.5: {request.AirData.PM25}");

        sb.AppendLine(
            $"PM10: {request.AirData.PM10}");

        sb.AppendLine(
            $"CO: {request.AirData.CO}");

        sb.AppendLine(
            $"O3: {request.AirData.O3}");

        sb.AppendLine(
            $"Temperatura: {request.AirData.Temperature}");

        sb.AppendLine();

        sb.AppendLine("HISTORIAL");

        foreach (var item in history)
        {
            sb.AppendLine(
                $"Usuario: {item.UserMessage}");

            sb.AppendLine(
                $"Asistente: {item.AssistantResponse}");
        }

        sb.AppendLine();

        sb.AppendLine(
            $"Pregunta actual: {request.Message}");

        return sb.ToString();
    }
}