using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackendTest.Utilities
{
    public class CacheHelper<T>
    {
        public void ClearTheBitch(T item)
        {
            string name = "IdToDatabaseId";
            Type t = typeof(T);
            FieldInfo field = t.GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
            var value = field.GetValue(item);
            Dictionary<string, int> cache = (Dictionary<string, int>)value;
            cache.Clear();
        }
    }
}
