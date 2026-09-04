using System;
using System.Collections.Generic;
using System.Text;

namespace Nodevia.Models
{
    public class Connection
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Port Source { get; }
        public Port Target { get; }

        public bool IsSelected { get; set; }

        public Connection(Port source, Port target)
        {
            if (source.Direction != PortDirection.Output)
                throw new ArgumentException("Source port must be an Output port.", nameof(source));

            if (target.Direction != PortDirection.Input)
                throw new ArgumentException("Target port must be an Input port.", nameof(target));

            Source = source;
            Target = target;
        }
    }
}
