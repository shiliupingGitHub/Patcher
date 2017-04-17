using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LiteJSON;
using System;
using System.IO.Compression;
public class Patcher  {
    #region Elems
    public class PatcherElem : IJsonSerializable, IJsonDeserializable
    {
        public class Elem  : IJsonSerializable, IJsonDeserializable
        {
            public string szName;
            public string mVersion;
            public string[] mDepends;
            public   JsonObject ToJson()
            {
                JsonObject obj = new JsonObject();
                obj.Put("name", szName);
                obj.Put("version", mVersion);
                obj.Put("depends", mDepends);
                return obj;
            }

            public void FromJson(JsonObject jsonObject)
            {
                szName = jsonObject.GetString("name");
                mVersion = jsonObject.GetString("version");
                mDepends = jsonObject.GetJsonArray("depends").ToArrayString();
            }
        }
        List<Elem> mElems = new List<Elem>();
        public Dictionary<string, Elem> mDic = new Dictionary<string, Elem>();
        public void AddElem(Elem e)
        {
            mElems.Add(e);
        }
        public void Serialize(string outPath)
        {
            string txt = Json.Serialize(this);
            File.WriteAllText(outPath, txt);
        }
        void CreatDic()
        {
            mDic.Clear();
            foreach(var e in mElems)
            {
                mDic[e.szName] = e;
            }
        }
        List<PatcherElem.Elem> Compare(PatcherElem elem)
        {
            List<PatcherElem.Elem> ret = new List<Elem>();
            foreach(var d in elem.mDic)
            {
                Elem me = null;
                mDic.TryGetValue(d.Key, out me);
                if(me == null)
                {
                    ret.Add(d.Value);
                    continue;
                }
                else if(me.mVersion != d.Value.mVersion)
                {
                    ret.Add(d.Value);
                    continue;
                }
                else
                {
                    string path = Application.persistentDataPath + "/" + d.Key;
                    if(!File.Exists(path))
                    {
                        ret.Add(d.Value);
                        continue;
                    }
                }
            }
            return ret;
        }
        public static  PatcherElem DeSerialize(string txt)
        {
            PatcherElem ret = Json.Deserialize<PatcherElem>(txt);
            ret.CreatDic();
           return  ret;
        }
        public JsonObject ToJson()
        {
            JsonObject obj = new JsonObject();
            obj.Put("content", mElems);
            return obj;
        }

        public void FromJson(JsonObject jsonObject)
        {
           mElems = jsonObject.GetJsonArray("c").DeserializeToList<Elem>();
        }
    }
    #endregion
    public static string GetABsPath(RuntimePlatform plat)
    {
        switch(plat)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "IOS";
            default:
                return "PC";
        }
    }
    static int mVersion = 0;
    public static PatcherElem mCurElems = null;
    public  static void InitVersion(int v)
    {
        mVersion = v;
    }
    public static void UnPackFiles()
    {
       int v =  PlayerPrefs.GetInt("UnPackVersion", 0);
        if(mVersion > v)
        {
           TextAsset  ta= (TextAsset)  Resources.Load("Game");

            using (MemoryStream ms = new MemoryStream(ta.bytes))
            {
                ZipStorer zip = ZipStorer.Open(ms, FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    string outPath = Application.persistentDataPath + "/" + entry.FilenameInZip;
                    zip.ExtractFile(entry, outPath);
                    
                }
                zip.Close();
            }
            PlayerPrefs.SetInt("UnPackVersion", mVersion);



        }
    }
}
