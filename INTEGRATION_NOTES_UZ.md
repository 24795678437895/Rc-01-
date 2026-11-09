# RC-01 integratsiya qaydlari

## Bog'lanish zanjiri
1. `RC01Missile.OnTriggerEnter/OnCollisionEnter` -> `RC01TargetHealth.ApplyDamage(hitDamage)`.
2. HP 0 -> `RC01TargetHealth.DestroyTarget()`.
3. DestroyTarget -> colliderlarni o'chirish, ixtiyoriy portlash prefab, `RC01MissionManager.TargetDestroyed(target)`, keyin obyektni kechiktirib o'chirish.
4. MissionManager -> qolgan nishonlar soni va score yangilanadi.
5. Radar va Targeting -> `IsDestroyed` bo'lgan obyektlarni e'tiborsiz qoldiradi.

## Sinov ro'yxati (Unity Editor)
- [ ] `RC-01 > Build Demo Scene` orqali sahna yarating.
- [ ] Play: HUD `TARGETS LEFT: 3`, `SCORE: 0` ko'rsatishini tekshiring.
- [ ] Nishonni qulflab raketa oting; birinchi zarbada HP 50 ga tushadi (standart damage 50).
- [ ] Ikkinchi zarbada nishon yo'qoladi, score +100, qolgan nishonlar 2 bo'ladi.
- [ ] Radar kontaktlar soni bittaga kamayadi.
- [ ] Uchala nishon yo'q қилинганда `MISSION COMPLETE` chiqadi.
- [ ] Android Landscape qurilmada touch tugmalarini tekshiring.

Eslatma: `RC01Missile.hitDamage` va `RC01TargetHealth.maxHealth/scoreValue` Inspector orqali o'zgartirilishi mumkin.


## 100 km radar yangilanishi
- `RC01Radar` endi `OverlapSphereNonAlloc` dan foydalanadi; har yangilanishda ortiqcha massiv ajratmaydi.
- Bir nishonda bir nechta collider bo'lsa ham, u bir marta sanaladi.
- Eng yaqin nishon masofasi va samolyotga nisbatan taxminiy bearing (daraja) ko'rsatiladi.
- `BUFFER FULL` chiqsa, Inspector'da `bufferCapacity` qiymatini oshiring.
- Masofa Unity world unit bilan hisoblanadi; loyiha masshtabida 1 unit = 1 metr deb olinadi.
- Bu o'yin ichidagi soddalashtirilgan sensor; haqiqiy radar aniqligi yoki 100 km real sensor ishlashini anglatmaydi.
