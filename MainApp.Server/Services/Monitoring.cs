using Google.Protobuf.Collections;
using Grpc.Core;
using MainApp.Context.Utility;
using MainApp.Server;
using Proto;
using Proto.Monitoring;
using System;

namespace MainApp.Server.Services;
public class ServiceMonitoring : Monitoring.MonitoringBase
{
	public ServiceMonitoring()
	{
	}

	public void generateParameterValues<T>(RepeatedField<ParameterValueData> field, Func<dynamic, dynamic, T> getRandomValue, T min, T max, int steps_number=0, int change_direction_chance=5, bool is_ascending = true) 
	where T: IComparable
	{
		T value, value_previous = getRandomValue(min, max);
		T step=steps_number!=0 ?((dynamic)max-min)/steps_number :0;

		for(int i=0; i<30; i++)
		{
			ParameterValueData parameter_value_data = new ParameterValueData();
			dynamic value_next_limit;

			if(is_ascending)
			{
				if(Utility.Random.Next(change_direction_chance)==0)
				{
					value=getRandomValue(min, value_previous);
					is_ascending=!is_ascending;
				}
				else
				{
					if(step.CompareTo((dynamic)0)!=0)
					{
						value_next_limit=(dynamic)value_previous+step;
						if(value_next_limit>=max)
							value_next_limit=value_previous;
					}
					else
						value_next_limit=max;
					value=getRandomValue(value_previous, value_next_limit);
				}
			}		
			else
			{
				if(Utility.Random.Next(change_direction_chance)==0)
				{
					value=getRandomValue(value_previous, max);
					is_ascending=!is_ascending;
				}
				else
				{
					if(step.CompareTo((dynamic)0)!=0)
					{
						value_next_limit=(dynamic)value_previous-step;
						if(value_next_limit<=min)
							value_next_limit=value_previous;
					}
					else
						value_next_limit=min;
					value=getRandomValue(value_next_limit, value_previous);
				}
			}
				
			if(value is double)
				parameter_value_data.Value=Convert.ToDouble(value);
			else
				parameter_value_data.Value=Convert.ToInt32(value);

			parameter_value_data.Date=i;
			field.Add(parameter_value_data);
			value_previous=value;
		}
	}

