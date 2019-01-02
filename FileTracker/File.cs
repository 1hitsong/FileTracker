using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTracker
{
    public class File
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public File(string name)
        {
            Name = name;
        }

        public File() { }
    }
}
