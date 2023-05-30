using Grpc.Core;
using Grpc.Net.Client;
using Proto.Monitoring;
using Proto;

namespace MainApp.Client.Clients.ClientServer;

public class ClientMonitoring : Client
{
    private readonly Monitoring.MonitoringClient _client;
    private Request _request = new Request();

    public ClientMonitoring()
    {
        initilizeChannel(5001);
        _client = new Monitoring.MonitoringClient(_channel);
    }

    public async Task<GetParametersReply> getParametersAsync(uint id)
    {
        _request.Id = id;
        return await _client.getParametersAsync(_request);
    }
    public async Task<getParameterValuesPastReply> getParameterValuesPastAsync(uint id)
    {
        _request.Id = id;
        return await _client.getParameterValuesPastAsync(_request);
    }
	public async Task<SendEditedParametersReply> sendEditedParametersAsync(uint id, SendEditedParametersRequest request)
	{
		_request.Id = id;
		return await _client.sendEditedParametersAsync(request);
	}
}
