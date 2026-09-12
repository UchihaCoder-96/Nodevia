using Nodevia.Execution;
using System.Globalization;
using System.Windows.Data;

namespace Nodevia.Demo
{
    public class ConstantBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            var outputs = new NodeOutputs();
            outputs.Set("Result", inputs.Get<double>("Value"));
            return outputs;
        }
    }

    public class AddBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            var outputs = new NodeOutputs();
            outputs.Set("Result", inputs.Get<double>("A") + inputs.Get<double>("B"));
            return outputs;
        }
    }

    public class SubtractBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            var outputs = new NodeOutputs();
            outputs.Set("Result", inputs.Get<double>("A") - inputs.Get<double>("B"));
            return outputs;
        }
    }

    public class MultiplyBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            var outputs = new NodeOutputs();
            outputs.Set("Result", inputs.Get<double>("A") * inputs.Get<double>("B"));
            return outputs;
        }
    }

    public class DivideBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double a = inputs.Get<double>("A");
            double b = inputs.Get<double>("B");

            if (b == 0)
                throw new InvalidOperationException("Division by zero.");

            var outputs = new NodeOutputs();
            outputs.Set("Result", a / b);
            return outputs;
        }
    }

    public class PowerBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            var outputs = new NodeOutputs();
            outputs.Set("Result", Math.Pow(inputs.Get<double>("Base"), inputs.Get<double>("Exponent")));
            return outputs;
        }
    }

    public class ModuloBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double a = inputs.Get<double>("A");
            double b = inputs.Get<double>("B");

            if (b == 0)
                throw new InvalidOperationException("Modulo by zero.");

            var outputs = new NodeOutputs();
            outputs.Set("Result", a % b);
            return outputs;
        }
    }

    public class SqrtBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double value = inputs.Get<double>("Value");

            if (value < 0)
                throw new InvalidOperationException("Cannot take the square root of a negative number.");

            var outputs = new NodeOutputs();
            outputs.Set("Result", Math.Sqrt(value));
            return outputs;
        }
    }

    public class NegateBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            var outputs = new NodeOutputs();
            outputs.Set("Result", -inputs.Get<double>("Value"));
            return outputs;
        }
    }

    public class DisplayBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs) => new NodeOutputs();
    }

    public class PortResultConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3)
                return "—";

            bool isConnected = values[0] is true;
            object value = isConnected ? values[1] : values[2];

            return value switch
            {
                null => "—",
                double d => d.ToString("0.####"),
                float f => f.ToString("0.####"),
                _ => value.ToString() ?? "—"
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}


