# RC-01 Flight Combat — Android Unity prototipi

**Holati: Alpha manba loyiha. APK emas.** Unity Editor ichida hali kompilyatsiya va haqiqiy telefonda sinovdan o'tkazilmagan.

## Nimalar bor
- F-35 uslubidan ilhomlangan, primitiv detallardan yig'iladigan sinov samolyoti (haqiqiy F-35 modeli emas).
- Rigidbody asosidagi sodda arcade parvoz boshqaruvi.
- Sensorli HUD tugmalari: pitch, roll, throttle, kamera, nishonni qulflash va raketa otish.
- Kokpit va tashqi kamera rejimlari.
- 3 ta oddiy dushman nishoni, raketa va topshiriq hisoblagichi.
- 100 000 metr (100 km) radiusli soddalashtirilgan radar masofa tekshiruvi. Bu haqiqiy harbiy radar simulyatsiyasi emas.

## Unity versiyasi
Unity **2022.3.62f1** (LTS) bilan ochish uchun mo'ljallangan.

## Unity Editor'da tayyorlash
1. ZIP'ni kompyuter yoki bulutli Windows ish muhitiga chiqaring.
2. Unity Hub orqali 2022.3.62f1 va Android Build Support (SDK/NDK, OpenJDK) o'rnating.
3. Loyihani Unity Hub'dan oching va import/kompilyatsiya tugashini kuting.
4. Menyudan `RC-01 > Build Demo Scene` ni bosing.
5. `RC-01 > Prepare Android Settings` ni bosing.
6. `RC-01 > Build Android APK` ni bosing. APK `Builds/Android/RC01_FlightCombat.apk` ichiga yoziladi.

## Android sozlamalari
- Landscape (gorizontal) ekran.
- ARM64 arxitektura.
- Minimum Android API 26.
- IL2CPP scripting backend.
- Ilova ID: `com.rc01.flightcombat`.

## Muhim cheklovlar
- Ushbu muhitda Unity Editor mavjud emas; shu sababli C# kompиляцияси, sahna ишга тушиши, FPS ва APK build тасдиқланмаган.
- Samolyot hozircha Unity primitive shakllaridan tuziladi; realistik tashqi model, animatsiyalar, tovush, portlash effektlari va to'liq kampaniya keyingi bosqichda kerak.
- 100 km radar hozircha masofa bo'yicha oddiy aniqlash; xarita, balandlik filtri va real radar displeyi yo'q.
- Android telefonda Unity loyihasini to'liq build qilish odatda Unity Editor o'rnatilgan kompyuter yoki bulutli Windows mashinasini talab qiladi.

## Nishon–raketa–radar–missiya integratsiyasi (v3)
- `RC01Missile` urilganda `RC01TargetHealth.ApplyDamage()` ni chaqiradi; trigger/collision bir kadrda ikki marta zarar bermasligi uchun impact himoyasi bor.
- `RC01TargetHealth` HP ni kamaytiradi, HP 0 bo'lganda colliderlarni o'chiradi, ixtiyoriy `explosionPrefab` yaratadi va missiya hisoblagichiga xabar beradi.
- `RC01MissionManager` har bir nishonni bir marta hisoblaydi va yo'q qilingan nishon uchun `scoreValue` ball qo'shadi.
- `RC01Radar` yo'q qilingan nishonlarni kontaktlar soniga qo'shmaydi.
- `RC01Targeting` faqat tirik `RC01TargetHealth` komponenti bor nishonlarni qulflaydi; bootstrap uni avtomatik ulaydi.
- Boshlang'ich sahnada uchta dushman nishoni ro'yxatdan o'tkaziladi; HUD'da qolgan nishonlar va ball ko'rsatiladi.

### Inspector sozlamasi
Har bir dushman obyektida `RC01TargetHealth` komponenti bo'ladi. `Max Health`/`Health` (standart 100), `Score Value` (standart 100) ni sozlang. Portlash ko'rinishi kerak bo'lsa, `Explosion Prefab` maydoniga particle prefab biriktiring. Prefab berilmasa, HP/score/radar/mission oqimi baribir ishlaydi.

### Tekshiruv holati
Manba kodlar o'zaro bog'landi va matn darajasida statik tekshiruv qilindi. Ushbu ish muhitida Unity Editor/Android SDK yo'q, shu sabab Unity kompиляцияси, sahna play-test ва APK build bajarilgan deb da'vo qilinmaydi. Unity 2022.3.62f1 da ochib, Console xatolarini tekshirish kerak.


## 2026-11-02 integratsiya tekshiruvi
- `com.unity.ugui` paketi qo'shildi: HUD, Button, Text va EventSystem uchun zarur.
- Mobil boshqaruv tugmalari PointerDown/PointerUp orqali bosib turilganda ishlaydi.
- Pitch boshqaruvi yo'nalishi tuzatildi: PITCH UP endi samolyot burnini yuqoriga ko'taradi.
- Unity Editor ichida `RC-01/Build Demo Scene`, so'ng `RC-01/Build Android APK` menyularini bajaring.
- Unity kompilyatsiyasi ushbu muhitda ishga tushirilmagan; APK tayyor deb hisoblanmaydi.

Batafsil yig'ish holati va APK olish bosqichlari: `BUILD_STATUS_UZ.md`.

## v3 интеграция
- Нишон қулфлаш сканери `OverlapSphereNonAlloc` билан ишлайди ва бир нишоннинг бир нечта collider'ини такрор ҳисобламайди.
- Ракета отиш тугмаси `RC01CombatSystem` орқали ишлайди: 6 та ракета ва 1.2 сониялик қайта отиш оралиғи.
- Лойиҳа ҳолати ва Unity синови чекловлари `BUILD_STATUS_UZ.md` да кўрсатилган.


## v4 — жанг тизими барқарорлиги
- Ракета сони launcher орқали ягона ҳисобланади.
- CombatSystem фақат қайта отиш вақтини назорат қилади.
- Йўқ қилинган нишон target lock дан автоматик чиқарилади.
- HUD ракета қолдиғини launcher дан олади.
- Android учун 60 FPS мақсади ва VSync ўчирилиши қўшилди.


## v6 — Android экран мослашуви
- HUD телефоннинг safe area чегарасини ҳисобга олади.
- Экран ландшафт режимида ишлайди.
- CanvasScaler кенглик ва баландликка тенг мослашувдан фойдаланади.

## v8 — парвоз ва жанг яхшиланиши
- Кўтариш кучи ва ҳаво қаршилиги қайта созланди.
- Радарда диапазон ёзуви Inspector'даги ҳақиқий `rangeMetres` қийматидан чиқарилади.
- Душман самолётлари ўйинчига даврий бошқариладиган ракета отади.
- Ўйинчи самолётига 100 HP корпус саломатлиги ва HUD кўрсаткичи қўшилди.
- Pitch ва roll сенсор тугмалари бир нечта бармоқ билан бир вақтда босилишни мустақил ҳисоблайди.
- Батафсил текширув чекловлари `BUILD_STATUS_UZ.md` да.


## v9.1 AESA — 2026-11-03 тайёрлаш
- AESA олд сектор треклари `[AESA]`, 360° Sensor Fusion орқали кўрилган бошқа треклар `[FUSION]` деб кўрсатилади.
- Бу ўйин симуляцияси; ҳақиқий ҳарбий радар модели эмас.
- Build ҳолати ва чекловлар учун `RC01_V9_1_REVIEW_REPORT_UZ.md` ни ўқинг. Архив ҳали APK эмас.
