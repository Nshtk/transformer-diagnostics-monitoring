using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using Grpc.Core;
using MainApp.ServerMediator.Models.Diagnostics;
using Proto.ServerMediator;
using Proto.ServerMediator.Db.Diagnostics;
using Proto;

namespace MainApp.ServerMediator.Services;

public class ServiceDbDiagnostics : DbDiagnostics.DbDiagnosticsBase
{
	/*private DbContextDiagnostics _db_context;

	public ServiceDbDiagnostics(DbContextDiagnostics db_context)
	{
		_db_context = db_context;
	}

	public override async Task<SelectReply> selectFromDatabase(SelectRequest request, ServerCallContext context)
	{
		InformationReply result_reply = new InformationReply() { Result =true, Details = "Entry successfully found." };
		Any any = new Any();

		switch(request.SelectFrom)
		{
			case Table.Player:
				Proto.Mediator.Player? player_message = null;
				Models.Player? player_model = new Models.Player();
				switch(request.SelectBy)
				{
					case Proto.Mediator.Field.Id:
						player_model=await _draughts_context.Player.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.Login:
						player_model=await _draughts_context.Player.SingleOrDefaultAsync(player => player.Login==request.Str);
						break;
					case Proto.Mediator.Field.Password:
						string _player_password_string_64 = request.Bytes.ToBase64();
						player_model=await _draughts_context.Player.SingleOrDefaultAsync(player => player.Password==_player_password_string_64);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(player_model!=null)
				{
					player_message=new Proto.Mediator.Player {
						Id=player_model.Id,
						Login=player_model.Login,
						Password=ByteString.CopyFrom(Convert.FromBase64String(player_model.Password))
					};
					any=Any.Pack(player_message);
					return new SelectReply() { Result = result_reply, Entry = any };
				}
				break;
			case Table.Playerinfo:
				Proto.Mediator.PlayerInfo? playerinfo_message = null;
				Models.PlayerInfo? playerinfo_model = new Models.PlayerInfo();
				switch(request.SelectBy)
				{
					case Proto.Mediator.Field.Id:
						playerinfo_model=await _draughts_context.PlayerInfo.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.RegistrationDate:
						playerinfo_model=await _draughts_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Registration_Date==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginTimestamp:
						playerinfo_model=await _draughts_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Login_Timestamp==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginLastDatetime:
						playerinfo_model=await _draughts_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Login_Last_Datetime==request.Timestamp.ToDateTime());
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(playerinfo_model!=null)
				{
					playerinfo_message=new Proto.Mediator.PlayerInfo {
						Id=playerinfo_model.Id,
						RegistrationDate=playerinfo_model.Registration_Date.ToTimestamp(),
						LoginTimestamp=playerinfo_model.Login_Timestamp.ToTimestamp(),
						LoginLastDatetime=playerinfo_model.Login_Last_Datetime.ToTimestamp()
					};
					any=Any.Pack(playerinfo_message);
					return new SelectReply() { Result = result_reply, Entry = any };
				}
				break;
			case Table.Playerblocklist:
				Proto.Mediator.PlayerBlockList? playerblocklist_message = null;
				Models.PlayerBlockList? playerblocklist_model = new Models.PlayerBlockList();
				switch(request.SelectBy)
				{
					case Proto.Mediator.Field.Id:
						playerblocklist_model=await _draughts_context.PlayerBlockList.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.RegistrationDate:
						playerblocklist_model=await _draughts_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Datetime_Begin==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginTimestamp:
						playerblocklist_model=await _draughts_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Datetime_End==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginLastDatetime:
						playerblocklist_model=await _draughts_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Info==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(playerblocklist_model!=null)
				{
					playerblocklist_message=new Proto.Mediator.PlayerBlockList {
						Id=playerblocklist_model.Id,
						BlockDatetimeBegin=playerblocklist_model.Block_Datetime_Begin.ToTimestamp(),
						BlockDatetimeEnd=playerblocklist_model.Block_Datetime_End.ToTimestamp(),
						BlockInfo=playerblocklist_model.Block_Info
					};
					any=Any.Pack(playerblocklist_message);
					return new SelectReply() { Result = result_reply, Entry = any };
				}
				break;
			case Table.PlayerSecurityquestion:
				Proto.Mediator.Player_SecurityQuestion? player_securityquestion_message = null;
				Models.Player_SecurityQuestion? player_securityquestion_model = new Models.Player_SecurityQuestion();
				switch(request.SelectBy)
				{
					case Proto.Mediator.Field.Id:
						player_model=await _draughts_context.Player.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.SecurityAnswer:
						player_securityquestion_model=await _draughts_context.Player_SecurityQuestion.SingleOrDefaultAsync(player_securityquestion => player_securityquestion.Security_Answer==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(player_securityquestion_model!=null)
				{
					player_securityquestion_message=new Proto.Mediator.Player_SecurityQuestion {
						Id=player_securityquestion_model.Id,
						SecurityAnswer=player_securityquestion_model.Security_Answer
					};
					any=Any.Pack(player_securityquestion_message);
					return new SelectReply() { Result = result_reply, Entry = any };
				}
				break;
			case Table.Securityquestion:
				Proto.Mediator.SecurityQuestion? securityquestion_message = null;
				Models.SecurityQuestion? securityquestion_model = new Models.SecurityQuestion();
				switch(request.SelectBy)
				{
					case Proto.Mediator.Field.Id:
						securityquestion_model=await _draughts_context.SecurityQuestion.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.Login:
						securityquestion_model=await _draughts_context.SecurityQuestion.SingleOrDefaultAsync(securityquestion => securityquestion.Question==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(securityquestion_model!=null)
				{
					securityquestion_message=new Proto.Mediator.SecurityQuestion {
						Id=securityquestion_model.Id,
						Question=securityquestion_model.Question
					};
					any=Any.Pack(securityquestion_message);
					return new SelectReply() { Result = result_reply, Entry = any };
				}
				break;
		}

		result_reply.Result=false;
		result_reply.Details="ERROR: Entry not found.";
		any=Any.Pack(new Proto.Mediator.Empty());

		return new SelectReply() { Result = result_reply, Entry = any };
	}

	public override async Task<InformationReply> deleteFromDatabase(DeleteRequest request, ServerCallContext context)
	{
		InformationReply result_reply = new InformationReply() { Result =true, Details = "Entry successfully deleted." };

		switch(request.DeleteFrom)
		{
			case Table.Player:
				Proto.Mediator.Player? player_message = null;
				Models.Player? player_model = new Models.Player();
				switch(request.DeleteBy)
				{
					case Proto.Mediator.Field.Id:
						player_model=await _draughts_context.Player.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.Login:
						player_model=await _draughts_context.Player.SingleOrDefaultAsync(player => player.Login==request.Str);
						break;
					case Proto.Mediator.Field.Password:
						string player_password_string_64 = request.Bytes.ToBase64();
						player_model=await _draughts_context.Player.SingleOrDefaultAsync(player => player.Password==player_password_string_64);
						break;
				}
				if(player_model!=null)
				{
					_draughts_context.Remove(player_model);
					await _draughts_context.SaveChangesAsync();
					return result_reply;
				}
				break;
			case Table.Playerinfo:
				Proto.Mediator.PlayerInfo? playerinfo_message = null;
				Models.PlayerInfo? playerinfo_model = new Models.PlayerInfo();
				switch(request.DeleteBy)
				{
					case Proto.Mediator.Field.Id:
						playerinfo_model=await _draughts_context.PlayerInfo.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.RegistrationDate:
						playerinfo_model=await _draughts_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Registration_Date==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginTimestamp:
						playerinfo_model=await _draughts_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Login_Timestamp==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginLastDatetime:
						playerinfo_model=await _draughts_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Login_Last_Datetime==request.Timestamp.ToDateTime());
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(playerinfo_model!=null)
				{
					_draughts_context.Remove(playerinfo_model);
					await _draughts_context.SaveChangesAsync();
					return result_reply;
				}
				break;
			case Table.Playerblocklist:
				Proto.Mediator.PlayerBlockList? playerblocklist_message = null;
				Models.PlayerBlockList? playerblocklist_model = new Models.PlayerBlockList();
				switch(request.DeleteBy)
				{
					case Proto.Mediator.Field.Id:
						playerblocklist_model=await _draughts_context.PlayerBlockList.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.RegistrationDate:
						playerblocklist_model=await _draughts_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Datetime_Begin==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginTimestamp:
						playerblocklist_model=await _draughts_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Datetime_End==request.Timestamp.ToDateTime());
						break;
					case Proto.Mediator.Field.LoginLastDatetime:
						playerblocklist_model=await _draughts_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Info==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(playerblocklist_model!=null)
				{
					_draughts_context.Remove(playerblocklist_model);
					await _draughts_context.SaveChangesAsync();
					return result_reply;
				}
				break;
			case Table.PlayerSecurityquestion:
				Proto.Mediator.Player_SecurityQuestion? player_securityquestion_message = null;
				Models.Player_SecurityQuestion? player_securityquestion_model = new Models.Player_SecurityQuestion();
				switch(request.DeleteBy)
				{
					case Proto.Mediator.Field.Id:
						player_securityquestion_model=await _draughts_context.Player_SecurityQuestion.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.SecurityAnswer:
						player_securityquestion_model=await _draughts_context.Player_SecurityQuestion.SingleOrDefaultAsync(player_securityquestion => player_securityquestion.Security_Answer==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(player_securityquestion_model!=null)
				{
					_draughts_context.Remove(player_securityquestion_model);
					await _draughts_context.SaveChangesAsync();
					return result_reply;
				}
				break;
			case Table.Securityquestion:
				Proto.Mediator.SecurityQuestion? securityquestion_message = null;
				Models.SecurityQuestion? securityquestion_model = new Models.SecurityQuestion();
				switch(request.DeleteBy)
				{
					case Proto.Mediator.Field.Id:
						securityquestion_model=await _draughts_context.SecurityQuestion.FindAsync(request.Integer);
						break;
					case Proto.Mediator.Field.Login:
						securityquestion_model=await _draughts_context.SecurityQuestion.SingleOrDefaultAsync(securityquestion => securityquestion.Question==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(securityquestion_model!=null)
				{
					_draughts_context.Remove(securityquestion_model);
					await _draughts_context.SaveChangesAsync();
					return result_reply;
				}
				break;
		}

		result_reply.Result=false;
		result_reply.Details="ERROR: Entry not found.";

		return result_reply;
	}
	public override async Task<InformationReply> updateInDatabase(UpdateRequest request, ServerCallContext context)
	{
		InformationReply result_reply = new InformationReply() { Result=true, Details = "Entry successfully deleted." };
		bool unpack_result;

		switch(request.UpdateIn)
		{
			case Table.Player:
				Models.Player? player_model;

				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.Player player_message);
				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack player!";
					break;
				}

				player_model=await _draughts_context.Player.FindAsync(player_message.Id);
				if(player_model==null)
				{
					result_reply.Details="ERROR: Failed to find player!";
					break;
				}
				player_model.Login=player_message.Login;
				player_model.Password=player_message.Password.ToBase64();
				try
				{
					_draughts_context.Player.Update(player_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return result_reply;
			case Table.Playerinfo:
				Models.PlayerInfo? playerinfo_model = new Models.PlayerInfo();
				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.PlayerInfo playerinfo_message);

				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack playerinfo!";
					break;
				}

				playerinfo_model=await _draughts_context.PlayerInfo.FindAsync(playerinfo_message.Id);
				if(playerinfo_model==null)
				{
					result_reply.Details="ERROR: Failed to find playerinfo!";
					break;
				}
				playerinfo_model.Login_Timestamp=playerinfo_message.LoginTimestamp.ToDateTime();
				playerinfo_model.Registration_Date=playerinfo_message.RegistrationDate.ToDateTime();
				playerinfo_model.Login_Last_Datetime=playerinfo_model.Login_Last_Datetime;

				try
				{
					_draughts_context.PlayerInfo.Update(playerinfo_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return result_reply;
			case Table.Playerblocklist:
				Models.PlayerBlockList? playerblocklist_model = new Models.PlayerBlockList();
				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.PlayerBlockList playerblocklist_message);

				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack playerblocklist!";
					break;
				}
				playerblocklist_model=await _draughts_context.PlayerBlockList.FindAsync(playerblocklist_message.Id);
				if(playerblocklist_model==null)
				{
					result_reply.Details="ERROR: Failed to find playerblocklist!";
					break;
				}
				playerblocklist_model.Block_Datetime_Begin=playerblocklist_message.BlockDatetimeBegin.ToDateTime();
				playerblocklist_model.Block_Datetime_End=playerblocklist_message.BlockDatetimeEnd.ToDateTime();
				playerblocklist_model.Block_Info=playerblocklist_message.BlockInfo;

				try
				{
					_draughts_context.PlayerBlockList.Update(playerblocklist_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return result_reply;
			case Table.Securityquestion:
				Models.SecurityQuestion? securityquestion_model = new Models.SecurityQuestion();

				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.SecurityQuestion securityquestion_message);
				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack securityquestion!";
					break;
				}

				securityquestion_model=await _draughts_context.SecurityQuestion.FindAsync(securityquestion_message.Id);
				if(securityquestion_model==null)
				{
					result_reply.Details="ERROR: Failed to find securityquestion!";
					break;
				}
				securityquestion_model.Question=securityquestion_message.Question;

				try
				{
					_draughts_context.SecurityQuestion.Update(securityquestion_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return result_reply;
			case Table.PlayerSecurityquestion:
				Models.Player_SecurityQuestion? player_securityquestion_model = new Models.Player_SecurityQuestion();
				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.Player_SecurityQuestion player_securityquestion_message);

				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack player_securityquestion!";
					break;
				}
				player_securityquestion_model=await _draughts_context.Player_SecurityQuestion.FindAsync(player_securityquestion_message.Id);
				if(player_securityquestion_model==null)
				{
					result_reply.Details="ERROR: Failed to find player_securityquestion!";
					break;
				}
				player_securityquestion_model.Security_Answer=player_securityquestion_message.SecurityAnswer;
				try
				{
					_draughts_context.Player_SecurityQuestion.Update(player_securityquestion_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return result_reply;
		}
		result_reply.Result=false;

		return result_reply;
	}
	public override async Task<InformationReply> insertToDatabase(InsertRequest request, ServerCallContext context)
	{
		InformationReply result_reply = new InformationReply() { Result = true, Details = "Entry successfully inserted." };
		bool unpack_result;

		switch(request.InsertTo)
		{
			case Table.Player:
				Models.Player player_model = new Models.Player();

				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.Player player_message);
				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack player!";
					break;
				}

				player_model.Id=player_message.Id;
				player_model.Login=player_message.Login;
				player_model.Password=player_message.Password.ToBase64();
				try
				{
					await _draughts_context.Player.AddAsync(player_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return result_reply;
			case Table.Playerinfo:
				Models.PlayerInfo playerinfo_model = new Models.PlayerInfo();
				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.PlayerInfo playerinfo_message);

				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack playerinfo!";
					break;
				}

				playerinfo_model.Id=playerinfo_message.Id;
				playerinfo_model.Player=new Models.Player {
					Id=playerinfo_message.Player.Id,
					Login=playerinfo_message.Player.Login,
					Password=playerinfo_message.Player.Password.ToBase64()
				};
				playerinfo_model.Login_Timestamp=playerinfo_message.LoginTimestamp.ToDateTime();
				playerinfo_model.Registration_Date=playerinfo_message.RegistrationDate.ToDateTime();
				playerinfo_model.Login_Last_Datetime=playerinfo_model.Login_Last_Datetime;

				try
				{
					await _draughts_context.PlayerInfo.AddAsync(playerinfo_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return result_reply;
			case Table.Playerblocklist:
				Models.PlayerBlockList playerblocklist_model = new Models.PlayerBlockList();
				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.PlayerBlockList playerblocklist_message);

				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack playerblocklist!";
					break;
				}
				playerblocklist_model.Id=playerblocklist_message.Id;
				playerblocklist_model.Player=new Models.Player {
					Id=playerblocklist_message.Player.Id,
					Login=playerblocklist_message.Player.Login,
					Password=playerblocklist_message.Player.Password.ToBase64()
				};
				playerblocklist_model.Block_Datetime_Begin=playerblocklist_message.BlockDatetimeBegin.ToDateTime();
				playerblocklist_model.Block_Datetime_End=playerblocklist_message.BlockDatetimeEnd.ToDateTime();
				playerblocklist_model.Block_Info=playerblocklist_message.BlockInfo;

				try
				{
					await _draughts_context.PlayerBlockList.AddAsync(playerblocklist_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return result_reply;
			case Table.Securityquestion:
				Models.SecurityQuestion securityquestion_model = new Models.SecurityQuestion();

				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.SecurityQuestion securityquestion_message);
				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack securityquestion!";
					break;
				}

				securityquestion_model.Id=securityquestion_message.Id;
				securityquestion_model.Question=securityquestion_message.Question;

				try
				{
					await _draughts_context.SecurityQuestion.AddAsync(securityquestion_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return result_reply;
			case Table.PlayerSecurityquestion:
				Models.Player_SecurityQuestion player_securityquestion_model = new Models.Player_SecurityQuestion();
				unpack_result=request.Entry.TryUnpack(out Proto.Mediator.Player_SecurityQuestion player_securityquestion_message);

				if(!unpack_result)
				{
					result_reply.Details="ERROR: Failed to unpack player_securityquestion!";
					break;
				}
				player_securityquestion_model.Id=player_securityquestion_message.Id;
				player_securityquestion_model.Player=new Models.Player {
					Id=player_securityquestion_message.Player.Id,
					Login=player_securityquestion_message.Player.Login,
					Password=player_securityquestion_message.Player.Password.ToBase64()
				};
				player_securityquestion_model.Security_Answer=player_securityquestion_message.SecurityAnswer;
				player_securityquestion_model.SecurityQuestion=new Models.SecurityQuestion() {
					Id=player_securityquestion_message.SecurityQuestion.Id,
					Question=player_securityquestion_message.SecurityQuestion.Question
				};
				try
				{
					await _draughts_context.Player_SecurityQuestion.AddAsync(player_securityquestion_model);
					await _draughts_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return result_reply;
		}
		result_reply.Result=false;

		return result_reply;
	}*/
}
