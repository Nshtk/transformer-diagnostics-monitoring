using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Proto;
using Proto.Common;
using Proto.ServerMediator;
using MainApp.Client.Clients.ServerMediator;
using Proto.ServerMediator.Db.Monitoring;

namespace MainApp.Server.Services;

public class ServiceCommon : Common.CommonBase
{
	ClientMonitoring _client_monitoring;
	public ServiceCommon()
	{
		_client_monitoring=new ClientMonitoring();
	}

	public override Task<DocumentationReply> getDocumentation(DocumentationRequest request, ServerCallContext context)
	{
		return base.getDocumentation(request, context);
	}

	public override async Task<TransformersDataReply> getTransformersData(Request request, ServerCallContext context)
	{
		TransformersDataReply reply=new TransformersDataReply() {Information=new InformationReply {Result=true, Details="Transfomers data received." } };

		SelectReply reply_select = _client_monitoring.selectFromDbAsync(new SelectRequest {
			SelectFrom=Table.MonitoringTransformer,
			SelectBy= Proto.ServerMediator.Field.IsFunctioning,
			Boolean=true
		}).Result;
		//reply.TransformersData=new 
		foreach(var entry in reply_select.Entry)
		{
			Transformer transformer;
			bool unpack_result = entry.TryUnpack(out transformer);
			TransformerData transformer_data = new TransformerData() {Id=transformer.Id, Name="", Location=transformer.Room.Name, Status="", LastConditionCheck=new Timestamp()};
			reply.TransformersData.Add(transformer_data);
		}
		
		return reply;
	}

	public override Task<EnableMonitoringReply> enableMonitoring(Request request, ServerCallContext context)
	{
		EnableMonitoringReply reply=new EnableMonitoringReply();

		if(ServiceConnection.Users[request.Id].monitoring_enabled==false)
		{
			ServiceConnection.Users[request.Id].monitoring_enabled=true;
			reply.Information=new InformationReply {
				Result = true,
				Details="Monitoring enabled.",
				Type=InformationReply.Types.Message_Type.Ordinary
			};
		}
		else
		{
			ServiceConnection.Users[request.Id].monitoring_enabled=false;
			reply.Information=new InformationReply {
				Result = true,
				Details="Monitoring disabled.",
				Type=InformationReply.Types.Message_Type.Ordinary
			};
		}
		reply.IsEnabled=ServiceConnection.Users[request.Id].monitoring_enabled;

		return Task.FromResult(reply);
	}
}
