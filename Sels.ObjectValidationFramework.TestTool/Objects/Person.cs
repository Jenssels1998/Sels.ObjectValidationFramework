using System;
using System.Collections.Generic;
using System.Text;

namespace Sels.ObjectValidationFramework.TestTool.Objects
{
    public class Person
    {
        public Person()
        {
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public Person Parent { get; set; }

        public List<Person> Children { get; set; }
    }
}
