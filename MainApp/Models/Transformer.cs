using System.Collections.ObjectModel;
using System;
using MathNet.Numerics.LinearAlgebra;
using MainApp.WPF.MVVM.ViewModel;
using MainApp.Context;
using MathNet.Numerics;
using System.Collections.Generic;

namespace MainApp.Models;

public interface IUnit
{
	public string Name
	{
		get;
		set;
	}
	public int Score
	{
		get;
		set;
	}

	public void calculateScore();
}

public class ModelParameter<T> : ModelBase, IUnit 
where T: IComparable<T>
{
	public struct PredictionData
	{
		public int[] date_pows;
		public T[] value_change_date_pows;

		public PredictionData(int date, T value_change, bool is_linear=true)
		{
			int date_pows_size;
			int value_change_pows_size;
			if(is_linear)
			{
				date_pows_size=2;
				value_change_pows_size = 1;
			}
			else
			{
				date_pows_size = 5;
				value_change_pows_size = 3;
			}
			date_pows = new int[date_pows_size];
			value_change_date_pows = new T[value_change_pows_size];
			for(int i = 0; i<date_pows_size;)
				date_pows[i]=(int)Math.Pow(date, ++i);
			for(int i = 0; i<value_change_pows_size;)
				value_change_date_pows[i]=(T)((dynamic)value_change*Math.Pow(date, ++i));
		}
	}

	private T? _value;
	private Documentation.LimitMode _limit_mode;
	private bool _is_off_limit;
	private T[] _bounds;
	public (int, T)[]? values_per_date;
	public PredictionData[] _prediction_data;
	private bool _degradation_is_linear;

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
	public T? Value
	{
		get { return _value; }
		set 
		{
			_value = value;
			if(_limit_mode==Documentation.LimitMode.LESSER)
			{
				if(_value?.CompareTo(_bounds[0])<0)
					IsOffLimit = true;
				else
					IsOffLimit = false;
			}
			else
			{
				if(_value?.CompareTo(_bounds[0])>0)
					IsOffLimit = true;
				else
					IsOffLimit = false;
			}
			invokePropertyChanged("Value");
		}
	}
	public T Limit
	{
		get;
		set;
	}

	public Documentation.LimitMode LimitMode
	{
		get { return _limit_mode; }
		private set { _limit_mode=value; }
	}
	public bool IsOffLimit
	{
		get { return _is_off_limit; }
		set 
		{
			_is_off_limit = value;
			if(_is_off_limit==true)
				calculateScore();
			else
				Score=4;
		}
	}
	public int Score
	{
		get;
		set;
	}

	public ModelParameter()
	{}
	public ModelParameter(string name, string description, bool is_editable, Documentation.LimitMode limit_mode, T[] bounds, bool degradation_is_linear=true)
	{
		Name = name;
		Description = description;
		IsEditable = is_editable;
		Limit=bounds[0];
		LimitMode = limit_mode;
		_bounds = bounds;
		_degradation_is_linear = degradation_is_linear;
	}

	public void calculateScore()
	{
		int score = 3;
		switch(LimitMode)
		{
			case Documentation.LimitMode.LESSER:
				for(int i=1; i<_bounds.Length; i++)
				{
					if(_value.CompareTo(_bounds[i])>0)
						break;
					score--;
				}
				break;
			case Documentation.LimitMode.GREATER:
				for(int i=1; i<_bounds.Length; i++)
				{
					if(_value.CompareTo(_bounds[i])<0)
						break;
					score--;
				}
				break;
		}
		Score = score;
	}
	public (int?, double?) predictState()
	{
		if(values_per_date==null)
			return (null, null);

		int[] summs_s;
		T[]	summs_t;
		Matrix<double> summs_s_matrix;
		Vector<double> a_vector;
		int limit_date;
		if(_degradation_is_linear)
		{
			summs_s=new int[2];
			summs_t=new T[1];
		}
		else
		{
			summs_s=new int[5];
			summs_t=new T[3];
		}

		_prediction_data=new PredictionData[values_per_date.Length];

		for(int i=0, j; i<values_per_date?.Length; i++)
		{
			_prediction_data[i]=new PredictionData(values_per_date[i].Item1, (dynamic)values_per_date[i].Item2-(dynamic)_bounds[0], _degradation_is_linear);
			for(j=0; j<summs_s.Length; j++)
				summs_s[j]+=_prediction_data[i].date_pows[j];
			for(j=0; j<summs_t.Length; j++)
				summs_t[j]+=(dynamic)_prediction_data[i].value_change_date_pows[j];
		}

		if(_degradation_is_linear)
		{
			limit_date=(int)Math.Round(Math.Abs((dynamic)_bounds[_bounds.Length-1]-_bounds[0])*summs_s[1]/summs_t[0]);
		}
		else
		{
			summs_s_matrix = Matrix<double>.Build.DenseOfArray(new double[,] {
				{ summs_s[0], summs_s[1], summs_s[2] },
				{ summs_s[1], summs_s[2], summs_s[3] },
				{ summs_s[2], summs_s[3], summs_s[4] }
			});
			a_vector=summs_s_matrix.Solve(Vector<double>.Build.Dense(new double[] { (dynamic)summs_t[0], (dynamic)summs_t[1], (dynamic)summs_t[2] }));
			var result = FindRoots.Cubic(-((dynamic)_bounds[_bounds.Length-1]-_bounds[0]), a_vector[0], a_vector[1], a_vector[2]);
			limit_date =(int)Math.Max(result.Item3.Real, Math.Max(result.Item1.Real, result.Item2.Real));
		}
		
		return (Math.Abs(limit_date), (dynamic)_bounds[_bounds.Length-1]);
	}

	public static explicit operator ModelParameter<T>(Documentation.Parameter<T> parameter)
	{
		return new ModelParameter<T>(parameter.Name, parameter.Description, parameter.IsEditable, parameter.limit_mode, parameter.bounds, parameter.degradation_is_linear);
	}
}
public class ModelParameterGroup
{
	public List<IUnit> parameters=new List<IUnit>();

