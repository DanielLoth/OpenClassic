using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenClassic.Server.Domain.Definition
{
    public class DoorDefinition
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Command1 { get; set; }

        public string Command2 { get; set; }

        public string ModelVar1 { get; set; }

        public string ModelVar2 { get; set; }

        public string ModelVar3 { get; set; }

        public int DoorType { get; set; }

        public int Unknown { get; set; }
    }
}
