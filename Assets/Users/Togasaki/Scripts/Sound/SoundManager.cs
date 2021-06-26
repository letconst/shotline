using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    ///�ϐ�//////////////////////////////////////////////////////////////////////////
    

    //BGM�ESE�̃p�X�i�X�g�����O�j
    private const string BGM_PATH = "Audio/BGM";
    private const string SE_PATH = "Audio/SE";

    //BGM�ESE�̃{�����[��
    [SerializeField] private float BGM_VOLUME = 1;
    [SerializeField] private float SE_VOLUME = 1;

    //�I�[�f�B�I�\�[�X
    private AudioSource bgmSource;
    private AudioSource seSource;

    //AudioClip
    AudioClip[] bgm;
    AudioClip[] se;

    //BGM��SE��Dictionary
    private Dictionary<string, AudioClip> bgmClipDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> seClipDic = new Dictionary<string, AudioClip>();

    ///////////////////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        //�I�[�f�B�I�\�[�X����
        bgmSource = gameObject.AddComponent<AudioSource>();
        seSource = gameObject.AddComponent<AudioSource>();

        //BGM_PATH�ESE_PATH�̃t�@�C�����̃I�[�f�B�I�N���b�v��z���
        bgm = Resources.LoadAll<AudioClip>(BGM_PATH);
        se = Resources.LoadAll<AudioClip>(SE_PATH);

        //����
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

    
    //BGM�Đ�
    public void PlayBGM(BGMLabel bgmLabel)
    {
        bgmSource.clip = bgmClipDic[bgmLabel.ToString()];
        bgmSource.Play();
    }


    //SE�Đ�
    public void PlaySE(SELabel seLabel)
    {
        seSource.clip = seClipDic[seLabel.ToString()];
        seSource.PlayOneShot(seSource.clip);
    }

    //BGM��~
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    //SE��~
    public void StopSE()
    {
        seSource.Stop();
    }

}

//BGM�ꗗ(���O�̓t�@�C�����Ɠ����ɂ��Ă�������)
public enum BGMLabel
{
    None,
    Title,
    MainGame,
    Result
}

//SE�ꗗ(���O�̓t�@�C�����Ɠ����ɂ��Ă�������)
public enum SELabel
{
    None,
    Shot,
    Damage,
    Start,
    ItemGet,
    ItemUse
}