	public float Weight
	{
		get;
		set;
	}

	public ModelParameterGroup(float weight)
	{
		Weight=weight;
	}

	public int getMinScore()
	{
		int score_min=4;
		foreach(var parameter in parameters)
			if(parameter.Score<score_min) 
				score_min=parameter.Score;
		return score_min;
	}
}
public class ModelElement : ModelBase, IUnit
{
	private int _score;
	private bool _has_active_defects;
	public List<ModelParameterGroup> parameter_groups=new List<ModelParameterGroup>();
	private ObservableCollection<IUnit> _parameters=new ObservableCollection<IUnit>();
	public Dictionary<int, Documentation.Defect> defects;

	public ObservableCollection<IUnit> Parameters
	{
		get { return _parameters; }
		set { _parameters = value; invokePropertyChanged("Parameters"); }
	}
	public Dictionary<int, Documentation.Defect> Defects
	{
		get { return defects; }
		set { defects = value; invokePropertyChanged("Defects"); }
	}

	public string Name
	{
		get;
		set;
	}
	public float Weight
	{
		get;
		set;
	}
	public int Score
	{
		get { return _score; }
		set { _score=value; invokePropertyChanged("Score"); }
	}
	public bool HasActiveDefects
	{
		get { return _has_active_defects; }
		set { _has_active_defects=value; invokePropertyChanged("HasActiveDefects"); }
	}

	public ModelElement(string name, float weight, Dictionary<int, Documentation.Defect> parameters_defects)
	{
		Name = name;
		Weight = weight;
		Defects=parameters_defects;
		Score = 100;
	}

	public void calculateScore()
	{
		float score=0;

		foreach(var parameter_group in parameter_groups)
			score+=(parameter_group.getMinScore()*parameter_group.Weight)/4;
			
		Score=(int)(score*100);
	}
	public bool trySetDefectState(int key, bool state)
	{
		if(defects.ContainsKey(key)) // && defects[key].IsActive!=state
		{
			Documentation.Defect defect_tmp = defects[key];
			defect_tmp.IsActive=state;
			defects[key]=defect_tmp;
			invokePropertyChanged("Defects");
			return true;
		}
		return false;
	}
}

