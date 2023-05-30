using Grpc.Core;
using Proto;
using Proto.Connection;

namespace MainApp.Client.Clients.ClientServer;
public class ClientConnection : Client
{
    private readonly Connection.ConnectionClient _client;
    private AuthorizeUserRequest _authorize_user_request = new AuthorizeUserRequest();

    public ClientConnection()
    {
        initilizeChannel(5001);
        _client = new Connection.ConnectionClient(_channel);
    }

    public async Task<AuthorizeUserReply> authorizeUserAsync(string username)
    {
        _authorize_user_request.UserName = username;

        if (_channel.State == ChannelState.Shutdown || _channel.State == ChannelState.TransientFailure)
            return new AuthorizeUserReply
            {
                Id = 0,
                Information = new InformationReply
                {
                    Result = false,
                    Details = "Can't connect to the server.",
                    Type = InformationReply.Types.Message_Type.Critical
                }
            };
        return await _client.authorizeUserAsync(_authorize_user_request);
    }
}
