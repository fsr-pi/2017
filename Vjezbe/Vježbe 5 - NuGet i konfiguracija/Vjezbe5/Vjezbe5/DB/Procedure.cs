using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Vjezbe5.DB
{
    

    public static class Procedure
    {
        
        public static string str = Program.Configuration["Data:DefaultConnection:lokalnaBaza"];

        public static List<string> GetBankaSteteCPICPID()
        {
            using (SqlConnection con = new SqlConnection(str))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.selectTopPribavitelji", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    List<string> list = new List<string>();
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string test = "";

                            test += dr[0].ToString();
                            test += " ";
                            test += dr[1].ToString();
                            test += " ";
                            test += dr[2].ToString();
                            test += " ";
                            test += dr[3].ToString();
                            test += " ";

                            list.Add(test);
                        }
                    }

                    con.Close();
                    return list;
                }
            }
        }
    }
}
