using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Data;
using System.ComponentModel;
using System.Reflection;


namespace MMS2
{
    public static class Extensions
        {
            public static string ToString(this DateTime? date)
            {
                return date.ToString(null, DateTimeFormatInfo.CurrentInfo);
            }
            public static string ToString(this DateTime? date, string format)
            {
                return date.ToString(format, DateTimeFormatInfo.CurrentInfo);
            }
            public static string ToString(this DateTime? date, IFormatProvider provider)
            {
                return date.ToString(null, provider);
            }
            public static string ToString(this DateTime? date, string format, IFormatProvider provider)
            {
                if (date.HasValue)
                    return date.Value.ToString(format, provider);
                else
                    return string.Empty;
            }
            public static string ReportDisplay(this DateTime? date)
            {
                if (date.HasValue)
                    return date.ToString("dd MMM yyyy", DateTimeFormatInfo.CurrentInfo);
                else
                    return string.Empty;
            }
            public static string DateTimeSaveFormat(this DateTime? date)
            {
                if (date.HasValue)
                    return date.ToString("yyyy-MM-dd hh:mm:ss tt", DateTimeFormatInfo.CurrentInfo);
                else
                    return null;
            }
            public static string HandleNull(this string str)
            {
                if (string.IsNullOrEmpty(str)) return "";
                else return str;
            }
            public static DataTable LINQToDataTable<T>(this DataTable dt, IEnumerable<T> varlist)
            {
                DataTable dtReturn = new DataTable();

                                 PropertyInfo[] oProps = null;
                Type colType;

                if (varlist == null) return dtReturn;

                foreach (T rec in varlist)
                {
                                         if (oProps == null)
                    {
                        oProps = ((Type)rec.GetType()).GetProperties();
                        foreach (PropertyInfo pi in oProps)
                        {
                            colType = pi.PropertyType;

                            if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                            {
                                colType = colType.GetGenericArguments()[0];
                            }

                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                        }
                    }
                    DataRow dr = dtReturn.NewRow();

                    foreach (PropertyInfo pi in oProps)
                    {
                        if (pi.GetValue(rec, null) == null)
                        {
                            if (pi.PropertyType.Name == "String") dr[pi.Name] = "";
                            else dr[pi.Name] = DBNull.Value;
                        }
                        else
                        {
                            dr[pi.Name] = pi.GetValue(rec, null);
                        }
                                             }

                    dtReturn.Rows.Add(dr);
                }
                return dtReturn;
            }
            public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
            {
                try
                {
                    List<T> list = new List<T>();

                    foreach (var row in table.AsEnumerable())
                    {
                        T obj = new T();

                        foreach (var prop in obj.GetType().GetProperties())
                        {
                            try
                            {
                                if (table.Columns.Contains(prop.Name))
                                {
                                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                                    propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new ApplicationException(ex.Message, ex.InnerException);
                                                             }
                        }

                        list.Add(obj);
                    }

                    return list;
                }
                catch
                {
                    return null;
                }
            }
            public static T DataTableToModel<T>(this DataTable table) where T : class, new()
            {
                try
                {
                    T list = new T();

                    foreach (var row in table.AsEnumerable())
                    {
                        T obj = new T();

                        foreach (var prop in obj.GetType().GetProperties())
                        {
                            try
                            {
                                if (table.Columns.Contains(prop.Name))
                                {
                                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                                                                         propertyInfo.SetValue(obj, ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                }
                            }
                            catch
                            {
                                                                 continue;
                            }
                        }

                        list = obj;
                    }

                    return list;
                }
                catch
                {
                    return null;
                }
            }
            public static string HandleInt(this string i)
            {
                if (i == null) return "0";
                else if (string.IsNullOrEmpty(i.ToString())) return "0";
                else return i;
            }
            public static decimal ObjToDecimal(this object o)
            {
                if (o == null) return 0;
                else if (o.ToString().Trim().Length == 0) return 0;
                else
                {
                    decimal ret = 0;
                    decimal.TryParse(o.ToString(), out ret);
                    return ret;
                }
            }
            public static DateTime? IsDate(this DateTime? obj)
            {
                string strDate = obj.ToString();
                try
                {
                    DateTime dt;
                    DateTime.TryParse(strDate, out dt);
                    if (dt != DateTime.MinValue && dt != DateTime.MaxValue) return dt;
                    return null;
                }
                catch
                {
                    return null;
                }
            }
            public static DateTime? ToDate(this string obj)
            {
                DateTime d = DateTime.MinValue;
                DateTime.TryParse(obj, out d);
                if (d == DateTime.MinValue) return null;
                return d;
            }
            public static string SaveFormat(this DateTime o)
            {
                if (o == null) return null;
                return o.ToString("yyyy-MM-dd hh:mm:ss tt");
            }
            public static DataTable ListToDataTable<T>(this IList<T> data)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                DataTable table = new DataTable();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (!prop.PropertyType.Name.Equals("List`1"))
                    {
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                }
                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (!prop.PropertyType.Name.Equals("List`1"))
                        {
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        }
                    }
                    table.Rows.Add(row);
                }
                return table;
            }
            public static string ListToXml<T>(this IList<T> data, string pTableName)
            {
                if (data == null) return null;
                DataTable dt = ListToDataTable(data);
                dt.TableName = pTableName;
                System.IO.StringWriter sw = new System.IO.StringWriter();
                dt.WriteXml(sw);
                return sw.ToString();
            }
            public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
            {

                foreach (T item in enumeration)
                {
                    action(item);
                }
            }
            public static DataTable ToADOTable<T>(this IEnumerable<T> varlist, CreateRowDelegate<T> fn)
            {
                DataTable toReturn = new DataTable();

                                 T TopRec = varlist.ElementAtOrDefault(0);

                if (TopRec == null)
                    return toReturn;

                                  
                PropertyInfo[] oProps = ((Type)TopRec.GetType()).GetProperties();

                foreach (PropertyInfo pi in oProps)
                {
                    Type pt = pi.PropertyType;
                    if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                        pt = Nullable.GetUnderlyingType(pt);
                    toReturn.Columns.Add(pi.Name, pt);
                }

                foreach (T rec in varlist)
                {
                    DataRow dr = toReturn.NewRow();
                    foreach (PropertyInfo pi in oProps)
                    {
                        object o = pi.GetValue(rec, null);
                        if (o == null)
                            dr[pi.Name] = DBNull.Value;
                        else
                            dr[pi.Name] = o;
                    }
                    toReturn.Rows.Add(dr);
                }

                return toReturn;
            }
            public static DataTable ToADOTable<T>(this IEnumerable<T> varlist)
            {
                DataTable toReturn = new DataTable();

                                 T TopRec = varlist.ElementAtOrDefault(0);

                if (TopRec == null)
                    return toReturn;

                                  
                PropertyInfo[] oProps = ((Type)TopRec.GetType()).GetProperties();

                foreach (PropertyInfo pi in oProps)
                {
                    Type pt = pi.PropertyType;
                    if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                        pt = Nullable.GetUnderlyingType(pt);
                    toReturn.Columns.Add(pi.Name, pt);
                }

                foreach (T rec in varlist)
                {
                    DataRow dr = toReturn.NewRow();
                    foreach (PropertyInfo pi in oProps)
                    {
                        object o = pi.GetValue(rec, null);

                        if (o == null)
                            dr[pi.Name] = DBNull.Value;
                        else
                            dr[pi.Name] = o;
                    }
                    toReturn.Rows.Add(dr);
                }

                return toReturn;
            }
            public static List<T> ToList<T>(this DataTable datatable) where T : new()
            {
                List<T> Temp = new List<T>();
                try
                {
                    List<string> columnsNames = new List<string>();
                    foreach (DataColumn DataColumn in datatable.Columns)
                        columnsNames.Add(DataColumn.ColumnName);
                    Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
                    return Temp;
                }
                catch { return Temp; }
            }
            public static T getObject<T>(DataRow row, List<string> columnsName) where T : new()
            {
                T obj = new T();
                try
                {
                    string columnname = "";
                    string value = "";
                    PropertyInfo[] Properties; Properties = typeof(T).GetProperties();
                    foreach (PropertyInfo objProperty in Properties)
                    {
                        columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                        if (!string.IsNullOrEmpty(columnname))
                        {
                            value = row[columnname].ToString();
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                                {
                                    value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                    objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                                }
                                else
                                {
                                    value = row[columnname].ToString().Replace("%", "");
                                    objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                                }
                            }
                        }
                    } return obj;
                }
                catch { return obj; }
            }
            public delegate object[] CreateRowDelegate<T>(T t);
            public static object ChangeType(object value, Type conversionType)
            {
                if (conversionType == null)
                {
                    throw new ArgumentNullException("conversionType");
                }

                if (conversionType.IsGenericType &&
                  conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (value == null)
                    {
                        return null;
                    }
                    NullableConverter nullableConverter = new NullableConverter(conversionType);
                    conversionType = nullableConverter.UnderlyingType;
                }

                return Convert.ChangeType(value, conversionType);
            }


        }
   
}