	public override Task<GetParametersReply> getParameters(Request request, ServerCallContext context)
	{
		GetParametersReply reply = new GetParametersReply();

		if(ServiceConnection.Users[request.Id].monitoring_enabled == false)
		{
			reply.Information=new InformationReply {
				Result = false,
				Details="Monitoring disabled.",
				Type=InformationReply.Types.Message_Type.Warning
			};
		}
		else
		{
			reply.Information=new InformationReply {
				Result = true,
				Details="Monitoring parameters received.",
				Type=InformationReply.Types.Message_Type.Ordinary
			};
		}

		reply.OilConcentrationH2			=Math.Round(Utility.Random.NextDouble()*(0.1-0.005)+0.005, 3);
		reply.OilConcentrationCh4			=Math.Round(Utility.Random.NextDouble()*(0.1-0.005)+0.005, 3);
		reply.OilConcentrationC2H2			=Math.Round(Utility.Random.NextDouble()*(0.1-0.005)+0.005, 3);
		reply.OilConcentrationC2H4			=Math.Round(Utility.Random.NextDouble()*(0.1-0.005)+0.005, 3);
		reply.OilConcentrationC2H6			=Math.Round(Utility.Random.NextDouble()*(0.1-0.005)+0.005, 3);
		reply.OilConcentrationCo			=Math.Round(Utility.Random.NextDouble()*(0.1-0.005)+0.005, 3);
		reply.OilConcentrationCo2			=Math.Round(Utility.Random.NextDouble()*(0.1-0.005)+0.005, 3);
		reply.OilAcidNumber					=Math.Round(Utility.Random.NextDouble()*(0.35-0.04)+0.04, 3);
		reply.WindingsHumidity				=Math.Round(Utility.Random.NextDouble()*(0.35-0.04)+0.04, 3);
		reply.InsulationFuranConcentration	=Math.Round(Utility.Random.NextDouble()*(9-1.3)+9, 3);
		reply.InsulationTemperature			=Utility.Random.Next(0, 100);
		reply.BushingsElectricalLossTangent	=Utility.Random.Next(1, 15);
		reply.HvrBreakVoltage				=Utility.Random.Next(30, 60);
		reply.MagneticCoreIdleLoss			=Utility.Random.Next(20, 50);
		reply.MagneticCorePartialDischarges	=Math.Round(Utility.Random.NextDouble(), 3);
		reply.MagneticCoreVibration			=Utility.Random.Next(0, 15);
		reply.CoolingSystemEfficiency		=Utility.Random.Next(0, 100);
		reply.CommonLifeTime				=Math.Round(Utility.Random.NextDouble()*(2.5+5)-5, 3);
		reply.CommonTemperature				=Utility.Random.Next(0, 100);


		return Task.FromResult(reply);
	}
	public override Task<getParameterValuesPastReply> getParameterValuesPast(Request request, ServerCallContext context)
	{
		getParameterValuesPastReply reply = new getParameterValuesPastReply();

		reply.Information=new InformationReply {
			Result = true,
			Details="Monitoring parameters from past 30 days received.",
			Type=InformationReply.Types.Message_Type.Ordinary
		};

		var getRandomDouble = (dynamic min, dynamic max) => { return min>max? (double)max :(double)(Utility.Random.NextDouble()*(max-min)+min); };
		var getRandomInt = (dynamic min, dynamic max) => { return min>max ? (int)max :(int)Utility.Random.Next(min, max); };

		generateParameterValues(reply.ValuesPastOilConcentrationH2, getRandomDouble, 0.007, 0.011, 200, 1000);
		generateParameterValues(reply.ValuesPastOilConcentrationCh4, getRandomDouble, 0.007, 0.011, 200, 1000);
		generateParameterValues(reply.ValuesPastOilConcentrationC2H2, getRandomDouble, 0.005, 0.1, 60, 1000);
		generateParameterValues(reply.ValuesPastOilConcentrationC2H4, getRandomDouble, 0.007, 0.011, 200, 1000);
		generateParameterValues(reply.ValuesPastOilConcentrationC2H6, getRandomDouble, 0.005, 0.1, 60, 1000);
		generateParameterValues(reply.ValuesPastOilConcentrationCo, getRandomDouble, 0.005, 0.1, 10, 1000);
		generateParameterValues(reply.ValuesPastOilConcentrationCo2, getRandomDouble, 0.059, 0.061, 200, 1000);
		generateParameterValues(reply.ValuesPastOilAcidNumber, getRandomDouble, 0.04, 0.35);
		generateParameterValues(reply.ValuesPastWindingsHumidity, getRandomDouble, 0.04, 0.35, 30, 1000);
		generateParameterValues(reply.ValuesPastInsulationFuranConcentration, getRandomDouble, 0.5, 2, 10, 1000); //
		generateParameterValues(reply.ValuesPastInsulationTemperature, getRandomInt, 0, 100);
		generateParameterValues(reply.ValuesPastBushingsElectricalLossTangent,getRandomInt, 1, 15);
		generateParameterValues(reply.ValuesPastHvrBreakVoltage, getRandomInt, 20, 40, 20, 100); // 30 60
		generateParameterValues(reply.ValuesPastMagneticCoreIdleLoss, getRandomInt, 20, 50);
		generateParameterValues(reply.ValuesPastMagneticCorePartialDischarges, getRandomDouble, 0, 0.2, 20, 50); // 0 1
		generateParameterValues(reply.ValuesPastMagneticCoreVibration, getRandomInt, 0, 15);
		generateParameterValues(reply.ValuesPastCoolingSystemEfficiency, getRandomInt, 0, 100);
		generateParameterValues(reply.ValuesPastCommonLifeTime, getRandomDouble, 2.5, -5);
		generateParameterValues(reply.ValuesPastCommonTemperature, getRandomInt, 0, 100);

		return Task.FromResult(reply);
	}
	public override Task<SendEditedParametersReply> sendEditedParameters(SendEditedParametersRequest request, ServerCallContext context)
	{
		return base.sendEditedParameters(request, context);
	}
}
