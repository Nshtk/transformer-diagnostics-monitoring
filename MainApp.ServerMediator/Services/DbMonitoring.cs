using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Proto.ServerMediator;
using Proto.ServerMediator.Db.Monitoring;
using MainApp.ServerMediator.Models.Monitoring;
using Proto;
using Microsoft.EntityFrameworkCore;

namespace MainApp.ServerMediator.Services;

public class ServiceDbMonitoring : DbMonitoring.DbMonitoringBase
{
	private DbContextMonitoring _db_context=new DbContextMonitoring();

	public ServiceDbMonitoring(DbContextMonitoring db_context)
	{
		_db_context = db_context;
	}

	public override async Task<SelectReply> selectFromDatabase(SelectRequest request, ServerCallContext context)
	{
		SelectReply reply = new SelectReply();
		InformationReply information_reply = new InformationReply() { Result =true, Details = "Entries successfully found." };
		Any any = new Any();
		reply.Information = information_reply;

		using(_db_context= new DbContextMonitoring())
		switch(request.SelectFrom)
		{
			case Table.MonitoringBuilding:
			{
				Proto.ServerMediator.Db.Monitoring.Building? message = null;
				Models.Monitoring.Building? model = new Models.Monitoring.Building();
				switch(request.SelectBy)
				{
					case Proto.ServerMediator.Field.Id:
						model=await _db_context.Building.FindAsync(request.Integer);
						break;
					case Proto.ServerMediator.Field.Name:
						model=await _db_context.Building.SingleOrDefaultAsync(Building => Building.name==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(model!=null)
				{
					message=new Proto.ServerMediator.Db.Monitoring.Building {
						Id=model.id,
						Name=model.name,
					};
					any=Any.Pack(message);
					reply.Entry.Add(any);
				}
			}
				break;
			case Table.MonitoringRoom:
			{
				Proto.ServerMediator.Db.Monitoring.Room? message = null;
				Models.Monitoring.Room? model = new Models.Monitoring.Room();
				switch(request.SelectBy)
				{
					case Proto.ServerMediator.Field.Id:
						model=await _db_context.Room.FindAsync(request.Integer);
						break;
					case Proto.ServerMediator.Field.Name:
						model=await _db_context.Room.SingleOrDefaultAsync(room => room.name==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(model!=null)
				{
					message=new Proto.ServerMediator.Db.Monitoring.Room {
						Id=model.id,
						Name=model.name,
					};
					any=Any.Pack(message);
					reply.Entry.Add(any);
				}
			}
				break;
			case Table.MonitoringTransformer:
			{
				Proto.ServerMediator.Db.Monitoring.Transformer? message = null;
				List<Models.Monitoring.Transformer>? models = new List<Models.Monitoring.Transformer>();
				switch(request.SelectBy)
				{
					case Proto.ServerMediator.Field.Id:
						models.Add(await _db_context.Transformer.FindAsync(request.Integer));
						break;
					case Proto.ServerMediator.Field.IsFunctioning:
						models=_db_context.Transformer.ToList().FindAll(transformer => transformer.is_functioning==request.Boolean);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(models.Count!=0)
				{
					foreach(var model in models)
					{
						message=new Proto.ServerMediator.Db.Monitoring.Transformer {
							Id=model.id,
							IsFunctioning=model.is_functioning,
							IsMonitored=model.is_monitored,
							Room=new Proto.ServerMediator.Db.Monitoring.Room {Id=model.Room_id, Name= _db_context.Room.FindAsync(model.Room_id).Result.name }
						};
						any=Any.Pack(message);
						reply.Entry.Add(any);
					}
				}
				/*if(model!=null)
				{
					message=new Proto.ServerMediator.Db.Monitoring.Transformer {
						Id=model.id,
						IsFunctioning=model.is_functioning,
						IsMonitored=model.is_monitored,
					};
					any=Any.Pack(message);
					reply.Entry.Add(any);
				}*/
			}
				break;
			case Table.MonitoringSensor:
			{
				Proto.ServerMediator.Db.Monitoring.Sensor? message = null;
				Models.Monitoring.Sensor? model = new Models.Monitoring.Sensor();
				switch(request.SelectBy)
				{
					case Proto.ServerMediator.Field.Id:
						model=await _db_context.Sensor.FindAsync(request.Integer);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(model!=null)
				{
					message=new Proto.ServerMediator.Db.Monitoring.Sensor {
						Id=model.id,
						Type=model.type,
					};
					any=Any.Pack(message);
					reply.Entry.Add(any);
				}
			}
				break;
			case Table.MonitoringSensorTransformer:
			{
				Proto.ServerMediator.Db.Monitoring.Sensor_Transformer? message = null;
				//Models.Monitoring.Sensor_Transformer? model = new Models.Monitoring.Sensor_Transformer();
				List<Models.Monitoring.Sensor_Transformer>? models = new List<Models.Monitoring.Sensor_Transformer>();
				switch(request.SelectBy)
				{
					case Proto.ServerMediator.Field.Id:
						models = await _db_context.Sensor_Transformer.ToListAsync();
						models.FindAll(sensor_transformer => sensor_transformer.Transformer_id==request.Integer);
						break;
					case Proto.ServerMediator.Field.IsFunctioning:
						models.Add(await _db_context.Sensor_Transformer.FindAsync(request.Boolean));
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(models.Count!=0)
				{
					foreach(var sensor_transformer in models)
					{
						message=new Proto.ServerMediator.Db.Monitoring.Sensor_Transformer {
							Value=sensor_transformer.value,
							IsFunctioning=sensor_transformer.is_functioning
						};
						any=Any.Pack(message);
						reply.Entry.Add(any);
					}
				}
			}
				break;
		}
		if(reply.Entry.Count==0)
		{
			information_reply.Result=false;
			information_reply.Details="Entry not found.";
			any=Any.Pack(new Proto.ServerMediator.Empty());
			reply.Entry.Add(any);
		}
		
		return reply;
	}

	/*public override async Task<InformationReply> deleteFromDatabase(DeleteRequest request, ServerCallContext context)
	{
		InformationReply information_reply = new InformationReply() { Information =true, Details = "Entry successfully deleted." };

		switch(request.DeleteFrom)
		{
			case Table.Transformer:
				Proto.ServerMediator.Transformer? player_message = null;
				Models.Monitoring.Transformer? player_model = new Models.Monitoring.Transformer();
				switch(request.DeleteBy)
				{
					case Proto.ServerMediator.Field.Id:
						player_model=await _db_context.Transformer.FindAsync(request.Integer);
						break;
					case Proto.ServerMediator.Field.Login:
						player_model=await _db_context.Transformer.SingleOrDefaultAsync(Transformer => Transformer.Login==request.Str);
						break;
					case Proto.ServerMediator.Field.Password:
						string player_password_string_64 = request.Bytes.ToBase64();
						player_model=await _db_context.Transformer.SingleOrDefaultAsync(Transformer => Transformer.Password==player_password_string_64);
						break;
				}
				if(player_model!=null)
				{
					_db_context.Remove(player_model);
					await _db_context.SaveChangesAsync();
					return information_reply;
				}
				break;
			case Table.Playerinfo:
				Proto.ServerMediator.PlayerInfo? playerinfo_message = null;
				Models.Monitoring.PlayerInfo? playerinfo_model = new Models.Monitoring.PlayerInfo();
				switch(request.DeleteBy)
				{
					case Proto.ServerMediator.Field.Id:
						playerinfo_model=await _db_context.PlayerInfo.FindAsync(request.Integer);
						break;
					case Proto.ServerMediator.Field.RegistrationDate:
						playerinfo_model=await _db_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Registration_Date==request.Timestamp.ToDateTime());
						break;
					case Proto.ServerMediator.Field.LoginTimestamp:
						playerinfo_model=await _db_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Login_Timestamp==request.Timestamp.ToDateTime());
						break;
					case Proto.ServerMediator.Field.LoginLastDatetime:
						playerinfo_model=await _db_context.PlayerInfo.SingleOrDefaultAsync(playerinfo => playerinfo.Login_Last_Datetime==request.Timestamp.ToDateTime());
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(playerinfo_model!=null)
				{
					_db_context.Remove(playerinfo_model);
					await _db_context.SaveChangesAsync();
					return information_reply;
				}
				break;
			case Table.Playerblocklist:
				Proto.ServerMediator.PlayerBlockList? playerblocklist_message = null;
				Models.Monitoring.PlayerBlockList? playerblocklist_model = new Models.Monitoring.PlayerBlockList();
				switch(request.DeleteBy)
				{
					case Proto.ServerMediator.Field.Id:
						playerblocklist_model=await _db_context.PlayerBlockList.FindAsync(request.Integer);
						break;
					case Proto.ServerMediator.Field.RegistrationDate:
						playerblocklist_model=await _db_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Datetime_Begin==request.Timestamp.ToDateTime());
						break;
					case Proto.ServerMediator.Field.LoginTimestamp:
						playerblocklist_model=await _db_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Datetime_End==request.Timestamp.ToDateTime());
						break;
					case Proto.ServerMediator.Field.LoginLastDatetime:
						playerblocklist_model=await _db_context.PlayerBlockList.SingleOrDefaultAsync(playerblocklist => playerblocklist.Block_Info==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(playerblocklist_model!=null)
				{
					_db_context.Remove(playerblocklist_model);
					await _db_context.SaveChangesAsync();
					return information_reply;
				}
				break;
			case Table.PlayerSecurityquestion:
				Proto.ServerMediator.Player_SecurityQuestion? player_securityquestion_message = null;
				Models.Monitoring.Player_SecurityQuestion? player_securityquestion_model = new Models.Monitoring.Player_SecurityQuestion();
				switch(request.DeleteBy)
				{
					case Proto.ServerMediator.Field.Id:
						player_securityquestion_model=await _db_context.Player_SecurityQuestion.FindAsync(request.Integer);
						break;
					case Proto.ServerMediator.Field.SecurityAnswer:
						player_securityquestion_model=await _db_context.Player_SecurityQuestion.SingleOrDefaultAsync(player_securityquestion => player_securityquestion.Security_Answer==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(player_securityquestion_model!=null)
				{
					_db_context.Remove(player_securityquestion_model);
					await _db_context.SaveChangesAsync();
					return information_reply;
				}
				break;
			case Table.Securityquestion:
				Proto.ServerMediator.SecurityQuestion? securityquestion_message = null;
				Models.Monitoring.SecurityQuestion? securityquestion_model = new Models.Monitoring.SecurityQuestion();
				switch(request.DeleteBy)
				{
					case Proto.ServerMediator.Field.Id:
						securityquestion_model=await _db_context.SecurityQuestion.FindAsync(request.Integer);
						break;
					case Proto.ServerMediator.Field.Login:
						securityquestion_model=await _db_context.SecurityQuestion.SingleOrDefaultAsync(securityquestion => securityquestion.Question==request.Str);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if(securityquestion_model!=null)
				{
					_db_context.Remove(securityquestion_model);
					await _db_context.SaveChangesAsync();
					return information_reply;
				}
				break;
		}

		information_reply.Information=false;
		information_reply.Details="ERROR: Entry not found.";

		return information_reply;
	}
	public override async Task<InformationReply> updateInDatabase(UpdateRequest request, ServerCallContext context)
	{
		InformationReply information_reply = new InformationReply() { Information=true, Details = "Entry successfully deleted." };
		bool unpack_result;

		switch(request.UpdateIn)
		{
			case Table.Transformer:
				Models.Monitoring.Transformer? player_model;

				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.Transformer player_message);
				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack Transformer!";
					break;
				}

				player_model=await _db_context.Transformer.FindAsync(player_message.Id);
				if(player_model==null)
				{
					information_reply.Details="ERROR: Failed to find Transformer!";
					break;
				}
				player_model.Login=player_message.Login;
				player_model.Password=player_message.Password.ToBase64();
				try
				{
					_db_context.Transformer.Update(player_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return information_reply;
			case Table.Playerinfo:
				Models.Monitoring.PlayerInfo? playerinfo_model = new Models.Monitoring.PlayerInfo();
				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.PlayerInfo playerinfo_message);

				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack playerinfo!";
					break;
				}

				playerinfo_model=await _db_context.PlayerInfo.FindAsync(playerinfo_message.Id);
				if(playerinfo_model==null)
				{
					information_reply.Details="ERROR: Failed to find playerinfo!";
					break;
				}
				playerinfo_model.Login_Timestamp=playerinfo_message.LoginTimestamp.ToDateTime();
				playerinfo_model.Registration_Date=playerinfo_message.RegistrationDate.ToDateTime();
				playerinfo_model.Login_Last_Datetime=playerinfo_model.Login_Last_Datetime;

				try
				{
					_db_context.PlayerInfo.Update(playerinfo_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return information_reply;
			case Table.Playerblocklist:
				Models.Monitoring.PlayerBlockList? playerblocklist_model = new Models.Monitoring.PlayerBlockList();
				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.PlayerBlockList playerblocklist_message);

				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack playerblocklist!";
					break;
				}
				playerblocklist_model=await _db_context.PlayerBlockList.FindAsync(playerblocklist_message.Id);
				if(playerblocklist_model==null)
				{
					information_reply.Details="ERROR: Failed to find playerblocklist!";
					break;
				}
				playerblocklist_model.Block_Datetime_Begin=playerblocklist_message.BlockDatetimeBegin.ToDateTime();
				playerblocklist_model.Block_Datetime_End=playerblocklist_message.BlockDatetimeEnd.ToDateTime();
				playerblocklist_model.Block_Info=playerblocklist_message.BlockInfo;

				try
				{
					_db_context.PlayerBlockList.Update(playerblocklist_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return information_reply;
			case Table.Securityquestion:
				Models.Monitoring.SecurityQuestion? securityquestion_model = new Models.Monitoring.SecurityQuestion();

				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.SecurityQuestion securityquestion_message);
				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack securityquestion!";
					break;
				}

				securityquestion_model=await _db_context.SecurityQuestion.FindAsync(securityquestion_message.Id);
				if(securityquestion_model==null)
				{
					information_reply.Details="ERROR: Failed to find securityquestion!";
					break;
				}
				securityquestion_model.Question=securityquestion_message.Question;

				try
				{
					_db_context.SecurityQuestion.Update(securityquestion_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return information_reply;
			case Table.PlayerSecurityquestion:
				Models.Monitoring.Player_SecurityQuestion? player_securityquestion_model = new Models.Monitoring.Player_SecurityQuestion();
				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.Player_SecurityQuestion player_securityquestion_message);

				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack player_securityquestion!";
					break;
				}
				player_securityquestion_model=await _db_context.Player_SecurityQuestion.FindAsync(player_securityquestion_message.Id);
				if(player_securityquestion_model==null)
				{
					information_reply.Details="ERROR: Failed to find player_securityquestion!";
					break;
				}
				player_securityquestion_model.Security_Answer=player_securityquestion_message.SecurityAnswer;
				try
				{
					_db_context.Player_SecurityQuestion.Update(player_securityquestion_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return information_reply;
		}
		information_reply.Information=false;

		return information_reply;
	}
	public override async Task<InformationReply> insertToDatabase(InsertRequest request, ServerCallContext context)
	{
		InformationReply information_reply = new InformationReply() { Information = true, Details = "Entry successfully inserted." };
		bool unpack_result;

		switch(request.InsertTo)
		{
			case Table.Transformer:
				Models.Monitoring.Transformer player_model = new Models.Monitoring.Transformer();

				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.Transformer player_message);
				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack Transformer!";
					break;
				}

				player_model.id=player_message.Id;
				player_model.Login=player_message.Login;
				player_model.Password=player_message.Password.ToBase64();
				try
				{
					await _db_context.Transformer.AddAsync(player_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return information_reply;
			case Table.Playerinfo:
				Models.Monitoring.PlayerInfo playerinfo_model = new Models.Monitoring.PlayerInfo();
				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.PlayerInfo playerinfo_message);

				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack playerinfo!";
					break;
				}

				playerinfo_model.id=playerinfo_message.Id;
				playerinfo_model.Transformer=new Models.Monitoring.Transformer {
					Id=playerinfo_message.Transformer.Id,
					Login=playerinfo_message.Transformer.Login,
					Password=playerinfo_message.Transformer.Password.ToBase64()
				};
				playerinfo_model.Login_Timestamp=playerinfo_message.LoginTimestamp.ToDateTime();
				playerinfo_model.Registration_Date=playerinfo_message.RegistrationDate.ToDateTime();
				playerinfo_model.Login_Last_Datetime=playerinfo_model.Login_Last_Datetime;

				try
				{
					await _db_context.PlayerInfo.AddAsync(playerinfo_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return information_reply;
			case Table.Playerblocklist:
				Models.Monitoring.PlayerBlockList playerblocklist_model = new Models.Monitoring.PlayerBlockList();
				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.PlayerBlockList playerblocklist_message);

				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack playerblocklist!";
					break;
				}
				playerblocklist_model.id=playerblocklist_message.Id;
				playerblocklist_model.Transformer=new Models.Monitoring.Transformer {
					Id=playerblocklist_message.Transformer.Id,
					Login=playerblocklist_message.Transformer.Login,
					Password=playerblocklist_message.Transformer.Password.ToBase64()
				};
				playerblocklist_model.Block_Datetime_Begin=playerblocklist_message.BlockDatetimeBegin.ToDateTime();
				playerblocklist_model.Block_Datetime_End=playerblocklist_message.BlockDatetimeEnd.ToDateTime();
				playerblocklist_model.Block_Info=playerblocklist_message.BlockInfo;

				try
				{
					await _db_context.PlayerBlockList.AddAsync(playerblocklist_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}

				return information_reply;
			case Table.Securityquestion:
				Models.Monitoring.SecurityQuestion securityquestion_model = new Models.Monitoring.SecurityQuestion();

				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.SecurityQuestion securityquestion_message);
				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack securityquestion!";
					break;
				}

				securityquestion_model.id=securityquestion_message.Id;
				securityquestion_model.Question=securityquestion_message.Question;

				try
				{
					await _db_context.SecurityQuestion.AddAsync(securityquestion_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return information_reply;
			case Table.PlayerSecurityquestion:
				Models.Monitoring.Player_SecurityQuestion player_securityquestion_model = new Models.Monitoring.Player_SecurityQuestion();
				unpack_result=request.Entry.TryUnpack(out Proto.ServerMediator.Player_SecurityQuestion player_securityquestion_message);

				if(!unpack_result)
				{
					information_reply.Details="ERROR: Failed to unpack player_securityquestion!";
					break;
				}
				player_securityquestion_model.id=player_securityquestion_message.Id;
				player_securityquestion_model.Transformer=new Models.Monitoring.Transformer {
					Id=player_securityquestion_message.Transformer.Id,
					Login=player_securityquestion_message.Transformer.Login,
					Password=player_securityquestion_message.Transformer.Password.ToBase64()
				};
				player_securityquestion_model.Security_Answer=player_securityquestion_message.SecurityAnswer;
				player_securityquestion_model.SecurityQuestion=new Models.Monitoring.SecurityQuestion() {
					Id=player_securityquestion_message.SecurityQuestion.Id,
					Question=player_securityquestion_message.SecurityQuestion.Question
				};
				try
				{
					await _db_context.Player_SecurityQuestion.AddAsync(player_securityquestion_model);
					await _db_context.SaveChangesAsync();
				}
				catch(Exception e)
				{
					_logger.LogInformation(nameof(insertToDatabase), e);
				}
				return information_reply;
		}
		information_reply.Information=false;

		return information_reply;
	}*/
}
