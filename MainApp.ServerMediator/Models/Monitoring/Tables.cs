namespace MainApp.ServerMediator.Models.Monitoring;

public class Building
{
	public long id
	{
		get;
		set;
	}
	public string name
	{
		get;
		set;
	}
}
public class Room
{
	public long id
	{
		get;
		set;
	}
	public long Building_id
	{
		get;
		set;
	}
	public string name
	{
		get;
		set;
	}
}
public class Transformer
{
	public long id
	{
		get;
		set;
	}
	public long Room_id
	{
		get;
		set;
	}
	public bool is_functioning
	{
		get;
		set;
	}
	public bool is_monitored
	{
		get;
		set;
	}
}
public class Sensor
{
	public long id
	{
		get;
		set;
	}
	public string name
	{
		get;
		set;
	}
	public string type
	{
		get;
		set;
	}
}
public class Sensor_Transformer
{
	public long id
	{
		get;
		set;
	}
	public long Sensor_id
	{
		get;
		set;
	}
	public long Transformer_id
	{
		get;
		set;
	}
	public bool is_functioning
	{
		get;
		set;
	}
	public double value
	{
		get;
		set;
	}
}