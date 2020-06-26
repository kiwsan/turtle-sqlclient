using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TurtleSqlClient.Attributes;
using TurtleSqlClient.Extensions;

namespace TurtleSqlClient
{
    public class SqlClientDbContext : IDisposable
    {
        public string ConnectionString { get; private set; }
        public SqlConnection DbSqlConnection { get; private set; }
        public SqlCommand DbSqlCommand { get; private set; }
        public SqlClientDbContext(string connectionString)
        {
            ConnectionString = connectionString;
            DbSqlConnection = new SqlConnection(ConnectionString);
            DbSqlCommand = DbSqlConnection.CreateCommand();
        }

        private TEntity MapEntity<TEntity>(IDataRecord items)
        {
            // create an instance on Activator
            var entity = Activator.CreateInstance<TEntity>();

            // fetch properties in entity by typeof
            foreach (var property in typeof(TEntity).GetProperties())
            {
                string name = property.Name;

                // get column name
                foreach (var attribute in property.GetCustomAttributes(true))
                {
                    var column = attribute as ColumnNameAttribute;

                    name = column == null ? property.Name : column.Name;

                    // set value
                    if (items.HasColumn(name) && !items.IsDBNull(items.GetOrdinal(name)))
                    {
                        property.SetValue(entity, items[name]);
                    }
                }
            }

            return entity;
        }

        public IEnumerable<TEntity> RunProc<TEntity>(string commandText, params SqlParameter[] parameters)
        {
            var items = new List<TEntity>();

            // command settings
            DbSqlCommand.Parameters.Clear();
            DbSqlCommand.CommandText = commandText;
            DbSqlCommand.CommandType = CommandType.StoredProcedure;

            // add params
            if (parameters?.Length > 0)
            {
                DbSqlCommand.Parameters.AddRange(parameters);
            }

            // open a connection
            if (DbSqlConnection.State == ConnectionState.Closed)
            {
                DbSqlConnection.Open();
            }

            // fetch record
            using (var data = DbSqlCommand.ExecuteReader())
            {
                while (data.Read())
                {
                    items.Add(MapEntity<TEntity>(data));
                }
            }

            return items;
        }

        public void Dispose()
        {
            if (DbSqlConnection.State == ConnectionState.Open)
            {
                DbSqlConnection.Close();
            }

            DbSqlCommand.Dispose();
            DbSqlConnection.Dispose();
        }

    }
}