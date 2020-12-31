﻿# SoNoS
Ваш помощник на трансляциях на Twitch.

## Установка

1. Установите [.Net Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
2. Скачиваем [последную версию](https://github.com/aleksjjj11/SonosTwitch/releases/latest)
3. Содержание архива поместите в любое удобное для вас место

## Первый запуск

* Запускаем ***SonosTwitch.exe***
* Видим такое окно (ниже), не боимся и нажимаем ОК. <br>
![Страшное окно](https://avatars.mds.yandex.net/get-images-cbir/2359610/AC-vrJlNFeZRL-3FDCxFWQ/ocr)
* Видим главное окно приложение. <br>
![](https://avatars.mds.yandex.net/get-images-cbir/2958134/lU1n3VW5G8TiugHSvvoo9Q/ocr)
* Дальше первым делом нажимаем на шестерню (сверху слева).
* Видим окно, где вводим данные, чтобы программа могла читать ваш чат Twitch. <br>
![](https://avatars.mds.yandex.net/get-images-cbir/3575410/OBQ2RuWSLXPwdj-IXkXO7w/ocr)
* Токен можно получить кликнув на слово ***Token?!***.
* После авторизации там, вводим **ACCESSES TOKEN** в поле для токена.
* В поле ***Login*** вводим свой **Twitch login**.
* Далее нажимаем кнопку **Login**, а после **Exit**.
* Возле шестерни должен появится ваш логин.

## Кратко о главном

### Prefix
Использовать односимвольный префикс для корректной работы. 
(В будущем  может появится поддержка многосимвольных префиксов)

### Раздел GROUPS
* **Everyone** - команды будут приниматься от всех пользователей на трансляции.
* **Subscriber** - команды будут приниматься от подписчиков.
* **Follower** - команды будут приниматься от фоловеров.  

### Раздел ADVANCED SETTING
* **GetOffer** -  звуки на командах не будут воспроизводится автоматически, а будут собираться 
влевом блоке приложения, где каждый звук можно будет принять (воспроизведётся), либо
отклонить.
* **Timeout** - задержка в милисекундах между принимаемыми сообщениями с чата трансляции.
* **Text to Speech** - команда для чтения сообщений пользователей с чата трансляции. 
Пример: ***!speech Hi dudes***. ***"Hi dudes"*** будет прочитано. В поле можно вводить любую команду, 
для данной функции. 

### Центральная часть
По центру снизу есть две кнопки:
* Add command - добавляет новую строку для команды и звук, который будет воспроизводится по ней.
* Reset all commands - удаляет все строки со звуками и командами.

### Под конец

Для применения изменений в командах и звуках нажимайте на ***зелёную*** кнопку)
Для звуков на данный момент принимаются только .wav файлы.

Вскоре ждём поддержку большего количества форматов.