public class ModelTransformer : ModelBase, IUnit
{
	#region Fields
	public readonly Documentation documentation;
	private ObservableCollection<ModelElement> _elements;
	private ObservableCollection<IUnit> _parameters_out_of_limits;
	private int _score;
	private bool _has_active_defects=false;
	private bool _is_loaded = false;
	#endregion

	#region Properties
	public Documentation Documentation
	{ 
		get { return documentation; }
	}
	public ObservableCollection<ModelElement> Elements
	{
		get { return _elements; }
		set { _elements = value; invokePropertyChanged("Elements"); }
	}
	public ObservableCollection<IUnit> Parameters_OutOfLimits
	{
		get { return _parameters_out_of_limits; }
		set { _parameters_out_of_limits = value; invokePropertyChanged("Parameters_OutOfLimits"); }
	}
	public bool IsLoaded
	{
		get { return _is_loaded; }
		set { _is_loaded = value; invokePropertyChanged("IsLoaded"); }
	}
	public string Name
	{
		get;
		set;
	}
	public int Score
	{
		get { return _score; }
		set { _score=value; invokePropertyChanged("Score"); }
	}
	public bool HasActiveDefects
	{
		get { return _has_active_defects; }
		set { _has_active_defects=value; invokePropertyChanged("HasActiveDefects"); }
	}
	#endregion

	public ModelTransformer(Documentation documentation)
	{
		this.documentation = documentation;
		Name = documentation.Name;
		Elements = new ObservableCollection<ModelElement>();
		foreach(var element in documentation.elements)
		{
			ModelElement element_model = new ModelElement(element.Name, element.Weight, element.parameters_defects);
			foreach(var parameter_group in element.parameter_groups)
			{
				ModelParameterGroup model_parameter_group = new ModelParameterGroup(parameter_group.Weight);
				foreach(var parameter in parameter_group.parameters)
				{
					object parameter_model = Activator.CreateInstance(typeof(ModelParameter<>).MakeGenericType(parameter.GetType().GetGenericArguments()[0]));
					if(parameter_model is ModelParameter<int>)
					{
						var parameter_casted = (ModelParameter<int>)(Documentation.Parameter<int>)parameter;
						model_parameter_group.parameters.Add(parameter_casted);
						element_model.Parameters.Add(parameter_casted);
					}
					else if(parameter_model is ModelParameter<double>)
					{
						var parameter_casted = (ModelParameter<double>)(Documentation.Parameter<double>)parameter;
						model_parameter_group.parameters.Add(parameter_casted);
						element_model.Parameters.Add(parameter_casted);
					}
				}
				element_model.parameter_groups.Add(model_parameter_group);
			}
			Elements.Add(element_model);
		}
	}

	public void invalidateParametersOutOfLimits()
	{
		int i, code_tmp;
		string code;
		bool result=false;
		ObservableCollection<IUnit> parameters_out_of_limits = new ObservableCollection<IUnit>();

		foreach(var element in _elements)
		{
			i = 1;
			code="";
			result=false;
			foreach(var parameter in element.Parameters)
			{
				if((bool)parameter.GetType().GetProperty("IsOffLimit").GetValue(parameter, null)==true)
				{
					parameters_out_of_limits.Add(parameter);
					code+=i.ToString();
					code_tmp = Int32.Parse(code);
					if(element.trySetDefectState(i, true) || element.trySetDefectState(code_tmp, true))
						result=true;
				}
				else
				{
					code+=i.ToString();
					code_tmp = Int32.Parse(code);
					element.trySetDefectState(i, false);
					element.trySetDefectState(code_tmp, false);
				}
				i++;
			}
			element.HasActiveDefects=result;
			if(result)
				_has_active_defects=true;
		}
		Parameters_OutOfLimits=parameters_out_of_limits;
		HasActiveDefects=_has_active_defects;
	}
	public void calculateScore()
	{
		_score=0;
		foreach(var element in Elements)
		{
			element.calculateScore();
			_score+=(int)(element.Score*element.Weight);
		}
		Score=_score;
	}
}
