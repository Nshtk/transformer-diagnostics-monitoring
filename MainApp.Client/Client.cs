using Grpc.Core;

namespace MainApp.Client;
public abstract class Client
{
	protected Channel _channel;

	protected Client()
	{}

	protected void initilizeChannel(int port)
	{
		_channel=new Channel("localhost", port, ChannelCredentials.Insecure);
	}
}
