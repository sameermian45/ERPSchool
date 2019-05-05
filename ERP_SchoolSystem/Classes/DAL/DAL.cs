using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ERP_SchoolSystem.Classes.DAL
{
    public static class DAL
    {

        static DAL()
        {

        }

        public static DataSet VerifyTaxFiler(string NameOfInsured, string NIC, string NTNNo)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = ERP_SchoolSystem.Classes.DataAccess.CreateSqlCommand();
            cmd.CommandText = "spCheckForTaxFiler";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@NameOfInsured", SqlDbType.NChar).Value = NameOfInsured;
            cmd.Parameters.Add("@NTNNo", SqlDbType.NChar).Value = NTNNo;
            cmd.Parameters.Add("@NIC", SqlDbType.NChar).Value = NIC;
            try
            {
                ds = ERP_SchoolSystem.Classes.DataAccess.GetDataSet(cmd);
                return ds;
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
    }
}