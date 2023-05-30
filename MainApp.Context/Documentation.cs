namespace MainApp.Context;

public class Documentation
{
	public enum LimitMode
	{
		GREATER,
		LESSER
	}

	public struct Defect
	{
		public int id;
		public int[]? dependecies_id;
		public bool IsActive
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public Defect(int id, string name, int[]? dependecies_id)
		{
			this.id=id;
			Name=name;
			IsActive=false;
			this.dependecies_id= dependecies_id;
		}
	}

	public interface IUnit
	{
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
	public struct Parameter<T>: IUnit
	{
		public LimitMode limit_mode;
		public T[] bounds;
		public bool degradation_is_linear;

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
		public bool IsEditable
		{
			get;
			set;
		}

		public Parameter(string name, string description, LimitMode limit_mode, T[] bounds, bool is_editable=false, bool degradation_is_linear=true)
		{
			Name = name;
			Description = description;
			this.limit_mode = limit_mode;
			this.bounds = bounds;
			IsEditable = is_editable;
			this.degradation_is_linear=degradation_is_linear;

		}
	}
	public struct ParameterGroup
	{
		public List<IUnit> parameters;

		public float Weight
		{
			get;
			set;
		}

		public ParameterGroup(float weight, List<IUnit> parameters)
		{
			Weight = weight;
			this.parameters=parameters;
		}
	}
	public struct Element : IUnit
	{
		public Dictionary<int, Defect> parameters_defects;
		public List<ParameterGroup> parameter_groups;

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
		public float Weight
		{
			get;
			set;
		}

		public Element(string name, string description, float weight, Dictionary<int, Defect> parameters_defects, List<ParameterGroup> parameter_groups)
		{
			Name=name;
			Description=description;
			Weight = weight;
			this.parameters_defects=parameters_defects;
			this.parameter_groups = parameter_groups;
		}
	}

	#region Fields
	public readonly List<Element> elements;
	public readonly string name;
	public readonly string description;
	#endregion

