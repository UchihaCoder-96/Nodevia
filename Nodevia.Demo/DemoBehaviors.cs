using Nodevia.Execution;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Nodevia.Demo
{
    public class AddBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double a = inputs.Get<double>("A");
            double b = inputs.Get<double>("B");

            var outputs = new NodeOutputs();
            outputs.Set("Result", a + b);
            return outputs;
        }
    }
    public class SubtractBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double a = inputs.Get<double>("A");
            double b = inputs.Get<double>("B");

            var outputs = new NodeOutputs();
            outputs.Set("Result", a - b);
            return outputs;
        }
    }
    public class MultiplyBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double a = inputs.Get<double>("A");
            double b = inputs.Get<double>("B");

            var outputs = new NodeOutputs();
            outputs.Set("Result", a * b);
            return outputs;
        }
    }
    public class DivideBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double a = inputs.Get<double>("A");
            double b = inputs.Get<double>("B");

            var outputs = new NodeOutputs();
            outputs.Set("Result", a / b);
            return outputs;
        }
    }

    public class ConstantFBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            double c = inputs.Get<double>("C");

            var outputs = new NodeOutputs();
            outputs.Set("Result", c);
            return outputs;
        }
    }

    public class PrintBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            return new NodeOutputs();
        }
    }

    public class PosterizeBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            try
            {
                Bitmap img = inputs.Get<Bitmap>("Image");
                float ths = inputs.Get<float>("Threshold");

                var outputs = new NodeOutputs();
                outputs.Set("Result", ImageFilters.Posterize(img, ths));
                return outputs;
            } catch
            {
                return new NodeOutputs();
            }
        }
    }

    public class ImageInputBehavior : NodeBehavior
    {
        public override NodeOutputs Evaluate(NodeInputs inputs)
        {
            try
            {
                string path = inputs.Get<string>("Path");

                var outputs = new NodeOutputs();
                outputs.Set("Result", ImageFilters.OpenImage(path));
                return outputs;
            }
            catch
            {
                return new NodeOutputs();
            }
        }
    }
}

