using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BasicSoundManager : SingletonMonoBehaviour<BasicSoundManager>
{
    //BGM�ESE�̃p�X
    private const string BGM_PATH = "Audio/BGM";
    private const string SE_PATH = "Audio/SE";
    
    //BGM�̐��ASE�̐�
    private int BGM_SOURCE_NUM = 1;
    private int SE_SOURCE_NUM = 5;
    
    //�t�F�[�h�A�E�g�̏�������
    [SerializeField] private float FADE_OUT_SECONDO = 0.5f;
    
    //BGM�ESE�̃{�����[��
    [SerializeField] private float BGM_VOLUME = 0.5f;
    [SerializeField] private float SE_VOLUME = 0.3f;

    //�t�F�[�h�A�E�g�n
    private bool isFadeOut = false;
    private float fadeDeltaTime = 0f;

    //SE�ԍ�
    private int nextSESourceNum = 0;

    //BGM�ԍ�
    private BGMLabel currentBGM = BGMLabel.None;
    private BGMLabel nextBGM = BGMLabel.None;

    //BGM��p�̃I�[�f�B�I�\�[�X
    private AudioSource bgmSource;

    //SE�̃��X�g
    private List<AudioSource> seSourceList;

    //BGM��SE��Dictionary
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


    /// �w�肵���t�@�C������SE�𗬂��B��������delay�Ɏw�肵�����Ԃ����Đ��܂ł̊Ԋu���󂯂�
    public void PlaySE(SELabel seLabel, float delay = 0.0f) => StartCoroutine(DelayPlaySE(seLabel, delay));

    private IEnumerator DelayPlaySE(SELabel seLabel, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource se = seSourceList[nextSESourceNum];
        se.PlayOneShot(seClipDic[seLabel.ToString()]);
        nextSESourceNum = (++nextSESourceNum < SE_SOURCE_NUM) ? nextSESourceNum : 0;
    }


    /// �w�肵��BGM�𗬂��B���łɗ���Ă���ꍇ��Next�ɗ\�񂵁A����Ă���BGM���t�F�[�h�A�E�g������
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
                Debug.LogError($"bgmClipDic��{bgmLabel.ToString()}�Ƃ���Key�͂���܂���");
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


    /// BGM���~�߂�
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
    Shot,
    Damage,
    Start,
    ItemGet,
    ItemUse
}
