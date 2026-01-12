# VFX ARTIST CASE STUDY

**Konu:** Tower Defense Attack & Impact VFX  
**Aday:** Burak Darende  
**Tarih:** Ocak 2026  
**Tools:** Unity 2022.3 (URP), Shader Graph, Particle System  
**Download:** [Unity Package](https://sendgb.com/8c0HWBXVNzi)

![Demo GIF](https://raw.githubusercontent.com/burakdarende/PanteonVFXCase/refs/heads/main/_Source/demo.gif)

### Referanslar (References)

| | | |
|:---:|:---:|:---:|
| ![Ref 1](https://raw.githubusercontent.com/burakdarende/PanteonVFXCase/main/_Source/ref/Screenshot%202026-01-09%20092424.png) | ![Ref 2](https://raw.githubusercontent.com/burakdarende/PanteonVFXCase/main/_Source/ref/Screenshot%202026-01-09%20165423.png) | ![Ref 3](https://raw.githubusercontent.com/burakdarende/PanteonVFXCase/main/_Source/ref/Screenshot%202026-01-09%20165813.png) |
| ![Ref 4](https://raw.githubusercontent.com/burakdarende/PanteonVFXCase/main/_Source/ref/Screenshot%202026-01-09%20165825.png) | ![Ref 5](https://raw.githubusercontent.com/burakdarende/PanteonVFXCase/main/_Source/ref/Screenshot%202026-01-09%20165853.png) | ![Ref 6](https://raw.githubusercontent.com/burakdarende/PanteonVFXCase/main/_Source/ref/Screenshot%202026-01-12%20143131.png) |


---

## 1. KONSEPT & YAKLAŞIM (Concept & Approach)
Bu case study için Hyper-Casual ve Hybrid-Casual oyun dinamiklerine uygun, **"Stylized, Snappy ve Satisfying"** bir görsel dil hedefledim.

* **Art Direction:** Stylized/Toon (Non-Photorealistic).
* **Temel Prensipler:**
    * **Readability:** Küçük mobil ekranlarda bile Projectile yolu ve Impact noktası anında okunabilir olmalı.
    * **Impact:** Vuruş anı yüksek kontrast ve doğru zamanlamayla "Juicy" hissettirmeli.
    * **Performance:** Tüm efektler sıkı mobil optimizasyon kurallarına (Low Overdraw, Batching-friendly) sadık kalarak tasarlandı.

---

## 2. TEKNİK DETAYLAR (Technical Breakdown)

### A. Hybrid Architecture: Procedural Ring Shader
Case dosyasındaki "Bonus" isterini karşılamak ve görsel kaliteyi artırmak adına **Hibrit** bir yapı kurdum. Standart efektler (Sparks, Dust) için optimize edilmiş Particle System kullanırken, efektin ana unsuru olan Impact Ring için Shader Graph ile kontrol edilen Mesh tabanlı özel bir çözüm geliştirdim.

* **Neden Hibrit?:** Tüm sistemi Mesh ile yapmak mobil performansı (Vertex Count) olumsuz etkilerdi. Bu yüzden sadece görselin en kritik parçası olan "Şok Dalgası"nda (Shockwave) Mesh teknolojisini kullandım.
* **Teknik Mantık:**
    * Ring Mesh'in UV haritasını **Linear Strip** (Düz şerit) tekniğiyle açarak Y eksenindeki gradyanı kontrol edilebilir hale getirdim.
    * **Shader Math:** `UV(y)` -> `Split` -> `Absolute` -> `Smoothstep`.
    * **Sonuç:** Bu sayede texture artifact'leri olmadan sonsuz çözünürlükte, **Thickness** ve **Softness** değerleri dinamik olarak değişebilen bir halka elde ettim.
* **Particle System Entegrasyonu:**
    * Shader parametrelerini script ile update etmek yerine, Particle System'ın **Custom Vertex Streams** özelliğini kullandım.
    * Particle'ın `Lifetime` verisini Shader'a stream ederek, halkanın zamanla incelmesini (Thickness fade-out) tamamen GPU tarafında çözdüm.


### B. Projectile Deformation & Trail
Merminin hız hissini artırmak için Projectile Core kısmında **"Stretched Billboard"** render modunu tercih ettim.
* **Deformation:** Mermi hızına bağlı olarak (Velocity Scale) mesh otomatik olarak uzuyor/kısalıyor.
* **Trail System:** Arkadaki enerji izi için **Rate over Time** yerine **Rate over Distance** kullandım. Bu sayede projectile çok hızlansa bile "dotted line" (kesik çizgi) sorunu oluşmuyor ve smooth bir trail elde ediliyor.

---

## 3. MİMARİ VE OPTİMİZASYON (Architecture & Optimization)

### A. Sub-Emitter Architecture
Kod tarafında karmaşıklığı önlemek için **"Self-Contained Prefab"** mimarisi kurdum.
* **Yapı:** `Projectile` (Parent) ve `Impact` (Child - Sub-Emitter).
* **Trigger Mantığı:** Impact efekti, kodla çağrılmak yerine Projectile'ın **Death (Collision)** event'i ile otomatik tetikleniyor.
* **Avantajı:** Runtime'da sürekli `Instantiate/Destroy` yapmak yerine Unity'nin kendi particle döngüsünü kullanarak **Garbage Collection (GC)** yükünü minimize ettim.

### B. Mobile Optimization
1.  **Fake Glow (Additive Shader):** Mobil GPU'yu yoran Real-time Light kullanmak yerine, **HDR Color** destekli `Unlit/Transparent/Additive` shader'lar ile "Fake Glow" illüzyonu yarattım.
2.  **Depth Texture (Soft Particles):** URP Asset ayarlarında **Depth Texture** özelliğini aktif ederek, zeminle kesişen efektlerdeki sert "Clipping" sorununu giderdim.
3.  **Mesh Particles:** Debris (Enkaz parçaları) için Low-poly mesh'ler ve basit primitive collider'lar kullandım.

---

## 4. ENTEGRASYON REHBERİ (Integration Guide)

VFX sistemi, developer'lar için "Plug & Play" mantığında hazırlandı.

* **Prefab:** `TowerAttack_VFX`
* **Kullanım:** Controller script'inde sadece `Fire()` metodunu çağırmanız yeterlidir.
    * Sistem, **Muzzle Flash** (Statik) ve **Projectile** (Dinamik) parçalarını tek bir root altında, transform sorunları yaşamadan yönetir.
* **Object Pooling:** Yapı, Object Pooling sistemleriyle uyumludur. Ana root üzerinde `Stop(Clear)` kullanılmadığı için, rapid-fire (seri atış) durumunda havadaki mermiler ve trailleri silinmeden yaşamaya devam eder.

```csharp
// Örnek Kullanım
public void Fire()
{
    weaponVFXRoot.Play(true);
}
```
---

## 5. KAYNAK KODLAR (Source Code)

### TowerAttackVFXController.cs

```csharp
using UnityEngine;

public class TowerAttackVFXController : MonoBehaviour
{   
    [Header("VFX Prefab Root")]
    [SerializeField] private ParticleSystem weaponVFXRoot; 

    [Header("Test Controls")]
    [SerializeField] private KeyCode fireKey = KeyCode.Space;

    private void Update()
    {
        if (Input.GetKeyDown(fireKey))
        {
            Fire();
        }
    }

    public void Fire()
    {
        if (weaponVFXRoot != null)
        {
            weaponVFXRoot.Play(true); 
        }
    }
}
```
