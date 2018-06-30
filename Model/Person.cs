using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Person
    {
        public long PersonId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public Department Department { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1} {2} {3}", PersonId, Name, Level, Department.Name);
        }
    }
}
