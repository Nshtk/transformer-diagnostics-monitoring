using Proto.ServerMediator;
using Proto.ServerMediator.Db.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Client.Clients.ServerMediator;
public class ClientMonitoring : Client
{
	private readonly DbMonitoring.DbMonitoringClient _client;
	//private SelectRequest _request = new SelectRequest();

	public ClientMonitoring()
	{
		initilizeChannel(5002);
		_client = new DbMonitoring.DbMonitoringClient(_channel);
	}

	public async Task<SelectReply> selectFromDbAsync(SelectRequest request)
	{
		return await _client.selectFromDatabaseAsync(request);
	}
	/*public async Task<getParameterValuesPastReply> getParameterValuesPastAsync(uint id)
	{
		_request.Id = id;
		return await _client.getParameterValuesPastAsync(_request);
	}*/
}
