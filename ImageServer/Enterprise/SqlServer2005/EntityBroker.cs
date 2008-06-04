using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// Provides base implementation of <see cref="IEntityBroker{TServerEntity,TSearchCriteria,TUpdateColumns}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a base implementation for doing a number of SQL related operations against
    /// a <see cref="ServerEntity"/> derived class.  The class implements the interface using
    /// SQL Server 2005 and ADO.NET.  
    /// </para>
    /// <para>
    /// It can generate dynamic SQL queries, updates, and inserts.
    /// It also allows for querying against a <see cref="ServerEntity"/> using a 
    /// <see cref="EntitySelectCriteria"/> derived class.
    /// </para>
    /// </remarks>
    /// <typeparam name="TServerEntity">The ServerEntity derived class to work against.</typeparam>
    /// <typeparam name="TSelectCriteria">The appropriate criteria for selecting against the entity.</typeparam>
    /// <typeparam name="TUpdateColumns">The columns for doing insert or updates.</typeparam>
    public abstract class EntityBroker<TServerEntity, TSelectCriteria, TUpdateColumns> : Broker,
                                                                                         IEntityBroker
                                                                                             <TServerEntity,
                                                                                             TSelectCriteria,
                                                                                             TUpdateColumns>
        where TServerEntity : ServerEntity, new()
        where TSelectCriteria : EntitySelectCriteria
        where TUpdateColumns : EntityUpdateColumns
    {
        #region Private Members

        private readonly String _entityName;

        #endregion

        #region Constructors

        protected EntityBroker(String entityName)
        {
            _entityName = entityName;
        }

        #endregion

        #region Private Static Members

        /// <summary>
        /// Gets ORDER BY clauses based on the input search condition.
        /// </summary>
        /// <returns>A string containing the ORDER BY clause.</returns>
        private static string GetSelectOrderBy(String entity, EntitySelectCriteria criteria)
        {
            StringBuilder sb = new StringBuilder();

            // recurse on subCriteria
            foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
            {
                string variable = string.Format("{0}.{1}", entity, subCriteria.GetKey());
                SearchConditionBase sc = subCriteria as SearchConditionBase;
                if (sc != null)
                {
                    if (sc.SortPosition != -1)
                    {
                        string sqlColumnName;

                        // With the Server, all primary keys end with "Key".  The database implementation itself
                        // names these columns with the name GUID instead of Key.
                        if (variable.EndsWith("Key"))
                            sqlColumnName = variable.Replace("Key", "GUID");
                        else
                            sqlColumnName = variable;

                        if (sb.ToString().Length == 0)
                            sb.AppendFormat("ORDER BY {0} {1}", sqlColumnName, sc.SortDirection ? "ASC" : "DESC");
                        else
                            sb.AppendFormat(", {0} {1}", sqlColumnName, sc.SortDirection ? "ASC" : "DESC");
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets WHERE clauses based on the input search condition for a specific column.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="sc"></param>
        /// <param name="command">The SQL Command.</param>
        /// <returns>A string containing the WHERE clause for the column.</returns>
        private static string GetSelectWhereText(string variable, SearchConditionBase sc, SqlCommand command)
        {
            StringBuilder sb = new StringBuilder();
            String sqlColumnName;
            String sqlParmName;
            object[] values = new object[sc.Values.Length];

            // With the Server, all primary keys end with "Key".  The database implementation itself
            // names these columns with the name GUID instead of Key.
            if (variable.EndsWith("Key"))
                sqlColumnName = variable.Replace("Key", "GUID");
            else
                sqlColumnName = variable;

            // We use input parameters to the select statement.  We create a variable name for the 
            // input parameter based on the column name input.  Variable names can't have periods,
            // so we have to remove the "."
            int j = sqlColumnName.IndexOf(".");
            if (j != -1)
                sqlParmName = sqlColumnName.Remove(j, 1);
            else
                sqlParmName = sqlColumnName;

            // Now go through the actual input parameters.  Replace references to ServerEntityKey with
            // the GUID itself for these parameters, and replace ServerEnum derived references with the 
            // value of the enum in the array so the input parameters work properly.
            for (int i = 0; i < sc.Values.Length; i++)
            {
                ServerEntityKey key = sc.Values[i] as ServerEntityKey;
                if (key != null)
                    values[i] = key.Key;
                else
                {
                    ServerEnum e = sc.Values[i] as ServerEnum;
                    if (e != null)
                        values[i] = e.Enum;
                    else
                        values[i] = sc.Values[i];
                }
            }

            // Generate the actual WHERE clauses based on the type of condition.
            switch (sc.Test)
            {
                case SearchConditionTest.Equal:
                    sb.AppendFormat("{0} = @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotEqual:
                    sb.AppendFormat("{0} <> @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Like:
                    sb.AppendFormat("{0} like @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotLike:
                    sb.AppendFormat("{0} not like @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Between:
                    sb.AppendFormat("{0} between @{1}1 and @{1}2", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName + "1", values[0]);
                    command.Parameters.AddWithValue("@" + sqlParmName + "2", values[1]);
                    break;
                case SearchConditionTest.In:
                    sb.AppendFormat("{0} in (", sqlColumnName); // assume at least one param
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i == 0)
                            sb.AppendFormat("@{0}{1}", sqlParmName, i + 1);
                        else
                            sb.AppendFormat(", @{0}{1}", sqlParmName, i + 1);
                        command.Parameters.AddWithValue(string.Format("@{0}{1}", sqlParmName, i + 1), values[i]);
                    }
                    sb.Append(")");
                    break;
                case SearchConditionTest.LessThan:
                    sb.AppendFormat("{0} < @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.LessThanOrEqual:
                    sb.AppendFormat("{0} <= @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.MoreThan:
                    sb.AppendFormat("{0} > @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.MoreThanOrEqual:
                    sb.AppendFormat("{0} >= @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotNull:
                    sb.AppendFormat("{0} is not null", sqlColumnName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Null:
                    sb.AppendFormat("{0} is null", sqlColumnName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotExists:
                    EntitySelectCriteria notExistsSubCriteria = (EntitySelectCriteria) values[0];

                    string sql;
                    sql = GetSelectSql(notExistsSubCriteria.GetKey(), command, notExistsSubCriteria,
                                       String.Format("{0}.GUID = {1}.{0}GUID", variable, notExistsSubCriteria.GetKey()));
                    sb.AppendFormat("NOT EXISTS ({0})", sql);
                    break;
                case SearchConditionTest.Exists:
                    RelatedEntityCondition<EntitySelectCriteria> rec =
                        sc as RelatedEntityCondition<EntitySelectCriteria>;
                    if (rec == null) throw new PersistenceException("Casting error with RelatedEntityCondition", null);
                    EntitySelectCriteria existsSubCriteria = (EntitySelectCriteria) values[0];

                    string baseTableColumn = rec.BaseTableColumn;
                    if (baseTableColumn.EndsWith("Key"))
                        baseTableColumn = baseTableColumn.Replace("Key", "GUID");
                    string relatedTableColumn = rec.RelatedTableColumn;
                    if (relatedTableColumn.EndsWith("Key"))
                        relatedTableColumn = relatedTableColumn.Replace("Key", "GUID");

                    string existsSql;

                    existsSql = GetSelectSql(existsSubCriteria.GetKey(), command, existsSubCriteria,
                                             String.Format("{0}.{2} = {1}.{3}", variable, existsSubCriteria.GetKey(),
                                                           baseTableColumn, relatedTableColumn));
                    sb.AppendFormat("EXISTS ({0})", existsSql);
                    break;
                case SearchConditionTest.None:

                default:
                    throw new ApplicationException(); // invalid
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get an array of WHERE clauses for all of the search criteria specified.
        /// </summary>
        /// <param name="qualifier">The table name to use for each of where clauses.</param>
        /// <param name="criteria">The actual Search Criteria specified.</param>
        /// <param name="command">The SQL command.</param>
        /// <returns>An array of WHERE clauses.</returns>
        private static String[] GetWhereSearchCriteria(string qualifier, SearchCriteria criteria, SqlCommand command)
        {
            List<string> list = new List<string>();

            if (criteria is SearchConditionBase)
            {
                SearchConditionBase sc = (SearchConditionBase) criteria;
                if (sc.Test != SearchConditionTest.None)
                {
                    String text = GetSelectWhereText(qualifier, sc, command);
                    list.Add(text);
                }
            }
            else
            {
                // recurse on subCriteria
                foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
                {
                    // Note:  this is a bit ugly, but we don't do the <Table>.<Column>
                    // syntax for Subselect type criteria.  Subselects only need 
                    // the table name, and there isn't a real column associated with it.
                    // We could pass the entity down all the way, but decided to do it
                    // this way instead.
                    string subQualifier;
                    if (subCriteria is RelatedEntityCondition<EntitySelectCriteria>)
                        subQualifier = qualifier;
                    else
                        subQualifier = string.Format("{0}.{1}", qualifier, subCriteria.GetKey());
                    list.AddRange(GetWhereSearchCriteria(subQualifier, subCriteria, command));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Proves a SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="criteria">The criteria for the select</param>
        /// <param name="subWhere">If this is being used to generate the SQL for a sub-select, additional where clauses are included here for the select.  Otherwise the parameter is null.</param>
        /// <returns>The SQL string.</returns>
        private static string GetSelectSql(string entityName, SqlCommand command, EntitySelectCriteria criteria,
                                           String subWhere)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM {0}", entityName);

            // Generate an array of the WHERE clauses to be used.
            String[] where = GetWhereSearchCriteria(entityName, criteria, command);

            // Add the where clauses on.
            bool first = true;
            if (subWhere != null)
            {
                first = false;
                sb.AppendFormat(" WHERE {0}", subWhere);
            }

            foreach (String clause in where)
            {
                if (first)
                {
                    first = false;
                    sb.AppendFormat(" WHERE {0}", clause);
                }
                else
                    sb.AppendFormat(" AND {0}", clause);
            }

            string orderBy = GetSelectOrderBy(entityName, criteria);
            if (orderBy.Length > 0)
                sb.AppendFormat(" {0}", orderBy);

            return sb.ToString();
        }

        /// <summary>
        /// Proves a SELECT COUNT(*) SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="criteria">The criteria for the select count</param>
        /// <param name="subWhere">If this is being used to generate the SQL for a sub-select, additional where clauses are included here for the select.  Otherwise the parameter is null.</param>
        /// <returns>The SQL string.</returns>
        private static string GetSelectCountSql(string entityName, SqlCommand command, EntitySelectCriteria criteria,
                                                String subWhere)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(*) FROM {0}", entityName);

            // Generate an array of the WHERE clauses to be used.
            String[] where = GetWhereSearchCriteria(entityName, criteria, command);

            // Add the where clauses on.
            bool first = true;
            if (subWhere != null)
            {
                first = false;
                sb.AppendFormat(" WHERE {0}", subWhere);
            }

            foreach (String clause in where)
            {
                if (first)
                {
                    first = false;
                    sb.AppendFormat(" WHERE {0}", clause);
                }
                else
                    sb.AppendFormat(" AND {0}", clause);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Resolves the Database column name for a field name
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private static String GetDbColumnName(EntityColumnBase parm)
        {
            String sqlColumnName;

            if (parm is EntityUpdateColumn<ServerEntity>)
            {
                if (parm.FieldName.EndsWith("Key"))
                    sqlColumnName = String.Format("{0}", parm.FieldName.Replace("Key", "GUID"));

                else if (parm.FieldName.EndsWith("GUID"))
                    sqlColumnName = String.Format("{0}", parm.FieldName);

                else
                    sqlColumnName = String.Format("{0}GUID", parm.FieldName);
            }
            else if (parm is EntityUpdateColumn<ServerEntityKey>)
            {
                if (parm.FieldName.EndsWith("Key"))
                    sqlColumnName = String.Format("{0}", parm.FieldName.Replace("Key", "GUID"));

                else if (parm.FieldName.EndsWith("GUID"))
                    sqlColumnName = String.Format("{0}", parm.FieldName);

                else
                    sqlColumnName = String.Format("{0}GUID", parm.FieldName);
            }
            else if (parm is EntityUpdateColumn<ServerEnum>)
            {
                if (parm.FieldName.EndsWith("Enum"))
                    sqlColumnName = String.Format("{0}", parm.FieldName);

                else
                    sqlColumnName = String.Format("{0}Enum", parm.FieldName);
            }
            else
            {
                sqlColumnName = String.Format("{0}", parm.FieldName);
            }

            return sqlColumnName;
        }

        private static string GetUpdateWhereClause(SqlCommand command, ServerEntityKey key)
        {
            command.Parameters.AddWithValue("@PrimaryKey", key.Key);
            return String.Format("[GUID]=@PrimaryKey");
        }

        private static String GetUpdateSetClause(SqlCommand command, EntityUpdateColumns parameters)
        {
            StringBuilder setClause = new StringBuilder();
            bool first = true;
            foreach (EntityColumnBase parm in parameters.SubParameters.Values)
            {
                String text;
                String sqlParmName = GetDbColumnName(parm);
                if (parm is EntityUpdateColumn<XmlDocument>)
                {
                    EntityUpdateColumn<XmlDocument> p = parm as EntityUpdateColumn<XmlDocument>;

                    XmlDocument xml = p.Value;
                    StringWriter sw = new StringWriter();
                    XmlWriterSettings xmlSettings = new XmlWriterSettings();
                    xmlSettings.Encoding = Encoding.UTF8;
                    xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
                    xmlSettings.Indent = false;
                    xmlSettings.NewLineOnAttributes = false;
                    xmlSettings.CheckCharacters = true;
                    xmlSettings.IndentChars = "";

                    XmlWriter xmlWriter = XmlWriter.Create(sw, xmlSettings);
                    xml.WriteTo(xmlWriter);
                    xmlWriter.Close();

                    command.Parameters.AddWithValue("@" + sqlParmName, sw.ToString());
                }
                else if (parm is EntityUpdateColumn<ServerEnum>)
                {
                    EntityUpdateColumn<ServerEnum> p = parm as EntityUpdateColumn<ServerEnum>;
                    ServerEnum v = p.Value;
                    command.Parameters.AddWithValue("@" + sqlParmName, v.Enum);
                }
                else if (parm is EntityUpdateColumn<ServerEntity>)
                {
                    EntityUpdateColumn<ServerEntity> p = parm as EntityUpdateColumn<ServerEntity>;
                    ServerEntity v = p.Value;
                    command.Parameters.AddWithValue("@" + sqlParmName, v.GetKey().Key);
                }
                else if (parm is EntityUpdateColumn<ServerEntityKey>)
                {
                    EntityUpdateColumn<ServerEntityKey> p = parm as EntityUpdateColumn<ServerEntityKey>;
                    ServerEntityKey key = p.Value;
                    command.Parameters.AddWithValue("@" + sqlParmName, key.Key);
                }
                else
                {
                    if (parm.Value is ServerEnum)
                    {
                        ServerEnum v = (ServerEnum)parm.Value;
                        command.Parameters.AddWithValue("@" + sqlParmName, v.Enum);
                    }
                    else
                        command.Parameters.AddWithValue("@" + sqlParmName, parm.Value);
                }

                text = String.Format("[{0}]=@{0}", sqlParmName);

                if (first)
                {
                    first = false;
                    setClause.AppendFormat(text);
                }
                else
                    setClause.AppendFormat(", {0}", text);
            }

            return setClause.ToString();
        }

        /// <summary>
        /// Proves a SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="key">The GUID of the table row to update</param>
        /// <param name="parameters">The columns to update.</param>
        /// <returns>The SQL string.</returns>
        private static string GetUpdateSql(string entityName, SqlCommand command, ServerEntityKey key,
                                           EntityUpdateColumns parameters)
        {
            // SET clause
            String setClause = GetUpdateSetClause(command, parameters);

            // WHERE clause
            String whereClause = GetUpdateWhereClause(command, key);

            return String.Format("UPDATE [{0}] SET {1} WHERE {2}", entityName, setClause, whereClause);
        }
		/// <summary>
		/// Proves a SQL statement based on the supplied input criteria.
		/// </summary>
		/// <param name="entityName">The entity that is being selected from.</param>
		/// <param name="command">The SqlCommand to use.</param>
		/// <param name="criteria">The criteria for the update</param>
		/// <param name="parameters">The columns to update.</param>
		/// <returns>The SQL string.</returns>
		private static string GetUpdateSql(string entityName, SqlCommand command, TSelectCriteria criteria,
										   EntityUpdateColumns parameters)
		{
			StringBuilder sb = new StringBuilder();
			// SET clause
			String setClause = GetUpdateSetClause(command, parameters);

			// WHERE clause
			// Generate an array of the WHERE clauses to be used.
			String[] where = GetWhereSearchCriteria(entityName, criteria, command);

			// Add the where clauses on.
			bool first = true;
			foreach (String clause in where)
			{
				if (first)
				{
					first = false;
					sb.AppendFormat(" WHERE {0}", clause);
				}
				else
					sb.AppendFormat(" AND {0}", clause);
			}

			return String.Format("UPDATE [{0}] SET {1} {2}", entityName, setClause, sb);
		}

        /// <summary>
        /// Generates a SQL statement based on the input.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="parameters">The columns to insert.</param>
        /// <param name="command">The SQL Command for which the Insert SQL is being created.</param>
        /// <returns>The SQL string.</returns>
        private static string GetInsertSql(SqlCommand command, string entityName, EntityUpdateColumns parameters)
        {
            Guid guid = Guid.NewGuid();

            // Build the text after the INSERT INTO clause
            StringBuilder intoText = new StringBuilder();
            intoText.Append("(");
            intoText.Append("[GUID]");

            // Build the text after the VALUES clause
            StringBuilder valuesText = new StringBuilder();
            valuesText.Append("(");
            valuesText.AppendFormat("@PrimaryKey");
            command.Parameters.AddWithValue("@PrimaryKey", guid);

            foreach (EntityColumnBase parm in parameters.SubParameters.Values)
            {
                String sqlParmName = GetDbColumnName(parm);
                intoText.AppendFormat(", {0}", sqlParmName);

                if (parm.Value is ServerEnum)
                {
                    ServerEnum v = (ServerEnum) parm.Value;
                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, v.Enum);
                }
                else if (parm is EntityUpdateColumn<ServerEntity>)
                {
                    ServerEntity v = (ServerEntity) parm.Value;
                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, v.GetKey().Key);
                }
                else if (parm is EntityUpdateColumn<ServerEntityKey>)
                {
                    ServerEntityKey key = (ServerEntityKey) parm.Value;
                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, key.Key);
                }
                else if (parm is EntityUpdateColumn<XmlDocument>)
                {
                    XmlDocument xml = (XmlDocument) parm.Value;
                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = false;

                    using (XmlWriter writer = XmlWriter.Create(sb, settings))
                    {
                        xml.WriteTo(writer);
                        writer.Flush();
                    }

                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, sb.ToString());
                }
                else
                {
                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, parm.Value);
                }
            }
            intoText.Append(")");
            valuesText.Append(")");

            // Generate the INSERT statement
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO [{0}] {1} VALUES {2}\n", entityName, intoText, valuesText);

            // Add the SELECT statement. This allows us to popuplate the entity with the inserted values 
            // and return to the caller
            sql.AppendFormat("SELECT * FROM [{0}] WHERE [GUID]=@PrimaryKey", entityName);

            return sql.ToString();
        }

        #endregion

        #region IEntityBroker<TServerEntity,TSelectCriteria,TUpdateColumns> Members

        /// <summary>
        /// Load an entity based on the primary key.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        public TServerEntity Load(ServerEntityKey entityRef)
        {
            TServerEntity row = null; // new TServerEntity();

            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(String.Format("SELECT * FROM {0} WHERE GUID = @GUID",
                                                       _entityName), Context.Connection);
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.Parameters.AddWithValue("@GUID", entityRef.Key);

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", _entityName);
                    command.Dispose();
                    return null;
                }
                else
                {
                    if (myReader.HasRows)
                    {
                        myReader.Read();

                        row = new TServerEntity();
                        PopulateEntity(myReader, row, typeof (TServerEntity));

                        return row;
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when loading entity: {0}",
                             _entityName);

                throw new PersistenceException(
                    String.Format("Unexpected problem when loading entity: {0}: {1}", _entityName,
                                  e.Message), e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.
                if (myReader != null)
                {
                    myReader.Close();
                    myReader.Dispose();
                }
                if (command != null)
                    command.Dispose();
            }

            return row;
        }

        public IList<TServerEntity> Find(TSelectCriteria criteria)
        {
            IList<TServerEntity> list = new List<TServerEntity>();

            Find(criteria, delegate(TServerEntity row) { list.Add(row); });

            return list;
        }

        public void Find(TSelectCriteria criteria, SelectCallback<TServerEntity> callback)
        {
            SqlDataReader myReader = null;
            SqlCommand command = null;
            string sql = "";

            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = sql = GetSelectSql(_entityName, command, criteria, null);

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", _entityName);
                    Platform.Log(LogLevel.Error, "Select statement: {0}", sql);

                    command.Dispose();
                    return;
                }
                else
                {
                    if (myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            TServerEntity row = new TServerEntity();

                            PopulateEntity(myReader, row, typeof (TServerEntity));

                            callback(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with select: {0}", sql);

                throw new PersistenceException(
                    String.Format("Unexpected problem with select statment on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.
                if (myReader != null)
                {
                    myReader.Close();
                    myReader.Dispose();
                }
                if (command != null)
                    command.Dispose();
            }
        }

        public int Count(TSelectCriteria criteria)
        {
            SqlCommand command = null;
            string sql = "";

            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;

                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = sql = GetSelectCountSql(_entityName, command, criteria, null);

                object result = command.ExecuteScalar();

                int count = (int) result;
                return count;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with select: {0}", sql);

                throw new PersistenceException(
                    String.Format("Unexpected problem with select statment on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the command, or else we won't be able to do anything with the
                // connection the next time here.
                if (command != null)
                    command.Dispose();
            }
        }


        public bool Delete(ServerEntityKey key)
        {
            Platform.CheckForNullReference(key, "key");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = String.Format("delete from {0} where GUID = '{1}'", _entityName, key.Key);

                int rows = command.ExecuteNonQuery();

                return rows > 0;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with update statment on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();
            }
        }

		public bool Update(ServerEntityKey key, TUpdateColumns parameters)
		{
			Platform.CheckForNullReference(key, "key");
			Platform.CheckForNullReference(parameters, "parameters");

			SqlCommand command = null;
			try
			{
				command = new SqlCommand();
				command.Connection = Context.Connection;
				command.CommandType = CommandType.Text;
				UpdateContext update = Context as UpdateContext;

				if (update != null)
					command.Transaction = update.Transaction;

				command.CommandText = GetUpdateSql(_entityName, command, key, parameters);

				int rows = command.ExecuteNonQuery();

				return rows > 0;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
							 command != null ? command.CommandText : "");

				throw new PersistenceException(
					String.Format("Unexpected problem with update statment on table {0}: {1}", _entityName, e.Message),
					e);
			}
			finally
			{
				// Cleanup the reader/command, or else we won't be able to do anything with the
				// connection the next time here.

				if (command != null)
					command.Dispose();
			}
		}

		public bool Update(TSelectCriteria criteria, TUpdateColumns parameters)
        {
			Platform.CheckForNullReference(criteria, "criteria");
            Platform.CheckForNullReference(parameters, "parameters");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = GetUpdateSql(_entityName, command, criteria, parameters);

                int rows = command.ExecuteNonQuery();

                return rows > 0;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with update statment on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();
            }
        }

        public TServerEntity Insert(TUpdateColumns parameters)
        {
            Platform.CheckForNullReference(parameters, "parameters");
            Platform.CheckFalse(parameters.IsEmpty, "parameters must not be empty");


            SqlCommand command = null;
            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = GetInsertSql(command, _entityName, parameters);

                TServerEntity entity = null;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            entity = new TServerEntity();
                            PopulateEntity(reader, entity, typeof (TServerEntity));
                            break;
                        }
                    }
                }

                return entity;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with update statment on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup
                if (command != null)
                    command.Dispose();
            }
        }

        #endregion
    }
}
