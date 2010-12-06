using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RelatedObjects.Storage;
using System.IO;
namespace Van.Utility.QQMsg
{
    public enum QQMsgType
    {
        BIM, C2C, Group, Sys, Mobile, TempSession //Disc
    }

    class QQMsgMgr
    {
        private static readonly int s_MsgTypeNum = (int)QQMsgType.TempSession + 1;
        private static readonly string[] s_MsgName = new string[] {
            "BIMMsg", "C2CMsg", "GroupMsg", "SysMsg", "MobileMsg", "TempSessionMsg"
        };
        private IStorageWrapper m_Storage;
        private byte[] m_Password;

        private List<string>[] m_MsgList = new List<string>[s_MsgTypeNum];

        public void Open(string QQID)
        {
            Open(QQID, null);
        }
        public void Open(string QQID, string QQPath)
        {
            if (QQPath == null)
            {
                using (Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Tencent\QQ"))
                {
                    QQPath = reg.GetValue("Install") as string;
                }
                if (QQPath == null) return;
            }

            for (int i = 0; i < m_MsgList.Length; ++i)
            {
                m_MsgList[i] = new List<string>();
            }
            m_Storage = null;
            m_Password = null;

            m_Storage = new IStorageWrapper(QQPath + QQID + @"\MsgEx.db");
            m_Password = QQMsgMgr.GetGlobalPass(m_Storage, QQID);

            if (m_Password == null) m_Storage = null;

            foreach (IBaseStorageWrapper.FileObjects.FileObject fileObject in m_Storage.foCollection)
            {
                if (fileObject.FileType == 1)
                {
                    for (int i = 0; i < m_MsgList.Length; ++i)
                    {
                        if (fileObject.FilePath == s_MsgName[i])
                        {
                            m_MsgList[i].Add(fileObject.FileName);
                        }
                    }
                }
            }
        }

        public void OutputMsg()
        {
            for (int i = 0; i < s_MsgTypeNum; ++i)
            {
                OutputMsg((QQMsgType)i);
            }
        }
        public void OutputMsg(QQMsgType type)
        {
            if (m_Storage == null) return;
            if (m_Password == null) return;

            int typeIndex = (int)type;
            if (typeIndex < 0 || typeIndex >= s_MsgTypeNum)
            {
                throw new ArgumentException("Invalid QQMsgType", "type");
            }

            string filePath = s_MsgName[typeIndex] + "\\";
            Directory.CreateDirectory(filePath);

            foreach (string QQID in m_MsgList[typeIndex])
            {
                string fileName = filePath + QQID + ".msj";
                OutputMsg(type, QQID, fileName);
            }
        }
        public void OutputMsg(QQMsgType type, string QQID)
        {
            if (m_Storage == null) return;
            if (m_Password == null) return;

            int typeIndex = (int)type;
            if (typeIndex < 0 || typeIndex >= s_MsgTypeNum)
            {
                throw new ArgumentException("Invalid QQMsgType", "type");
            }

            string filePath = s_MsgName[typeIndex] + "\\";
            Directory.CreateDirectory(filePath);

            string fileName = filePath + QQID + ".msj";
            OutputMsg(type, QQID, fileName);
        }

        private void OutputMsg(QQMsgType type, string QQID, string fileName)
        {
            string msgPath = s_MsgName[(int)type] + QQID;
            IList<byte[]> msgList = QQMsgMgr.DecryptMsg(m_Storage, msgPath, m_Password);
            Encoding encoding = Encoding.GetEncoding(936);

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < msgList.Count; ++i)
                    {
                        using (MemoryStream ms = new MemoryStream(msgList[i]))
                        {
                            using (BinaryReader br = new BinaryReader(ms, Encoding.GetEncoding(936)))
                            {
#if false
                                fs.Write(msgList[i], 0, msgList[i].Length);
#else
                                int ticks = br.ReadInt32();
                                DateTime time = new DateTime(1970, 1, 1) + new TimeSpan(0, 0, ticks);
                                switch (type)
                                {
                                    case QQMsgType.BIM:
                                    case QQMsgType.C2C:
                                    case QQMsgType.Mobile:
                                        ms.Seek(1, SeekOrigin.Current);
                                        break;
                                    case QQMsgType.Group:
                                        ms.Seek(8, SeekOrigin.Current);
                                        break;
                                    case QQMsgType.Sys:
                                        ms.Seek(4, SeekOrigin.Current);
                                        break;
                                    case QQMsgType.TempSession: //?
                                        ms.Seek(9, SeekOrigin.Current);
                                        break;
                                }
                                if (type == QQMsgType.TempSession)
                                {
                                    int gLen = br.ReadInt32();
                                    string groupName = encoding.GetString(br.ReadBytes(gLen));
                                    if (groupName.Length > 0) sw.WriteLine("{0}", groupName);
                                }
                                int nLen = br.ReadInt32();
                                string id = encoding.GetString(br.ReadBytes(nLen));
                                sw.WriteLine("{0}: {1}", id, time.ToString());
                                int cLen = br.ReadInt32();
                                string msg = encoding.GetString(br.ReadBytes(cLen));
                                msg.Replace("\n", Environment.NewLine);
                                sw.WriteLine(msg);
                                sw.WriteLine();
#endif
                            }
                        }
                    }
                }
            }
        }

        public void OutputFileList()
        {
            if (m_Storage == null) return;

            Dictionary<string, long> dic = new Dictionary<string, long>();
            foreach (IBaseStorageWrapper.FileObjects.FileObject fileObject in m_Storage.foCollection)
            {
                if (fileObject.FileType == 2 && fileObject.FileName == "Index.msj")
                {
                    dic[fileObject.FilePath] = fileObject.Length / 4;
                }
            }

            for (int i = 0; i < m_MsgList.Length; ++i)
            {
                Console.WriteLine("{0}", s_MsgName[i]);
                foreach (string ID in m_MsgList[i])
                {
                    Console.WriteLine("\t{0}: {1}", ID, dic[s_MsgName[i] + ID]);
                }
            }
        }

        private static IBaseStorageWrapper.FileObjects.FileObject GetStorageFileObject(IStorageWrapper iw, string path, string fileName)
        {
            foreach (IBaseStorageWrapper.FileObjects.FileObject fileObject in iw.foCollection)
            {
                if (fileObject.CanRead)
                {
                    if (fileObject.FilePath == path && fileObject.FileName == fileName) return fileObject;
                }
            }
            return null;
        }
        private static byte[] Decrypt(byte[] src, byte[] pass, long offset)
        {
            //RedQ.QQCrypt decryptor = new RedQ.QQCrypt();
            //return decryptor.QQ_Decrypt(src, pass, offset);
            byte[] b = new byte[2];
            return b;
        }

        private static IList<byte[]> DecryptMsg(IStorageWrapper iw, string path, byte[] pass)
        {
            List<byte[]> msgList = new List<byte[]>();

            int num = 0;
            int[] pos = null;
            int[] len = null;
            using (IBaseStorageWrapper.FileObjects.FileObject fileObject = GetStorageFileObject(iw, path, "Index.msj"))
            {
                if (fileObject == null) return msgList;
                int fileLen = (int)fileObject.Length;
                num = fileLen / 4;
                pos = new int[num + 1];
                using (BinaryReader br = new BinaryReader(fileObject))
                {
                    for (int i = 0; i < num; ++i)
                    {
                        pos[i] = br.ReadInt32();
                    }
                }
            }
            using (IBaseStorageWrapper.FileObjects.FileObject fileObject = GetStorageFileObject(iw, path, "Data.msj"))
            {
                if (fileObject != null)
                {
                    int fileLen = (int)fileObject.Length;
                    len = new int[num];
                    pos[num] = fileLen;
                    for (int i = 0; i < num; ++i)
                    {
                        len[i] = pos[i + 1] - pos[i];
                    }
                    using (BinaryReader br = new BinaryReader(fileObject))
                    {
                        for (int i = 0; i < num; ++i)
                        {
                            fileObject.Seek(pos[i], SeekOrigin.Begin);
                            byte[] data = br.ReadBytes(len[i]);
                            byte[] msg = Decrypt(data, pass, 0);
                            msgList.Add(msg);
                        }
                    }
                }
            }
            return msgList;
        }
        private static byte[] GetGlobalPass(IStorageWrapper iw, string QQID)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] dataID = new byte[QQID.Length];
            for (int i = 0; i < QQID.Length; ++i) dataID[i] = (byte)(QQID[i]);
            byte[] hashID = md5.ComputeHash(dataID);
            IBaseStorageWrapper.FileObjects.FileObject fileObject = GetStorageFileObject(iw, "Matrix", "Matrix.db");
            if (fileObject != null)
            {
                using (BinaryReader br = new BinaryReader(fileObject))
                {
                    byte[] data = br.ReadBytes((int)fileObject.Length);
                    long len = data.Length;
                    if (len < 6 || data[0] != 0x51 || data[1] != 0x44) return null;
                    if (len >= 32768) return null;

                    bool bl = false;
                    int i = 6;
                    while (i < len)
                    {
                        bl = false;
                        byte type = data[i++];
                        if (i + 2 > len) break;
                        int len1 = data[i] + data[i + 1] * 256;
                        byte xor1 = (byte)(data[i] ^ data[i + 1]);
                        i += 2;
                        if (i + len1 > len) break;
                        for (int j = 0; j < len1; ++j) data[i + j] = (byte)(~(data[i + j] ^ xor1));
                        if (len1 == 3 && data[i] == 0x43 && data[i + 1] == 0x52 && data[i + 2] == 0x4B)
                        {
                            bl = true;
                        }
                        i += len1;

                        if (type > 7) break;
                        if (i + 4 > len) break;
                        int len2 = data[i] + data[i + 1] * 256 + data[i + 2] * 256 * 256 + data[i + 3] * 256 * 256 * 256;
                        byte xor2 = (byte)(data[i] ^ data[i + 1]);
                        i += 4;
                        if (i + len2 > len) break;
                        if (type == 6 || type == 7)
                        {
                            for (int j = 0; j < len2; ++j) data[i + j] = (byte)(~(data[i + j] ^ xor2));
                        }
                        if (bl && len2 == 0x20)
                        {
                            byte[] dataT = new byte[len2];
                            for (int j = 0; j < len2; ++j) dataT[j] = data[i + j];
                            return Decrypt(dataT, hashID, 0);
                        }
                        i += len2;
                    }
                    if (i != len) return null;
                }
            }
            return null;
        }
    }
}
