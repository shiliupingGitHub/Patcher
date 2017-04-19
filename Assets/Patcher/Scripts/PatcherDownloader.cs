using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
public class PatcherDownloader : MonoBehaviour
{
    public enum STATUS
    {
        NONE,
        DOWNLOADING,
        ERROR,
    }

    Patcher.PatcherElem mDownloads;
    Patcher.PatcherElem mDownloadeds;
    System.Action<string> mOnError;
    System.Action<Patcher.PatcherElem> mOnFinish;
    public string mPath;
    public int mTotal = 0;
    public int mLeft = 0;
    STATUS mStatus = STATUS.NONE;
    void Update()
    {
        if (mDownloads == null)
            return;
        if (mStatus != STATUS.DOWNLOADING)
            return;
        if (mDownloads.mElems.Count == 0)
        {
            if (null != mOnFinish)
                mOnFinish(mDownloadeds);
        }
        mLeft = 0;
        List<Patcher.PatcherElem.Elem> rm = new List<Patcher.PatcherElem.Elem>();
        foreach (var e in mDownloads.mElems)
        {
            if (e.mDonload.isDone)
            {
                if (e.mDonload.error == null)
                {

                    File.WriteAllBytes(e.ResPath, e.mDonload.bytes);
                    e.mDonload = null;
                    rm.Add(e);
                }
                else
                {
                    mStatus = STATUS.ERROR;
                }
            }
            else
            {
                mLeft = Mathf.Max(0, e.mLength - e.mDonload.size);
            }
        }

        bool mWrite = false;
        foreach (var r in rm)
        {
            mDownloads.RemoveElem(r);
            mDownloadeds.AddElem(r);
            mWrite = true;
        }
        if (mWrite)
        {
            string localPath = Application.persistentDataPath + "/Pather.txt";
            mDownloadeds.Serialize(localPath);
        }
    }
   public  void BeginDownload(string remotePath, System.Action<Patcher.PatcherElem> onFinish)
    {
        mOnFinish = onFinish;
        mPath = remotePath;
       StartCoroutine(Donload());
    }
    public IEnumerator Donload()
    {
        string remotePath = mPath + "/Pather.txt";
        string localPath = Application.persistentDataPath + "/Pather.txt"; ;
        WWW www = new WWW(remotePath);
        yield return www;
        if (www.error == null)
        {
            mDownloads = Patcher.PatcherElem.DeSerialize(www.text);
            if (File.Exists(localPath))
            {
                string szLocal = File.ReadAllText(localPath);
                mDownloadeds = Patcher.PatcherElem.DeSerialize(szLocal);
            }
            else
                mDownloadeds = new Patcher.PatcherElem();
            List<Patcher.PatcherElem.Elem> rm = new List<Patcher.PatcherElem.Elem>();
            mTotal = 0;
            foreach (var e in mDownloads.mElems)
            {
                if (mDownloadeds.IsChange(e))
                {
                    e.mDonload = new WWW(mPath + "/" + e.szName);
                    mTotal += e.mLength;
                }
                else
                    rm.Add(e);
            }

            foreach (var r in rm)
            {
                mDownloads.RemoveElem(r);
            }
            mStatus = STATUS.DOWNLOADING;
        }
        else if (null != mOnError)
        {
            mOnError(www.error);
            mStatus = STATUS.ERROR;
        }

    }
}
