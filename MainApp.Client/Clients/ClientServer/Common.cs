using Proto;
using Proto.Common;
using Proto.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Client.Clients.ClientServer;
public class ClientCommon : Client
{
    private readonly Common.CommonClient _client;
    private Request _request = new Request();
    public ClientCommon()
    {
        initilizeChannel(5001);
        _client = new Common.CommonClient(_channel);
    }

    public async Task<EnableMonitoringReply> enableMonitoringAsync(uint id)
    {
        _request.Id = id;
        return await _client.enableMonitoringAsync(_request);
    }
	public async Task<TransformersDataReply> getTransformersDataAsync(uint id)
	{
		_request.Id = id;
		return await _client.getTransformersDataAsync(_request);
	}
}
