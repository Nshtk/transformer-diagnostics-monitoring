using Grpc.Core;
using Proto;
using Proto.Connection;

namespace MainApp.Server.Services;

public class ServiceConnection : Connection.ConnectionBase
{
	public class User
	{
		public uint id;
		public uint id_connected_to;
		public string? name;
		public DateTime last_time_connected;
		public bool monitoring_enabled=false;
	}

	private uint _id_counter = 0;
	public static Dictionary<uint, User?> Users = new Dictionary<uint, User?>();

	public override Task<AuthorizeUserReply> authorizeUser(AuthorizeUserRequest request, ServerCallContext context)
	{
		AuthorizeUserReply reply = new AuthorizeUserReply
		{
			Information=new InformationReply 
			{
				Result=true,
				Details=$"Connected as {request.UserName} #{_id_counter}.",
				Type=InformationReply.Types.Message_Type.Ordinary
			},
			Id=_id_counter
		};
		Users.Add(_id_counter, new User 
		{
			id=_id_counter,
			id_connected_to=_id_counter,
			name=request.UserName,
			last_time_connected=DateTime.Now
		});
		_id_counter++;

		return Task.FromResult(reply);
	}
	public override Task<KeepConnectionReply> keepConnection(KeepConnectionRequest request, ServerCallContext context)
	{
		if(Users[request.Id]==null)
		{
			return Task.FromResult(new KeepConnectionReply
			{
				Information=new InformationReply
				{
					Result=false,
					Details="Connection blocked, user is not authorized.",
					Type=InformationReply.Types.Message_Type.Critical
				}
			});
		}
		if(!Users.ContainsKey(request.Id))
		{
			Users.Add(_id_counter++, null);
			return Task.FromResult(new KeepConnectionReply 
			{
				Information=new InformationReply 
				{
					Result=false,
					Details="Connection blocked, user id not found.",
					Type=InformationReply.Types.Message_Type.Critical
				}
			});
		}

		Users[request.Id].last_time_connected=DateTime.Now;
		return Task.FromResult(new KeepConnectionReply 
		{
			Information=new InformationReply 
			{
				Result=true,
				Details="",
				Type=InformationReply.Types.Message_Type.Ordinary
			}
		});
	}
}
