using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("-----TOP AYARLARI")]
    [SerializeField] private GameObject[] Toplar;
    int AktifTopIndex;
    [SerializeField] Transform FirePoint;
    [SerializeField] private float TopGucu;
    [SerializeField] private Animator _TopAtar;
    [SerializeField] private ParticleSystem TopAtmaEfekt;
    [SerializeField] private ParticleSystem[] TopEfektleri;
    int AktifTopEfektiIndex;
    [SerializeField] private AudioSource[] TopSesleri;
    int AktifTopSesIndex;
    int AktifTopSayisi;
    [Header("-----LEVEL AYARLARI")]
    [SerializeField] private int HedefTopSayisi;
    [SerializeField] private int MevcutTopSayisi;
    int GirenTopSayisi;
    [SerializeField] private Slider LevelSlider;
    [SerializeField] private TextMeshProUGUI KalanTopSayisi_Text;

    [Header("-----UI AYARLARI")]
    [SerializeField] private GameObject[] paneller;
    [SerializeField] private TextMeshProUGUI YildizSayisi;
    [SerializeField] private TextMeshProUGUI Kazandin_LevelSayisi;
    [SerializeField] private TextMeshProUGUI Kaybettin_LevelSayisi;

    [Header("-----DIGER AYARLAR")]
    [SerializeField] private Renderer KovaSeffaf;
    float KovaninBaslangicDegeri;
    float KovaStepDegeri;
    [SerializeField] private AudioSource[] DigerSesler;
    int AktifSahneIndex;
    string LevelAdi;
    void Start()
    {
        KovaninBaslangicDegeri = .5f;
        KovaStepDegeri = .25f / HedefTopSayisi;
        LevelSlider.maxValue = HedefTopSayisi;
        KalanTopSayisi_Text.text = MevcutTopSayisi.ToString();
        AktifSahneIndex = SceneManager.GetActiveScene().buildIndex;
        LevelAdi = SceneManager.GetActiveScene().name;
        AktifTopEfektiIndex = 0;
    }   
    public void TopGirdi()
    {
        GirenTopSayisi++;
        LevelSlider.value = GirenTopSayisi;
        KovaninBaslangicDegeri -= KovaStepDegeri;
        KovaSeffaf.material.SetTextureScale("_MainTex", new Vector2(1f, KovaninBaslangicDegeri));

        TopSesleri[AktifTopSesIndex].Play();
        AktifTopSesIndex++;

        if (AktifTopSesIndex == TopSesleri.Length - 1)
        {
            AktifTopSesIndex = 0;
        }

        if (GirenTopSayisi == HedefTopSayisi)
        {          
            DigerSesler[1].Play();
            PlayerPrefs.SetInt("Level", AktifSahneIndex + 1);
            PlayerPrefs.SetInt("Yildiz", PlayerPrefs.GetInt("Yildiz")+15);
            YildizSayisi.text = PlayerPrefs.GetInt("Yildiz").ToString();
            Kazandin_LevelSayisi.text = "LEVEL : "+LevelAdi;
            paneller[1].SetActive(true);
            Time.timeScale = 0;
        }

        AktifTopSayisi = 0;

        foreach (var top in Toplar)
        {
            if (top.activeInHierarchy)
                AktifTopSayisi++;
        }

        if(AktifTopSayisi == 0)
        {
            if (MevcutTopSayisi == 0 && GirenTopSayisi != HedefTopSayisi)
            {
                Kaybettin();
            }
            if ((MevcutTopSayisi + GirenTopSayisi) < HedefTopSayisi)
            {
                Kaybettin();
            }
        }
        
    }
    public void TopGirmedi()
    {
        AktifTopSayisi = 0;
        //Oyuncu son toplarýný hýzlýca ateþlediðinde ve elinde top kalmadýðý zaman 
        //Oyuncu top havadayken oyunu kaybediyor.
        //Bu hatayý önlemek için sahnede top olup olmadýðýný kontrol ettim.
        foreach (var top in Toplar)
        {
            if (top.activeInHierarchy)
                AktifTopSayisi++;
        }
        //Sahnede top yoksa kontrol ediliyor.
        if (AktifTopSayisi == 0)
        {
            if (MevcutTopSayisi == 0)
            {
                Kaybettin();
            }
            if ((MevcutTopSayisi + GirenTopSayisi) < HedefTopSayisi)
            {
                Kaybettin();
            }
        }
    }
    public void OyunuDurdur()
    {
        paneller[0].SetActive(true);
        Time.timeScale = 0;
    }
    public void PanellericinButonislemi(string islem)
    {
        switch (islem)
        {
            case "Devamet":
                Time.timeScale = 1;
                paneller[0].SetActive(false);
                break;
            case "Cikis":
                Application.Quit();
                break;
            case "Ayarlar"://RUN CRONTROLDAKÝ GÝBÝ PANEL CIKARTABÝLÝRÝSÝN
                break;
            case "Tekrar":
                Time.timeScale = 1;
                SceneManager.LoadScene(AktifSahneIndex);
                break;
            case "Birsonraki":
                Time.timeScale = 1;
                SceneManager.LoadScene(AktifSahneIndex+1);
                break;
        }
    }
    public void ParcEfektOrtayaCikart(Vector3 pozisyon, Color TopRengi)
    {
        TopEfektleri[AktifTopEfektiIndex].transform.position = pozisyon;

        var main = TopEfektleri[AktifTopEfektiIndex].main;
        main.startColor = TopRengi;

        TopEfektleri[AktifTopEfektiIndex].gameObject.SetActive(true);
        AktifTopEfektiIndex++;

        if (AktifTopEfektiIndex == TopEfektleri.Length - 1)
        {
            AktifTopEfektiIndex = 0;
        }
    }
    void Kaybettin()
    {
        Time.timeScale = 0;
        DigerSesler[0].Play();
        Kaybettin_LevelSayisi.text = "LEVEL : " + LevelAdi;
        paneller[2].SetActive(true);
    }
    public void TopAt()
    {
        if (Time.timeScale != 0 && MevcutTopSayisi > 0)
        {           
                MevcutTopSayisi--;
                KalanTopSayisi_Text.text = MevcutTopSayisi.ToString();
                _TopAtar.Play("TopAtar");
                TopAtmaEfekt.Play();
                DigerSesler[2].Play();
                Toplar[AktifTopIndex].transform.SetPositionAndRotation(FirePoint.position, FirePoint.rotation);
                Toplar[AktifTopIndex].SetActive(true);
                Toplar[AktifTopIndex].GetComponent<Rigidbody>().AddForce(Toplar[AktifTopIndex].transform.TransformDirection(90, 90, 0) * TopGucu, ForceMode.Force);
                if (Toplar.Length - 1 == AktifTopIndex)
                {
                    AktifTopIndex = 0;
                }
                else
                {
                    AktifTopIndex++;
                }          
        }
    }
}
