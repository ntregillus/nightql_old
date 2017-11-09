using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace NightQL.Data
{
    public class SqlExcecuter
    {
        private DbConnection _con;
        private List<SqlParameter> _parameters;

        public SqlExcecuter(DbConnection con)
        {
            _con = con;
            CommandType = CommandType.Text;
            _parameters = new List<SqlParameter>();
            if(_con.State != ConnectionState.Open)
            {
                _con.Open();
            }
        }
        public CommandType CommandType { get; set; }

        public IEnumerable<T> HydrateModelsByProcedure<T>(string procName, DbMapper<T> mapper) where T : class, new()
        {
            IEnumerable<T> result = null;
            using (var cmd = _con.CreateCommand())
            {
                cmd.CommandText = procName;
                cmd.CommandType = CommandType;
                foreach (var p in _parameters)
                {
                    cmd.Parameters.Add(p);
                }
                using (var reader = cmd.ExecuteReader())
                {

                    result = mapper.Map(reader);
                }
                ClearParameters();
                foreach (var parameter in cmd.Parameters)
                {
                    _parameters.Add((SqlParameter)parameter);
                }
            }
            return result;
        }

        public void ExecuteAsReader(string sql, Action<IDataReader> func){
            using(var cmd = _con.CreateCommand())
            {
                cmd.CommandText = sql;
                CommandType = CommandType;
                 foreach (var p in _parameters)
                {
                    cmd.Parameters.Add(p);
                }
                using (var reader = cmd.ExecuteReader())
                {
                    func(reader);
                }
            }
        }

        public void ExecuteNonQuery(string commandText)
        {

            using (var cmd = _con.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType;
                foreach (var p in _parameters)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();
                _parameters.Clear();
                foreach(var parameter in cmd.Parameters)
                {
                    _parameters.Add((SqlParameter)parameter);
                }
                //todo: verify the _parameters records are upodated with output parameters
            }

        }
        public IDbDataParameter AddParameter(string paramName, object value, ParameterDirection direction, DbType type, int? size = null)
        {
            var parameter = AddParameter(paramName, value);
            parameter.Direction = direction;
            parameter.DbType = type;
            if (size.HasValue)
                parameter.Size = size.Value;
            return parameter;
        }

        public IDbDataParameter AddParameter(string paramName, object value)
        {
            if (value is string && string.IsNullOrWhiteSpace(value.ToString()))
            {
                value = null;
            }

            var p = new SqlParameter(paramName, value);
            if (value == null)
                p.Value = DBNull.Value;
            _parameters.Add(p);
            return p;
        }

        public IDbDataParameter AddParameterIDList(string paramName, IEnumerable<int> values)
        {
            DataTable t = new DataTable();
            t.Columns.Add("i", typeof(int));
            foreach (var i in values)
            {
                t.Rows.Add(i);
            }
            var p = AddParameter(paramName, t) as SqlParameter;
            p.SqlDbType = SqlDbType.Structured;
            return p;
        }

        public IDbDataParameter AddParameterStringList(string paramName, IEnumerable<string> values, string colName = "name")
        {
            DataTable t = new DataTable();
            t.Columns.Add(colName, typeof(string));
            foreach (var i in values)
            {
                t.Rows.Add(i);
            }
            var p = AddParameter(paramName, t) as SqlParameter;
            p.SqlDbType = SqlDbType.Structured;
            return p;
        }

        public void ClearParameters()
        {
            _parameters.Clear();
        }
        protected object GetParameterValueObject(string paramName)
        {
            var param = _parameters.Where(p => p.ParameterName == paramName).FirstOrDefault();
            var result = param == null ? null : param.Value;
            return result == DBNull.Value ? null : result;
        }

        public T GetParameterValue<T>(string paramName)
        {
            object objValue = GetParameterValueObject(paramName);
            if (objValue == null)
                return default(T);
            if (objValue is System.ValueType)
                return (T)objValue;
            return (T)Convert.ChangeType(objValue, typeof(T));
        }
    }
}