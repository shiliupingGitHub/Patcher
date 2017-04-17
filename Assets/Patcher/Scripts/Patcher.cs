using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LiteJSON;
using System;

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
        public void AddElem(Elem e)
        {
            mElems.Add(e);
        }
        public void Serialize(string outPath)
        {
            string txt = Json.Serialize(this);
            File.WriteAllText(outPath, txt);
        }
        public static  PatcherElem DeSerialize(string txt)
        {
           return Json.Deserialize<PatcherElem>(txt);
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
}