	#region Properties
	public string Name
	{ 
		get { return name; } 
	}
	#endregion
	public Documentation(string name, string description)
	{
		this.name = name;
		this.description = description;
		elements = new List<Element> {
			new Element("Качество масла", "", 0.2f, new Dictionary<int, Defect> { {1234, new Defect(0, "Частичные разряды", null)} },
				new List<ParameterGroup> {
					new ParameterGroup (0.5f, new List<IUnit> {
						new Parameter<double>("Концентрация H2", "",    LimitMode.GREATER, new double[] { 0.009, 0.01, 0.011}),
						new Parameter<double>("Концентрация CH4", "",   LimitMode.GREATER, new double[] { 0.009, 0.01, 0.011 }),
						new Parameter<double>("Концентрация C2H2", "",  LimitMode.GREATER, new double[] { 0, 0.001, 0.0011 }),
						new Parameter<double>("Концентрация C2H4", "",  LimitMode.GREATER, new double[] { 0.009, 0.01, 0.011 }),
						new Parameter<double>("Концентрация C2H6", "",  LimitMode.GREATER, new double[] { 0, 0.005, 0.0051 }),
						new Parameter<double>("Концентрация CO", "",    LimitMode.GREATER, new double[] { 0.0049, 0.005, 0.0051 }),
						new Parameter<double>("Концентрация CO2", "",   LimitMode.GREATER, new double[] { 0.059, 0.06, 0.061 }),
					}),
					new ParameterGroup (0.5f, new List<IUnit> {
						new Parameter<double>("Кислотное число", "",   LimitMode.GREATER, new double[] { 0.07, 0.013, 0.019, 0.25}),
					})
				}
			),
			new Element("Обмотки", "", 0.1f, new Dictionary<int, Defect> { {1, new Defect(1, "Увлажнение", null)}, { 2, new Defect(11, "Деформация при коротком замыкании", new int[1] {2})}, },
				new List<ParameterGroup> {
					new ParameterGroup (1.0f, new List<IUnit> {
						new Parameter<double>("Влагосодержание", "", LimitMode.GREATER, new double[] {1, 2, 3.9, 4}),
					})
				}
			),
			new Element("Изоляция", "", 0.2f, new Dictionary<int, Defect> { {1, new Defect(2, "Загрязнение", null) }, {2, new Defect(3, "Перегрев", null)}, { 0, new Defect(12, "Дугообразование", new int[2] { 2, 4 }) }, { 12, new Defect(13, "Старение", new int[2] { 1, 9 }) }, },
				new List<ParameterGroup> {
					new ParameterGroup (0.6f, new List<IUnit> {
						new Parameter<double>("Концентрация фурановых соединений", "", LimitMode.GREATER, new double[] { 1.84, 3.27, 5.52, 7.1}),
					}),
					new ParameterGroup (0.4f, new List<IUnit> {
						new Parameter<int>("Температура", "", LimitMode.GREATER, new int[] {40, 50, 55, 75}),
					}),
				}
			),
			new Element("Высоковольтные вводы", "", 0.1f, new Dictionary<int, Defect> { {1, new Defect(4, "Низкая сопротивляемость", null)} },
				new List<ParameterGroup> {
					new ParameterGroup (1.0f, new List<IUnit> {
						new Parameter<int>("Тангенс угла диэлектрических потерь", "", LimitMode.GREATER, new int[] {8, 9, 10, 12}),
					})
				}
			),
			new Element("Устройства РПН", "", 0.1f, new Dictionary<int, Defect> { {1, new Defect(5, "Нарушение герметичности", null)} },
				new List<ParameterGroup> {
					new ParameterGroup (1.0f, new List<IUnit> {
						new Parameter<int>("Пробивное напряжение", "", LimitMode.LESSER, new int[] {50, 48, 47, 45}, degradation_is_linear:false),
					})
				}
			),
			new Element("Магнитопровод", "", 0.2f, new Dictionary<int, Defect> { { 1, new Defect(6, "Увеличение потерь", null) }, { 2, new Defect(7, "Перегрев", null)}, { 12, new Defect(14, "Короткозамкнутые контуры", new int[2] { 0, 11 }) } },
				new List<ParameterGroup> {
					new ParameterGroup (0.5f, new List<IUnit> {
						new Parameter<int>("Потери холостого хода", "", LimitMode.GREATER, new int[] {28, 30, 35, 40}),
					}),
					new ParameterGroup (0.3f, new List<IUnit> {
						new Parameter<double>("Уровень перенапряжений", "", LimitMode.LESSER, new double[] {0.2, 0.4, 0.6, 0.8}, degradation_is_linear:false),
					}),
					new ParameterGroup (0.2f, new List<IUnit> {
						new Parameter<int>("Уровень вибрации", "", LimitMode.GREATER, new int[] {5, 8, 10}),
					})
				}
			),
			new Element("Система охлаждения", "", 0.05f, new Dictionary<int, Defect> { {1, new Defect(8, "Низкая производительность", null)} },
				new List<ParameterGroup> {
					new ParameterGroup (1.0f, new List<IUnit> {
						new Parameter<int>("Эффективность", "", LimitMode.LESSER, new int[] {100, 65, 30, 1}, true),
					})
				}
			),
			new Element("Общие испытания", "", 0.05f, new Dictionary<int, Defect> { { 1, new Defect(9, "Старение", null) }, { 2, new Defect(10, "Перегрев", null)} },
				new List<ParameterGroup> {
					new ParameterGroup (0.5f, new List<IUnit> {
						new Parameter<double>("Срок службы", "", LimitMode.GREATER, new double[] {0.13, 0.57, 1, 1.85}),
					}),
					new ParameterGroup (0.5f, new List<IUnit> {
						new Parameter<int>("Температура", "", LimitMode.GREATER, new int[] {50, 60, 65, 70}),
					})
				}
			),
		};
	}
}
