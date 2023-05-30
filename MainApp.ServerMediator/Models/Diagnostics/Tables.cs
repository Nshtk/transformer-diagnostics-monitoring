namespace MainApp.ServerMediator.Models.Diagnostics;

public class Transformer
{
	public long Id
	{
		get;
		set;
	}
	public string Status
	{
		get;
		set;
	}
	public string Description
	{
		get;
		set;
	}
	public int StateScore
	{
		get;
		set;
	}
	public DateOnly NextMaintenance
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
}
public class Defect
{
	public long Id
	{
		get;
		set;
	}
}
public class Transformer_Element
{
	public long Transformer_Id
	{
		get;
		set;
	}
	public long Element_Id
	{
		get;
		set;
	}
	public int StateScore
	{
		get;
		set;
	}
}
public class Transformer_Element_Defect
{
	public long Id
	{
		get;
		set;
	}
	public long Transformer_Element_Transformer_Id
	{
		get;
		set;
	}
	public long Transformer_Element_Element_Id
	{
		get;
		set;
	}
	public string Description
	{
		get;
		set;
	}
	public string Status
	{
		get;
		set;
	}
}
