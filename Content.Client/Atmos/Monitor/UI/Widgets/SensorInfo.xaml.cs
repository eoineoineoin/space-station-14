using Content.Client.Message;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Temperature;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Atmos.Monitor.UI.Widgets;

[GenerateTypedNameReferences]
public sealed partial class SensorInfo : BoxContainer
{
    public Action<string, AtmosMonitorThresholdType, AtmosAlarmThreshold, Gas?>? OnThresholdUpdate;
    private string _address;

    private ThresholdControl _pressureThreshold;
    private ThresholdControl _temperatureThreshold;
    private Dictionary<Gas, ThresholdControl> _gasThresholds = new();
    private Dictionary<Gas, Label> _gasLabels = new();

    public SensorInfo(AtmosSensorData data, string address)
    {
        RobustXamlLoader.Load(this);

        _address = address;

        SensorAddress.Title = $"{address} : {data.AlarmState}";

        PressureLabel.SetMarkup(Loc.GetString("air-alarm-ui-window-pressure-indicator",
                    ("color", ColorForThreshold(data.Pressure, data.PressureThreshold).ToHex()),
                    ("pressure", $"{data.Pressure:0.##}")));
        TemperatureLabel.SetMarkup(Loc.GetString("air-alarm-ui-window-temperature-indicator",
                ("color", ColorForThreshold(data.Temperature, data.TemperatureThreshold).ToHex()),
                ("tempC", $"{TemperatureHelpers.KelvinToCelsius(data.Temperature):0.#}"),
                ("temperature", $"{data.Temperature:0.##}")));
        AlarmStateLabel.SetMarkup(Loc.GetString("air-alarm-ui-window-alarm-state", ("state", $"{data.AlarmState}")));

        foreach (var (gas, amount) in data.Gases)
        {
            var label = new Label();

            var fractionGas = amount / data.TotalMoles;
            label.FontColorOverride = ColorForThreshold(fractionGas, data.GasThresholds[gas]);
            label.Text = Loc.GetString("air-alarm-ui-gases", ("gas", $"{gas}"),
                ("amount", $"{amount:0.####}"),
                ("percentage", $"{(100 * fractionGas):0.##}"));
            GasContainer.AddChild(label);
            _gasLabels.Add(gas, label);
        }

        _pressureThreshold =
            new ThresholdControl(Loc.GetString("air-alarm-ui-thresholds-pressure-title"), data.PressureThreshold, AtmosMonitorThresholdType.Pressure);
        PressureThresholdContainer.AddChild(_pressureThreshold);
        _temperatureThreshold = new ThresholdControl(Loc.GetString("air-alarm-ui-thresholds-temperature-title"), data.TemperatureThreshold,
            AtmosMonitorThresholdType.Temperature);
        TemperatureThresholdContainer.AddChild(_temperatureThreshold);

        _pressureThreshold.ThresholdDataChanged += (type, threshold, arg3) =>
        {
            OnThresholdUpdate!(_address, type, threshold, arg3);
        };

        _temperatureThreshold.ThresholdDataChanged += (type, threshold, arg3) =>
        {
            OnThresholdUpdate!(_address, type, threshold, arg3);
        };

        foreach (var (gas, threshold) in data.GasThresholds)
        {
            var gasThresholdControl = new ThresholdControl(Loc.GetString($"air-alarm-ui-thresholds-gas-title", ("gas", $"{gas}")), threshold, AtmosMonitorThresholdType.Gas, gas, 100);
            gasThresholdControl.ThresholdDataChanged += (type, threshold, arg3) =>
            {
                OnThresholdUpdate!(_address, type, threshold, arg3);
            };

            _gasThresholds.Add(gas, gasThresholdControl);
            GasThresholds.AddChild(gasThresholdControl);
        }
    }

    public void ChangeData(AtmosSensorData data)
    {
        SensorAddress.Title = $"{_address} : {data.AlarmState}";

        PressureLabel.SetMarkup(Loc.GetString("air-alarm-ui-window-pressure-indicator",
                    ("color", ColorForThreshold(data.Pressure, data.PressureThreshold)),
                    ("pressure", $"{data.Pressure:0.##}")));
        TemperatureLabel.SetMarkup(Loc.GetString("air-alarm-ui-window-temperature-indicator",
                ("color", ColorForThreshold(data.Temperature, data.TemperatureThreshold)),
                ("tempC", $"{TemperatureHelpers.KelvinToCelsius(data.Temperature):0.#}"),
                ("temperature", $"{data.Temperature:0.##}")));

        AlarmStateLabel.SetMarkup(Loc.GetString("air-alarm-ui-window-alarm-state", ("state", $"{data.AlarmState}")));

        foreach (var (gas, amount) in data.Gases)
        {
            if (!_gasLabels.TryGetValue(gas, out var label))
            {
                continue;
            }

            var fractionGas = amount / data.TotalMoles;
            label.Text = Loc.GetString("air-alarm-ui-gases", ("gas", $"{gas}"),
                ("amount", $"{amount:0.####}"),
                ("percentage", $"{(100 * fractionGas):0.##}"));
            label.FontColorOverride = ColorForThreshold(fractionGas, data.GasThresholds[gas]);
        }

        _pressureThreshold.UpdateThresholdData(data.PressureThreshold);
        _temperatureThreshold.UpdateThresholdData(data.TemperatureThreshold);
        foreach (var (gas, control) in _gasThresholds)
        {
            if (!data.GasThresholds.TryGetValue(gas, out var threshold))
            {
                continue;
            }

            control.UpdateThresholdData(threshold);
        }
    }

    private Color ColorForThreshold(float amount, AtmosAlarmThreshold threshold)
    {
        threshold.CheckThreshold(amount, out AtmosAlarmType curAlarm);
        if(curAlarm == AtmosAlarmType.Danger)
        {
            return new Color(1f, 0, 0);
        }
        else if(curAlarm == AtmosAlarmType.Warning)
        {
            return new Color(1f, 1f, 0);
        }

        return new Color(1f, 1f, 1f);
    }

 }
