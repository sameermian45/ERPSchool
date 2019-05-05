using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ERP_SchoolSystem.Classes
{
    public static class DataAccess
    {

        static DataAccess()
        {

        }

        public static SqlCommand CreateSqlCommand()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            return cmd;
        }
        public static DataSet GetDataSet(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            try
            {
                cmd.Connection.Open();
                adapter.Fill(ds);

            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return ds;
        }
        public static void BeginTransaction(SqlCommand cmd)
        {
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            cmd.Connection.Close();
        }
        public static int ProcessTransaction(SqlCommand cmd)
        {
            int rowsAffected;
            try
            {
                cmd.Connection.Open();
                rowsAffected = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rowsAffected;
        }
        public static void CommitTransaction(SqlCommand cmd)
        {
            cmd.Connection.Close();
        }
    }
}