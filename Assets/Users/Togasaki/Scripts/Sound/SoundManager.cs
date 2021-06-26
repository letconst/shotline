using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    ///変数//////////////////////////////////////////////////////////////////////////
    

    //BGM・SEのパス（ストリング）
    private const string BGM_PATH = "Audio/BGM";
    private const string SE_PATH = "Audio/SE";

    //BGM・SEのボリューム
    [SerializeField] private float BGM_VOLUME = 1;
    [SerializeField] private float SE_VOLUME = 1;

    //オーディオソース
    private AudioSource bgmSource;
    private AudioSource seSource;

    //AudioClip
    AudioClip[] bgm;
    AudioClip[] se;

    //BGMとSEのDictionary
    private Dictionary<string, AudioClip> bgmClipDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> seClipDic = new Dictionary<string, AudioClip>();

    ///////////////////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        //オーディオソース生成
        bgmSource = gameObject.AddComponent<AudioSource>();
        seSource = gameObject.AddComponent<AudioSource>();

        //BGM_PATH・SE_PATHのファイル下のオーディオクリップを配列へ
        bgm = Resources.LoadAll<AudioClip>(BGM_PATH);
        se = Resources.LoadAll<AudioClip>(SE_PATH);

        //音量
        bgmSource.volume = BGM_VOLUME;
        seSource.volume = SE_VOLUME;


        foreach (AudioClip b in bgm)
        {
            bgmClipDic.Add(b.name, b);
        }

        foreach (AudioClip s in se)
        {
            seClipDic.Add(s.name, s);
        }

    }

    
    //BGM再生
    public void PlayBGM(BGMLabel bgmLabel)
    {
        bgmSource.clip = bgmClipDic[bgmLabel.ToString()];
        bgmSource.Play();
    }


    //SE再生
    public void PlaySE(SELabel seLabel)
    {
        seSource.clip = seClipDic[seLabel.ToString()];
        seSource.PlayOneShot(seSource.clip);
    }

    //BGM停止
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    //SE停止
    public void StopSE()
    {
        seSource.Stop();
    }

}

//BGM一覧(名前はファイル名と同じにしてください)
public enum BGMLabel
{
    None,
    Title,
    MainGame,
    Result
}

//SE一覧(名前はファイル名と同じにしてください)
public enum SELabel
{
    None,
    Shot,
    Damage,
    Start,
    ItemGet,
    ItemUse
}
