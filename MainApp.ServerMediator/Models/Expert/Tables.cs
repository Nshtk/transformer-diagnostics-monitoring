namespace MainApp.ServerMediator.Models.Expert;

public class Documentation
{
	public long Id
	{
		get;
		set;
	}
	public string Name
	{
		get;
		set;
	}
}
public class Transformer
{
	public long Id
	{
		get;
		set;
	}
	public long Documentation_Id
	{
		get;
		set;
	}
	public string Name
	{
		get;
		set;
	}
}
public class Element
{
	public long Id
	{
		get;
		set;
	}
	public string Name
	{
		get;
		set;
	}
	public string Description
	{
		get;
		set;
	}
}
public class Parameter
{
	public long Id
	{
		get;
		set;
	}
	public long Element_Id
	{
		get;
		set;
	}
	public string Name
	{
		get;
		set;
	}
	public string Description
	{
		get;
		set;
	}
}
public class Defect
{
	public long Id
	{
		get;
		set;
	}
	public string Name
	{
		get;
		set;
	}
	public string Description
	{
		get;
		set;
	}
}
public class Element_Documentation
{
	public long Element_Id
	{
		get;
		set;
	}
	public long Documentation_Id
	{
		get;
		set;
	}
}
public class Defect_Documentation
{
	public long Id
	{
		get;
		set;
	}
	public long Documentation_Id
	{
		get;
		set;
	}
	public string Recomendation
	{
		get;
		set;
	}
}
