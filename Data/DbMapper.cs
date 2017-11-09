using System;
using System.Collections.Generic;
using System.Data;

namespace NightQL.Data
{
    public class DbMapper<T> where T:class, new()
    {
        public DbMapper()
        {
            PropertyMappings = new Dictionary<string, Action<T, object>>();
        }
        protected Dictionary<string, Action<T, object>> PropertyMappings { get; set; }

        public void Add(string fieldName, Action<T, object> assignment)
        {
            if (PropertyMappings.ContainsKey(fieldName))
                PropertyMappings[fieldName] = assignment;
            else
                PropertyMappings.Add(fieldName, assignment);
        }

        public DbMapper<T> DuplicateMapper()
        {
            var result = new DbMapper<T>();
            foreach(var propMapping in this.PropertyMappings)
            {
                result.Add(propMapping.Key, propMapping.Value);
            }
            return result;
        }

        public List<T> Map(IDataReader reader)
        {
            List<T> result = new List<T>();
            while(reader.Read())
            {
                T item = new T();
                foreach (var nvp in PropertyMappings)
                {
                    object value = reader[nvp.Key] == DBNull.Value ? null : reader[nvp.Key];
                    nvp.Value.Invoke(item, value);
                }
                result.Add(item);
            }
            return result;
        }



    }


    public static class MapHelp
    {
        public static string StringMap(object value)
        {
            return (value == null) ? null : value.ToString();
        }
        public static int IntMap(object value)
        {
            return value == null ? 0 : (int)value;
        }

        public static T EnumMap<T>(object value) where T : struct, IConvertible
        {
            if (value == null){
                return default(T);
            }
            if (value is byte[])
            {
                return (T)Enum.ToObject(typeof(T), ((byte[])value)[0]);
            }
            else if (value is byte)
            {
                return (T)Enum.ToObject(typeof(T), ((byte)value));
            }
            else if (value is short || value is int || value is long)
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            else
            {
                throw new NotImplementedException($"Only handling single byte arrays. {value.GetType().FullName}  will have to handle this type later");
            }
        }

        public static DateTime? MapDate(object value)
        {
            if (value == null){
                return null;
            }
            return (DateTime)value;
        }

        public static T? NullableMap<T>(object value) where T : struct
        {
            return value == null ? null : new T?((T)Convert.ChangeType(value, typeof(T)));
        }
    }

}