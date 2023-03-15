using AirlineSendAgent.Dtos;

namespace AirlineSendAgent.Client;
public interface IWebhookClient
{
    Task SendWebhookNotifiaction(FlightDetailChangePayloadDto flightDetailChangePayloadDto);

}
