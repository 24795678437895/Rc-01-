# RC-01 — бепул булутда Android APK йиғиш

## Лойиҳа ҳолати
- Unity версияси: 2022.3.62f1 LTS.
- Асосий demo саҳнаси архивга киритилди: `Assets/Scenes/RC01_FlightCombat.unity`.
- Саҳнада `RC01GameBootstrap` автоматик ишга тушади.
- Android build вақтида `RC01CloudBuildSettings` ARM64, IL2CPP, landscape ва package ID ни автоматик ўрнатади.

## Unity Build Automation орқали
1. `https://cloud.unity.com/` орқали Unity ID билан киринг.
2. DevOps / Build Automation ни очинг ва бепул режани ишга туширинг.
3. Source Control бўлимида GitHub ёки Unity Version Control ни уланг.
4. RC-01 лойиҳаси репозиторийсини танланг.
5. Build Automation > Configurations > Quick target setup ни очинг.
6. Platform: Android; Unity version: 2022.3.62f1 (ёки шу тармоқдаги мавжуд 2022.3 LTS); architecture: ARM64.
7. Бепул Windows Micro машинасини танланг ва build ни бошланг.
8. Build history да натижа ва APK артефактини олинг.

## Тўлов хавфсизлиги
- Бепул лимит: ҳар ой 200 Windows Minutes (Micro), 100 Mac Minutes ва 100 Linux Minutes; аниқ қолдиқни Unity Dashboard да текширинг.
- Лимитдан ошганда хизмат тўхтайди; давом эттириш учун тўлов усули қўшиш талаб қилинади. Тўлов картасини киритманг.
- Build Automation Android ни қўллайди. Unity 2022.3 LTS Build Automation да қўллаб-қувватланади.

## Эслатма
Бу архив ҳали Unity Editor да компиляция ёки Android қурилмада синовдан ўтказилмаган. Булутдаги биринчи build натижаси хатоларни аниқлайди; APK тайёр деб фақат муваффақиятли build дан кейин айтиш мумкин.
