using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace softerCell_U3_v0._01
{
    class messageFlowSearch
    {
        public static HashSet<string> iu_cs_sinalling_relation_delegate_imsi(List<streamMessagePool._stream_message> ml, string imsi, bool sFlag)
        {

            //哈希表收集关联因子
            HashSet<string> hs = new HashSet<string>();
            hs.Add(imsi);

            //lr关联收集sccp连接串slr
            IEnumerable<string> l_slr_opc =
                    from n in ml
                    where n.message_gsm_a_imsi == imsi & n.message_sccp_slr != null
                    select n.message_sccp_slr + n.message_source
                        into m
                        where m.Contains("0x")
                        select m;
            hs.UnionWith(l_slr_opc);
            if (l_slr_opc.Any())
                foreach (string slr_opc in l_slr_opc)
                    hs.UnionWith(iu_cs_sinalling_relation_delegate_lr(ml, slr_opc, sFlag));

            //lr关联收集sccp连接串dlr
            IEnumerable<string> l_dlr_dpc =
                    from n in ml
                    where n.message_gsm_a_imsi == imsi & n.message_sccp_dlr != null
                    select n.message_sccp_dlr + n.message_destination
                        into m
                        where m.Contains("0x")
                        select m;
            hs.UnionWith(l_dlr_dpc);
            if (l_dlr_dpc.Any())
                foreach (string dlr_dpc in l_dlr_dpc)
                    hs.UnionWith(iu_cs_sinalling_relation_delegate_lr(ml, dlr_dpc, sFlag));

            return hs;
        }

        public static HashSet<string> iu_cs_sinalling_relation_delegate_lr(List<streamMessagePool._stream_message> ml, string lr_pc, bool sFlag)
        {

            //哈希表收集关联因子
            HashSet<string> hs = new HashSet<string>();
            hs.Add(lr_pc);

            //lr关联收集sccp连接串slr
            IEnumerable<string> l_slr_opc =
                 from n in ml
                 where n.message_sccp_dlr + n.message_destination == lr_pc
                 select n.message_sccp_slr + n.message_source
                     into m
                     where m.Contains("0x")
                     select m;
            hs.UnionWith(l_slr_opc);

            //lr关联收集sccp连接串dlr
            IEnumerable<string> l_dlr_dpc =
                from n in ml
                where n.message_sccp_slr + n.message_source == lr_pc
                select n.message_sccp_dlr + n.message_destination
                    into m
                    where m.Contains("0x")
                    select m;
            hs.UnionWith(l_dlr_dpc);

            //sccp连接串收集imsi、主被叫号码
            if (sFlag)
            {

                //收集IMSI
                IEnumerable<string> l_imsi =
                    from n in ml
                    where n.message_gsm_a_imsi != null
                    & (hs.Contains(n.message_sccp_slr + n.message_source) | hs.Contains(n.message_sccp_dlr + n.message_destination))
                    select n.message_gsm_a_imsi
                        into m
                        where m.Length > 10
                        select m;
                hs.UnionWith(l_imsi);
                if (l_imsi.Any())
                    foreach (string imsi in l_imsi)
                        hs.UnionWith(p_iu_cs_sinalling_relation_delegate_imsi(ml, imsi));

                //收集主被叫号码
                IEnumerable<string> l_cpn =
                    from n in ml
                    where n.message_id_num != null
                    & (hs.Contains(n.message_sccp_slr + n.message_source) | hs.Contains(n.message_sccp_dlr + n.message_destination))
                    select n.message_id_num
                        into m
                        where m.Length > 10 & m.IndexOf("8613800") == -1
                        select m;
                hs.UnionWith(l_cpn);

                //根据号码再收集CIC----------------------------------------------------
                if (l_cpn.Any())
                    foreach (string cpn in l_cpn)
                    {
                        IEnumerable<streamMessagePool._stream_message> l_cic =
                            from n in ml
                            where n.CallingPartyNumber != null & n.CalledPartyNumber != null
                            select n
                                into m
                                where m.CallingPartyNumber.Replace("F", "").Replace("17244", "").Trim() == cpn
                                | m.CalledPartyNumber.Replace("F", "").Replace("17244", "").Trim() == cpn
                                select m;
                        if (l_cic.Any())
                        {
                            foreach (streamMessagePool._stream_message cic in l_cic)
                            {
                                hs.Add(cic.CIC + cic.message_source + cic.message_destination);
                                hs.Add(cic.CIC + cic.message_destination + cic.message_source);
                            }
                        }
                    }
            }
            return hs;
        }

        private static HashSet<string> p_iu_cs_sinalling_relation_delegate_imsi(List<streamMessagePool._stream_message> ml, string imsi)
        {

            //哈希表收集关联因子
            HashSet<string> hs = new HashSet<string>();
            hs.Add(imsi);

            //lr关联收集sccp连接串slr
            IEnumerable<string> l_slr_opc =
                    from n in ml
                    where n.message_gsm_a_imsi == imsi & n.message_sccp_slr != null
                    select n.message_sccp_slr + n.message_source
                        into m
                        where m.Contains("0x")
                        select m;
            hs.UnionWith(l_slr_opc);
            if (l_slr_opc.Any())
                foreach (string slr_opc in l_slr_opc)
                    hs.UnionWith(p_iu_cs_sinalling_relation_delegate_lr(ml, slr_opc));

            //lr关联收集sccp连接串dlr
            IEnumerable<string> l_dlr_dpc =
                    from n in ml
                    where n.message_gsm_a_imsi == imsi & n.message_sccp_dlr != null
                    select n.message_sccp_dlr + n.message_destination
                        into m
                        where m.Contains("0x")
                        select m;
            hs.UnionWith(l_dlr_dpc);
            if (l_dlr_dpc.Any())
                foreach (string dlr_dpc in l_dlr_dpc)
                    hs.UnionWith(p_iu_cs_sinalling_relation_delegate_lr(ml, dlr_dpc));

            return hs;
        }

        private static HashSet<string> p_iu_cs_sinalling_relation_delegate_lr(List<streamMessagePool._stream_message> ml, string lr_pc)
        {

            //哈希表收集关联因子
            HashSet<string> hs = new HashSet<string>();
            hs.Add(lr_pc);

            //lr关联收集sccp连接串slr
            IEnumerable<string> l_slr_opc =
                 from n in ml
                 where n.message_sccp_dlr + n.message_destination == lr_pc
                 select n.message_sccp_slr + n.message_source
                     into m
                     where m.Contains("0x")
                     select m;
            hs.UnionWith(l_slr_opc);

            //lr关联收集sccp连接串dlr
            IEnumerable<string> l_dlr_dpc =
                from n in ml
                where n.message_sccp_slr + n.message_source == lr_pc
                select n.message_sccp_dlr + n.message_destination
                    into m
                    where m.Contains("0x")
                    select m;
            hs.UnionWith(l_dlr_dpc);

            return hs;
        }

        public static HashSet<string> iu_cs_sinalling_relation_delegate_cic(List<streamMessagePool._stream_message> ml, string sCIC, bool sFlag)
        {

            //哈希表收集关联因子
            HashSet<string> hs = new HashSet<string>();

            //CIC关联收集----------------------------------主叫号码-+-被叫号码
            IEnumerable<string> l_cpn =
                from n in ml
                where n.CIC + n.message_source + n.message_destination == sCIC
                | n.CIC + n.message_destination + n.message_source == sCIC
                select n.CallingPartyNumber + "," + n.CalledPartyNumber
                        into m
                        where m.Length > 10
                        select m.Replace("F", "").Replace("17244", "").Trim();
                if(l_cpn.Any ())
                    foreach (string s in l_cpn )
                    {
                        string[] ar=s.Split(Convert.ToChar (","));
                        hs.UnionWith(ar.ToList());        
                    }

            //CPN关联setup消息，提取setup消息中的lr关联其他消息的关联因子
            IEnumerable<string> l_lr =
                from n in ml
                where (n.message_sccp_slr != null | n.message_sccp_dlr != null)
                & hs.Contains(n.message_id_num)
                select n.message_frame;
                if (l_lr.Any())
                    foreach (string fr in l_lr)
                    {
                        int i = Int32.Parse(fr);
                        hs.UnionWith(iu_cs_sinalling_relation_delegate_fr(i, sFlag));
                    }

            return hs;
        }

        public static HashSet<string> iu_cs_sinalling_relation_delegate_fr(int fr, bool sFlag)
        {
            Dictionary<int, string> ds = new Dictionary<int, string>();
            streamMessagePool._stream_message sm = streamMessagePool.ml.ElementAt(fr);
            ds.Add(0, sm.message_gsm_a_imsi);
            ds.Add(1, sm.message_sccp_dlr + sm.message_destination);
            ds.Add(2, sm.message_sccp_slr + sm.message_source);
            ds.Add(3, sm.CIC + sm.message_source + sm.message_destination);
            HashSet<string> hs = new HashSet<string>();
            foreach (KeyValuePair<int, string> kv in ds)
            {
                if (kv.Value != null)
                    if (kv.Value.Length > 10)
                    {
                        switch (kv.Key)
                        {
                            case 0:
                                hs = iu_cs_sinalling_relation_delegate_imsi(streamMessagePool.ml, kv.Value, sFlag);
                                break;
                            case 1:
                                hs = iu_cs_sinalling_relation_delegate_lr(streamMessagePool.ml, kv.Value, sFlag);
                                break;
                            case 2:
                                hs = iu_cs_sinalling_relation_delegate_lr(streamMessagePool.ml, kv.Value, sFlag);
                                break;
                            case 3:
                                hs.Add(sm.CIC + sm.message_source + sm.message_destination);
                                hs.Add(sm.CIC + sm.message_destination + sm.message_source);
                                hs.UnionWith(iu_cs_sinalling_relation_delegate_cic(streamMessagePool.ml, kv.Value, sFlag));
                                break;
                            default:
                                break;
                        }
                        break;
                    }
            }
            return hs;
        }

        public static IEnumerable<streamMessagePool._stream_message> convert_hsRelationString_listUserMessage(HashSet<string> hs, int fr, HashSet<string> hsEndMessage)
        {

            //收集关联集合消息------------------------------------开始至所有的消息
            IEnumerable<streamMessagePool._stream_message> list_fr =
            from n in streamMessagePool.ml
            where Int32.Parse(n.message_frame) - fr >= 0
            select n
                into m
                where hs.Contains(m.message_sccp_slr + m.message_source)
                    | hs.Contains(m.message_sccp_dlr + m.message_destination)
                    | hs.Contains(m.message_gsm_a_imsi)
                    | hs.Contains(m.CIC + m.message_source + m.message_destination)
                    //| hs.Contains(m.CallingPartyNumber)
                    //| hs.Contains(m.message_id_num)
                    //| hs.Contains(m.CalledPartyNumber)
                select m;

            //本次计算的消息流的结束消息--------------------------截取结束消息
            IEnumerable<string> list_max_fr =
            from n in list_fr
            where hsEndMessage.Contains(n.message_info)
            select n.message_frame;
            if (list_max_fr.Any())
            {
                int int_max_fr = Int32.Parse(list_max_fr.FirstOrDefault());
                list_fr =
                from n in list_fr
                where int_max_fr - Int32.Parse(n.message_frame) >= 0
                select n;
            }

            return list_fr;
        }
    }
}


