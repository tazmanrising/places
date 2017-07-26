using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ClearviewNexxaLeadsImport
{
    public class GenericToDataTable
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        private GenericToDataTable()
        { }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name=”T”>Custome Class </typeparam>
        /// <param name=”lst”>List Of The Custome Class</param>
        /// <returns> Return the class datatbl </returns>
        public static DataTable ConvertTo<T>(IList<T> lst)
        {
            //create DataTable Structure
            DataTable tbl = CreateTable<T>();
            Type entType = typeof(T);

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            //get the list item and add into the list
            foreach (T item in lst)
            {
                DataRow row = tbl.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) == null ? DBNull.Value : prop.GetValue(item);
                }
                tbl.Rows.Add(row);
            }

            return tbl;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name=”T”>Custome Class</typeparam>
        /// <returns></returns>
        public static DataTable CreateTable<T>()
        {
            //T –> ClassName
            Type entType = typeof(T);
            //set the datatable name as class name
            DataTable tbl = new DataTable(entType.Name);
            //get the property list
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            foreach (PropertyDescriptor prop in properties)
            {
                //add property as column and allow for Nullable types                
                tbl.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            return tbl;
        }

    }
}
