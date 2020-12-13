using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surukleyici : MonoBehaviour {
    //Sürüklenebilir objelerin sahip olduğu tag için oluşturduğumuz değişken
    [SerializeField]
    string SuruklenebilirEtiket;
    //Takip için belirlediğimiz hiz
    [SerializeField]
    float takipHizi;

    //Farklı objelere erişeceğimiz için, kullanacağımız bileşenleri burada belirliyoruz. İhtiyaca göre bileşenleri atıyor veya null hale getiriyoruz.
    Rigidbody2D seciliRgb;
    Transform seciliTransform;
    //Seçilen objenin kinematik olup olmama bilgisini tuttuğumuz değişken
    bool kinematikMi;

    //Tıklama anında objenin açısı ve fare ile arasındaki mesafeyi tuttuğumuz değişkenler
    float baslangicAcisi;
    Vector2 baslangicMesafe;
    //Seçili olan objenin Y değerini tutuyoruz. Bu sayede taban ve merkez arasındaki mesafeyi elde edeceğiz.
    float yBuyuklugu;
    //Main Camera tagına sahip kamera objesini tutacağımız değişken.
    Camera anaKamera;

    void Start () {
        //Main Camera tagına sahip kameraya erişiyoruz.
        anaKamera = Camera.main;
    }

    void Update () {
        //Sol tık ile obje kontrolü yapıyoruz.
        if (Input.GetMouseButtonDown (0)) {
            ObjeyiBul ();
        }
        //Sol tık basılmayı bıraktığı anda sürükleme işlemini durdurmak için objeyi bırakma fonksiyonunu çağırıyoruz.
        if (Input.GetMouseButtonUp (0)) {
            ObjeyiBirak ();
        }
    }

    //Daha sağlıklı bir hareket sağlamak adıyla hareketi FixedUpdate içerisinde yapıyoruz. Eğer bir seçili bir rigidbody varsa hareket işlemini gerçekleştiriyoruz.
    void FixedUpdate () {
        if (seciliRgb != null) {
            ObjeyiSurukle ();
        }
    }

    void ObjeyiBul () {
        //Tıklama yaptığımız konumda bir obje var mı bunun verisini elde ediyoruz.
        RaycastHit2D hit2D = Physics2D.GetRayIntersection (anaKamera.ScreenPointToRay (Input.mousePosition));
        //Eğer bir obje bulduysak ve tagı belirlediğimiz tag ile aynıysa objenin bileşenlerini ve gerekli bilgileri değişkenlere kaydediyoruz.
        if (hit2D && hit2D.collider.tag.Equals (SuruklenebilirEtiket)) {
            seciliRgb = hit2D.collider.attachedRigidbody;
            if (seciliRgb == null) {
                Debug.LogWarning ("Rigidbody yok");
            } else {
                seciliTransform = hit2D.transform;
                kinematikMi = seciliRgb.isKinematic;
                seciliRgb.isKinematic = false;

                baslangicAcisi = seciliRgb.rotation;

                yBuyuklugu = hit2D.collider.bounds.extents.y * .5f;
                //Farenin merkeze göre konumunu alarak, konum farkını hesaplıyoruz.
                Vector2 mousePos = anaKamera.ScreenToWorldPoint (Input.mousePosition);
                baslangicMesafe = seciliRgb.position - mousePos;
            }
        }
    }

    void ObjeyiSurukle () {
        Vector2 mousePos = anaKamera.ScreenToWorldPoint (Input.mousePosition);

        //SmoothDamp fonksiyonları müdahale edeceğimiz için, velocity ve angularVelocity değerlerini değişkenlere alıyoruz.
        Vector2 velocity = seciliRgb.velocity;
        float angularVelocity = seciliRgb.angularVelocity;

        //Fare ve obje arasında başka bir obje var mı bunu kontrol etmek amacıyla LineCast oluşturuyoruz
        RaycastHit2D hit2D = Physics2D.Linecast (seciliRgb.position, mousePos);

        Vector2 hedefkonum;
        float hedefAci;

        //Engel olup olmama durumuna göre hedef konum ve hedef açı değerlerini hesaplıyoruz.
        if (hit2D) {
            /*
            Burada yaptığımız hesap şöyle
            Engel üzerinde ışının çarptığı noktaya
            yüzeyin baktığı yönü ekliyoruz. Ancak objenin y boyutuyla bunu çarpıyoruz.
            Normal değeri 1 birimlik uzunluğa sahip olduğu için, örneğin yBuyukluğu .5 ise, 
            engel yüzeyinde belirlediğimiz noktadan .5 birim (objenin baktığı yöne doğru) uzağa 
            konumlandırma yapıyoruz. 
            */
            hedefkonum = hit2D.point + hit2D.normal * yBuyuklugu;;
            //Yüzey ve objenin vector2 tipindeki yön değerleri arasındaki açıyı elde ediyoruz.
            float yuzeyleAradakiAci = Vector2.SignedAngle (seciliTransform.up, hit2D.normal);
            //Float tipindeki bu farkı, varolan float tipindeki açıya eklediğimizde erişmek istediğimiz açıyı elde etmiş oluyoruz.
            hedefAci = seciliRgb.rotation + yuzeyleAradakiAci;
        } else {
            //Eğer fareye gideceksek, başlangıçtaki fare-merkez farkını korumak için, ilk tıklamada elde ettiğimiz mesafeyi fare pozisyonuna ekliyoruz.
            hedefkonum = mousePos + baslangicMesafe;
            hedefAci = baslangicAcisi;
        }
        /*
        Burada mesafe hesaplamamızın sebebi, SmoothDamp içerisinde hareketin hızını değil, süresini belirtmemiz.
        Basitçe şöyle düşünebliriz, 15 birimlik mesafeyi 3 birim hızla gidersek, 5 saniyede sürecektir işlem. 
        3 birim uzaklıkta ise, 1 saniyede. Bu mantığa dayanarak ve hedefle obje arasındaki mesafe sürekli değişeceğinden
        bunu belirtiyoruz ve "smoothTime" değişkeni olarak mesafe/takipHizi değerini veriyoruz.
         */
        float mesafe = Vector2.Distance (seciliRgb.position, hedefkonum);
        Vector2.SmoothDamp (seciliRgb.position, hedefkonum, ref velocity, mesafe / takipHizi);
        Mathf.SmoothDamp (seciliRgb.rotation, hedefAci, ref angularVelocity, mesafe / takipHizi);

        seciliRgb.velocity = velocity;
        seciliRgb.angularVelocity = angularVelocity;
    }
    /*
    Sürüklemeyi bırakmak için daha önce kaydettiğimiz değerleri boşaltıyoruz. Kinematik olma durumunu da tekrar objeye kazandırıyoruz.
    Başta yaptığımız kontrol ile "null exception" hatasını önlemek için. Sonuçta secili bir rigidbody yoksa ona erişemeyiz.
    */
    void ObjeyiBirak () {
        if (seciliRgb == null) {
            return;
        }
        seciliRgb.isKinematic = kinematikMi;
        if (kinematikMi) {
            seciliRgb.velocity = Vector2.zero;
            seciliRgb.angularVelocity = 0;
        }
        seciliRgb = null;
        baslangicAcisi = 0;
        yBuyuklugu = 0;
    }
}
