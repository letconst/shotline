using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    //�ϐ��]�[��/////////////////////////////////////////////////////////////

    //�I�[�f�B�I�t�@�C���̃p�X
    private const string BGM_PATH = "Audio/BGM";
    private const string SE_PATH = "Audio/SE";

    //BGM��SE�̐�
    private const int BGM_SOURCE_NUM = 1;
    private const int SE_SOURCE_NUM = 5;

    //����
    private float BGMVolume = 1f;
    private float SEVolume = 1f;

    //AudioSource
    private AudioSource bgmSource;
    private List<AudioSource> seSourceList;

    //Dictionary
    private Dictionary<string, AudioClip> seClipDic;
    private Dictionary<string, AudioClip> bgmClipDic;

    /////////////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < SE_SOURCE_NUM + BGM_SOURCE_NUM; i++)
        {
            gameObject.AddComponent<AudioSource>();
        }

        bgmClipDic = (Resources.LoadAll(BGM_PATH) as Object[]).ToDictionary(bgm => bgm.name, bgm => (AudioClip)bgm);
        seClipDic = (Resources.LoadAll(SE_PATH) as Object[]).ToDictionary(se => se.name, se => (AudioClip)se);
    }

    //BGM�Đ�
    public static void PlayBGM(BGMLabel bGMLabel)
    {
        if (!Instance.bgmSource.isPlaying)
        {
            if (Instance.bgmClipDic.ContainsKey(bGMLabel.ToString()))
            {
                Instance.bgmSource.clip = Instance.bgmClipDic[bGMLabel.ToString()];
            }
            else
            {
                Debug.LogError($"bgmClipDic��{bGMLabel.ToString()}�Ƃ���Key�͂���܂���B");
            }

            Instance.bgmSource.Play();
        }
    }
    //SE�Đ�
    public static void PlaySE(SELabel seLabel)
    {

    }

    //BGM��~
    public static void StopBGM()
    {
        Instance.bgmSource.Stop();
    }

}
public enum BGMLabel
{
    Title,
    MainGame,
    Result
}

public enum SELabel
{
    Shot,
    Damage,
    Start,
    ItemGet,
    ItemUse
}
