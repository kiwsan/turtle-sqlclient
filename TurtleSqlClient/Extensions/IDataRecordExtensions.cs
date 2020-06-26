using System;
using System.Data;

namespace TurtleSqlClient.Extensions
{
    public static class IDataRecordExtensions
    {
        public static bool HasColumn(this IDataRecord items, string name)
        {
            for (int i = 0; i < items.FieldCount; i++)
            {
                if (items.GetName(i).Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}