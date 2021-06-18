using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BasicSoundManager : SingletonMonoBehaviour<BasicSoundManager>
{
    //BGM・SEのパス
    private const string BGM_PATH = "Audio/BGM";
    private const string SE_PATH = "Audio/SE";
    
    //BGMの数、SEの数
    private int BGM_SOURCE_NUM = 1;
    private int SE_SOURCE_NUM = 5;
    
    //フェードアウトの初期時間
    [SerializeField] private float FADE_OUT_SECONDO = 0.5f;
    
    //BGM・SEのボリューム
    [SerializeField] private float BGM_VOLUME = 0.5f;
    [SerializeField] private float SE_VOLUME = 0.3f;

    //フェードアウト系
    private bool isFadeOut = false;
    private float fadeDeltaTime = 0f;

    //SE番号
    private int nextSESourceNum = 0;

    //BGM番号
    private BGMLabel currentBGM = BGMLabel.None;
    private BGMLabel nextBGM = BGMLabel.None;

    //BGM専用のオーディオソース
    private AudioSource bgmSource;

    //SEのリスト
    private List<AudioSource> seSourceList;

    //BGMとSEのDictionary
    private Dictionary<string, AudioClip> seClipDic;
    private Dictionary<string, AudioClip> bgmClipDic;




    protected override void Awake()
    {

        for (int i = 0; i < SE_SOURCE_NUM + BGM_SOURCE_NUM; i++)
        {
            gameObject.AddComponent<AudioSource>();
        }

        IEnumerable<AudioSource> audioSources = GetComponents<AudioSource>().Select(a => { a.playOnAwake = false; a.volume = BGM_VOLUME; a.loop = true; return a; });
        bgmSource = audioSources.First();
        seSourceList = audioSources.Skip(BGM_SOURCE_NUM).ToList();
        seSourceList.ForEach(a => { a.volume = SE_VOLUME; a.loop = false; });

        bgmClipDic = (Resources.LoadAll(BGM_PATH) as Object[]).ToDictionary(bgm => bgm.name, bgm => (AudioClip)bgm);
        seClipDic = (Resources.LoadAll(SE_PATH) as Object[]).ToDictionary(se => se.name, se => (AudioClip)se);
    }


    /// 指定したファイル名のSEを流す。第二引数のdelayに指定した時間だけ再生までの間隔を空ける
    public void PlaySE(SELabel seLabel, float delay = 0.0f) => StartCoroutine(DelayPlaySE(seLabel, delay));

    private IEnumerator DelayPlaySE(SELabel seLabel, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource se = seSourceList[nextSESourceNum];
        se.PlayOneShot(seClipDic[seLabel.ToString()]);
        nextSESourceNum = (++nextSESourceNum < SE_SOURCE_NUM) ? nextSESourceNum : 0;
    }


    /// 指定したBGMを流す。すでに流れている場合はNextに予約し、流れているBGMをフェードアウトさせる
    public void PlayBGM(BGMLabel bgmLabel)
    {
        if (!bgmSource.isPlaying)
        {
            currentBGM = bgmLabel;
            nextBGM = BGMLabel.None;
            if (bgmClipDic.ContainsKey(bgmLabel.ToString()))
            {
                bgmSource.clip = bgmClipDic[bgmLabel.ToString()];
            }
            else
            {
                Debug.LogError($"bgmClipDicに{bgmLabel.ToString()}というKeyはありません");
            }
            bgmSource.Play();
        }
        else if (currentBGM != bgmLabel)
        {
            isFadeOut = true;
            nextBGM = bgmLabel;
            fadeDeltaTime = 0f;
        }
    }


    /// BGMを止める
    public void StopSound()
    {
        bgmSource.Stop();
        seSourceList.ForEach(a => { a.Stop(); });
    }


    private void Update()
    {
        if (isFadeOut)
        {
            fadeDeltaTime += Time.deltaTime;
            bgmSource.volume = (1.0f - fadeDeltaTime / FADE_OUT_SECONDO) * BGM_VOLUME;

            if (fadeDeltaTime >= FADE_OUT_SECONDO)
            {
                isFadeOut = false;
                bgmSource.Stop();
            }
        }
        else if (nextBGM != BGMLabel.None)
        {
            bgmSource.volume = BGM_VOLUME;
            PlayBGM(nextBGM);
        }
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
    Shot,
    Damage,
    Start,
    ItemGet,
    ItemUse
}
