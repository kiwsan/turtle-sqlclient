using System;

namespace TurtleSqlClient.Attributes
{
    public class ColumnNameAttribute : Attribute
    {
        public ColumnNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}