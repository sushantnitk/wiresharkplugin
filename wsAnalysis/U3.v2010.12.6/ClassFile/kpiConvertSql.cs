using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace softerCell_U3_v0._01
{
    class kpiConvertSql
    {
        public static void mobile_kpi_stat_result()
        {
            List<string> ls = new List<string>();
            ls.Add("_moc"); ls.Add("_mtc"); ls.Add("_lau"); ls.Add("_ho_in"); ; ls.Add("_ho_out"); ls.Add("_isup_iam");
          
            foreach (string lSR in ls)
            {
                List<streamMessagePool._kpi_stat> lKS = new List<streamMessagePool._kpi_stat>();

                DataTable dt = streamMessagePool.Acc.RunQuery("select * from softercell" + lSR);
                int id = 0;
                double startTime;
                double endTime;
                string st;
                string et;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].ColumnName.IndexOf("message_frame") != -1)
                    {
                        streamMessagePool._kpi_stat ks = new streamMessagePool._kpi_stat();
                        id++;
                        ks.message_info = dt.Columns[i].ColumnName.ToString();
                        ks.id = id;
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (dt.Columns[i + 1].ColumnName.ToString().IndexOf("time") != -1)
                            {
                                st = dt.Rows[j][2].ToString();
                                et = dt.Rows[j][i + 1].ToString();
                                if (st.IndexOf(":") == -1 && et.Length != 0 && et.IndexOf(":") == -1)
                                {
                                    startTime = double.Parse(st);
                                    endTime = double.Parse(et);
                                    if ((endTime - startTime) >= 0)
                                    {                
                                        ks.message_sum++;
                                        ks.message_time += (endTime - startTime);
                                        //切换完成的标识，纯统计方法的校准
                                        if (lSR == "_ho_out" && ks.message_info == "id_Iu_Release_r_message_frame" 
                                            && dt.Rows[j][i+2].ToString().IndexOf ("successful-relocation (11)") == -1)
                                            ks.message_sum--;
                                    }
                                }
                            }
                        }
                        ks.message_time = Math.Round(ks.message_time / ks.message_sum, 2);
                        lKS.Add(ks);
                    }
                }
                int m = lKS.Count;
                for (int i = 0; i < m; i++)
                {
                    if (i != 0)
                    {
                        lKS.ElementAt(i).loss_sum = lKS.ElementAt(0).message_sum - lKS.ElementAt(i).message_sum;

                        lKS.ElementAt(i).loss_rate = Math.Round(lKS.ElementAt(i).loss_sum / lKS.ElementAt(0).message_sum, 2);

                        lKS.ElementAt(i).step_delay = Math.Round(lKS.ElementAt(i).message_time - lKS.ElementAt(i - 1).message_time, 2);

                        lKS.ElementAt(i).delay_rate = Math.Round(lKS.ElementAt(i).step_delay / lKS.ElementAt(m - 1).message_time, 2);
                    }
                    else
                    {
                        lKS.ElementAt(i).loss_sum = 0;
                        lKS.ElementAt(i).step_delay = 0;
                        lKS.ElementAt(i).loss_rate = 0;
                        lKS.ElementAt(i).step_delay = 0;
                    }
                }

                listConvertSql.mobile_kpi_stat_flow(lKS, lSR);
            }
        }
        

    }
}
