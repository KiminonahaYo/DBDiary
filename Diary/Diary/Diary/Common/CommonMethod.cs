using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DBClassLib.All.Base;

namespace Diary.Common
{
    public static class CommonMethod
    {
        /// <summary>
        ///     DBコネクションを生成する。
        /// </summary>
        /// <returns>コネクション</returns>
        public static DbConnection CreateConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            CommonLibrary.Win32.IniFile ini = new CommonLibrary.Win32.IniFile("./Diary.ini");

            builder.DataSource = ini.Keys("Database", "DATASOURCE").ReadToString();
            builder.UserID = ini.Keys("Database", "USER_ID").ReadToString();
            builder.Password = ini.Keys("Database", "PASSWORD").ReadToString();
            builder.InitialCatalog = ini.Keys("Database", "INITIAL_CATALOG").ReadToString();

            DbConnection con = new DbConnection(DBClassLib.All.DbKind.SQLServer);
            con.ConnectionString = builder.ToString();

            return con;
        }
    }
}
