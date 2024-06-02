# <img src="WordKiller/WordKiller/Resources/Icon/256x256.ico" width="30"> WordKiller

<p align="justify"> 
WordKiller — значительно экономит время и усилия при написании студенческих работ. Эта программа автоматически форматирует документ в соответствии с заданными стандартами, оставляя пользователю только задачу написания текста.
</p>

# Содержание

1. [Главное меню](#главное-меню)
    * [Простые элементы документа](#простые-элементы-документа)
        * [Текст](#текст)
        * [Раздел](#раздел)
        * [Подраздел](#подраздел)
        * [Список](#список)
        * [Картинка](#картинка)
        * [Таблица](#таблица)
        * [Код](#код)
    * [Сложные элементы документа](#сложные-элементы-документа)
        * [Титульник](#титульник)
        * [Лист задания](#лист-задания)
        * [Список литературы](#список-литературы)
        * [Приложение](#приложение)
    * [Перемещение элементов документа](#перемещение-элементов-документа)
    * [Быстрый поиск по документу](#быстрый-поиск-по-документу)
    * [Вернее меню](#вернее-меню)
    * [Сохранение](#сохранение)
2. [Настройки](#настройки)
    * [Общие](#общие)
    * [Персонализация](#персонализация)
    * [Профиль](#профиль)
    * [Стили](#стили)
3. [Будущие нововведения](#будущие-нововведения)
4. [Известные баги](#известные-баги)

   
# Главное меню
Окно приложения разделено на две основные зоны:

* В левой части находится древовидный список объектов, добавленных в документ;
* В правой - информация о выбранном объекте.
  
Над этими зонами расположено меню с операциями над документом. 
В строке заголовка приложения отображается информация о типе документа. Если документ сохранен, дополнительно отображается путь к файлу. Если документ не сохранен, вместо пути отображается название приложения.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/c06bb0c0-a557-4f4d-b321-5d6f00d8f15f">

Для **добавления нового объекта** в документ выполните следующие шаги:

1) Нажмите кнопку "Добавить", расположенную в левом нижнем углу экрана;
2) В появившемся выпадающем списке выберите тип объекта, который хотите добавить;

Также можно **добавить объект**, используя контекстное меню:

1) Щелкните правой кнопкой мыши по древовидному списку;
2) Выбрать вставить до или после выбранного элемента древовидного дерева;
3) В открывшемся контекстном меню выберите нужный тип объекта.

Для **удаления объекта** выполните следующие шаги:

1) Щелкните правой кнопкой мыши по объекту, который хотите удалить, в древовидном списке;
2) В появившемся контекстном меню выберите пункт "Удалить".

## Простые элементы документа   

Стили для "Простых" элементов можно изменить в [настройках](#стили).

### Текст
Для удобства навигации введённый в правой части текст дублируется в подзаголовке элемента в древовидном списке.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/9e56739e-b378-442e-a47c-feb83d8d1f21">

### Раздел

В древовидном списке в него можно включить: подзаголовок, картинка, таблица, список, текст, код. В правой части вводиться название раздела (заголовка 1 уровня).

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/7a46ccd3-f46e-4a03-bedc-5c58d816add3">

### Подраздел

В древовидном списке в него можно включить: картинка, таблица, список, текст, код.  В правой части вводиться название подраздела (заголовка 2 уровня).

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/ccb26660-a1b2-4006-ac32-e3e6df93e1ea">

### Список

Каждая строка - это новый элемент списка списка, а каждый символ "!" добавляет новый уровень к элементу списка. При нажатии на кнопки "А" обычный нумерованный список (например, скопированный из какой-то статьи) конвертируется в нужный для программы формат. Название списка в итоговом отчёте не используется, оно необходимо только для облегчения идентификации в древовидном списке.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/33650758-1040-4baa-a90e-068560ec8372">

### Картинка

Нумерация картинок происходит автоматически (сквозная по всем разделам). В верхней части вводится подпись под картинкой. Для вставки картинки её нужно перенести с помощью функции drag&drop.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/492eac94-9dc1-41ac-8447-95fdf6971476">

### Таблица

Нумерация таблиц происходит автоматически (сквозная по всем разделам). В верхней части вводится название таблицы. Чтобы создать таблицу, сначала введите количество строк и столбцов, а затем заполните нужные поля данными.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/800929a6-4cb3-47df-86af-207eebf93564">

### Код

Вверху вводится название файла. Внизу вводится сам файл, который можно добавить двумя способами: 
* вручную, вводя данные;
* с помощью функции drag-and-drop.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/ecfe9e2a-5cb2-4312-87e2-0e28950eeb55">

## Сложные элементы документа

### Титульник

Для каждого типа отчёта предусмотрена разная структура, которую можно [поменять](#стили).

Существует два типа полей: поле с выпадающим списком и поле для ввода обычного текста. Над полями с выпадающим списком находится кнопка "Обновить", которая подгружает элементы из API, чтобы их можно было выбрать из выпадающего списка. Если доступ к API отсутствует или по другим причинам, можно нажать на букву "P" и ввести все поля вручную.

Также есть тип полей "Выполнили", в котором можно выбрать одного или нескольких исполнителей работы. Они будут подгружены из [профиля](#профиль).

<img width=600 height=400 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/2c46c9e5-04ab-466e-ba67-d23afde81cb9">

Титульник можно заменить на фотографию, нажав на кнопку с изображением фотоаппарата.

<img width=600 height=400 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/c6e2cacd-4218-48e5-af45-0af2dc86a701">


### Лист задания

Лист задания содержит три поля для ввода данных (остальные поля будут использованы из титульного листа).

<img width=600 height=410 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/ae9d5347-5cc8-4396-8dd9-6193c3e58df0">

Лист задания можно заменить фотографией (или двумя фотографиями, если лист задания двухсторонний), нажав на кнопку с изображением фотоаппарата.

<img width=600 height=400 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/de73d55d-cd0a-4b56-9229-fa2c68a94b58">

### Список литературы

В список литературы можно добавить либо книгу, либо электронный ресурс, так как каждый из них имеет свои поля для заполнения. 
Чтобы добавить электронный ресурс, сначала нажмите на кнопку с надписью "Электронный ресурс", а затем на кнопку с изображением плюса. После этого в таблицу добавится новая строка, которую можно будет заполнить. 

При нажатии на кнопку "А" список литературы будет отсортирован в алфавитном порядке.

<img width=600 height=400 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/bbf76a13-4d2e-4479-946e-ef440dbf1231">

Книга добавляется аналогичным образом.

<img width=600 height=400 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/4221bc46-5054-417b-b7ad-2682337f3066">

### Приложение

В приложение можно добавить три вида элементов: картинку, таблицу и код.

<img width=600 height=400 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/bc1f27e2-46c3-4980-9f8a-6c4ab1a70ebe">

## Перемещение элементов документа

Каждый элемент в древовидном списке можно перемещать. После отпускания кнопки при перетаскивании открывается окно, помогающее управлять элементами:
* "*" означает вставку в элемент (в раздел  или подраздел);
* "**" означает перемещение элементов местами;
* "↓" вставляет после выбранного элемента;
* "↑" вставляет перед выбранным элементом.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/38bf305f-760c-4964-88cf-70314af32e02">

## Быстрый поиск по документу

После нажатия клавиш Ctrl + F открывается меню поиска над древовидным списком. При вводе текста ищется ближайший к выбранному элементу объект. При нажатии кнопки со стрелкой вправо ищется следующее совпадение, а при нажатии на крестик меню поиска закрывается.

<img width=600 height=345 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/8bb9c48f-cb53-4a46-ac17-41bfc79eb780">

## Вернее меню

В верхнем меню есть четыре пункта:
* Файл;
* Тип;
* Документ;
* Справка.

### Файл 

* Экспорт - экспортирует текущий объект в формат docx;
* В формате PDF - при экспорте в docx также создаётся PDF с тем же названием;
* В формате HTML - при экспорте в docx также создаётся HTML с тем же названием;
* Создать - перед созданием нового файла будет предложено сохранить текущий документ, так как данные текущего документа будут полностью очищены;
* Открыть - перед открытием файла будет предложено сохранить текущий документ, так как данные текущего документа будут полностью заменены данными из открываемого * файла. Необходимо выбрать путь к файлу;
* Сохранить - сохраняет текущий файл (если файл не создан, то опция недоступна);
* Сохранить как... - сохраняет текущий документ как новый файл;
* Выход - закрывает приложение.

### Тип

Выбор типа документа, который влияет на оформление, титульный лист и нумерацию. 

Доступные типы:
* Обычный документ;
* Курсовые работы;
* Лабораторные работы;
* Практические работы;
* Рефераты;
* Контрольные работы;
* Практика;
* Выпускная квалификационная работа.

### Документ 

* Титульник - добавляет или удаляет титульный лист;
* Лист задания - добавляет или удаляет лист задания;
* Список литературы - добавляет или удаляет список литературы;
* Приложение - добавляет или удаляет приложение;
* Содержание - включает или выключает содержание;
* Нумерация - включает или выключает нумерацию;
* Цифры к заголовкам - включает или выключает цифры к заголовкам.

### Спрака 

* Документация - открывает этот README файл;
* О программе - показывает информацию о программе и номер версии.

## Сохранение

### Сохранение и открытие файлов

Документ сохраняется в формате программы .wkr.
При ассоциации расширения .wkr с программой, можно будет открыть файл, дважды кликнув по нему, как с документами Word.
Документы можно сохранять с шифрованием и без шифрования.
В файле сохраняются объекты древовидного списка и настройки из пункта меню "Документ".

### Изменения в файле

При изменении файла в левом верхнем углу появляется символ "\*", указывающий на то, что файл был изменён.
При попытке закрыть программу с несохранёнными изменениями (символ "\*"), появляется диалоговое окно с вопросом о необходимости сохранения изменений, продолжения работы без сохранения или отмены действия.

### Процесс сохранения

При сохранении документа символ "\*" убирается.
На его месте в течение 5 секунд отображается логотип программы, указывая на успешное сохранение.

# Настройки

## Общие

В этом разделе собраны механические настройки приложения. Здесь можно:
* Ассоциировать расширение с программой: позволяет открывать файлы с расширением .wkr через это приложение при двойном клике;
* Автосохранение: включает автоматическое сохранение документа каждые 5 минут;
* Проверка синтаксиса: включает автоматическую проверку синтаксиса;
* Шифрование: включает шифрование файлов;
* Закрытие приложения после экспорта: автоматически закрывает приложение после экспорта документа в формат docx;
* Автозаполнение заголовка объекта: при добавлении нового объекта в древовидный список заголовок будет автоматически заполняться типом объекта.
Все настройки приложения хранятся в специальном файле, который можно открыть, удалить или полностью удалить всю иерархию папок, связанных с настройками.

<img width=600 height=600 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/8f022d84-8f2f-4141-8dd5-ace505f3f425">

## Персонализация

В этом разделе можно настроить приложение под свой вкус. Доступные опции:

* Цветовая схема: приложение состоит из трех основных цветов, которые можно изменить.Также можно настроить цвет наведения на объект и цвет нажатия на объект;
* Размер шрифта элементов интерфейса: возможность изменить размер шрифта для всех элементов интерфейса;
* Шрифт полей для ввода текста: возможность изменить шрифт для всех полей ввода текста;
* Язык приложения: возможность выбрать язык интерфейса. Доступные языки:
    * Русский;
    * Белорусский;
    * Английский;
    * Немецкий;
    * Французский;
    * Китайский.

<img width=600 height=700 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/28c2d0ea-fad4-415c-b1e6-694813e06b6b">

## Профиль

Настройки профиля предназначены для автоматического подставления одинаковой информации, которая встречаеться в отчётах.

<img width=600 height=450 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/4fda3be5-db59-49ba-93ca-5c398c02b080">

Также имеется таблица, где можно заполнить данные людей, которые будут пользоваться этим приложением. Это позволит легко выбирать, кто выполнял отчёт. В таблице можно указать:
* Фамилия;
* Имя;
* Отчество;
* Номер зачётной книжки (шифр);
* Автовыполняющий - будет выбран по умолчанию в качестве человека, выполняющего работу.

<img width=600 height=300 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/c7ddbbce-df9f-4f8e-846f-3f528d6db3fe">

## Стили

В этом разделе можно задать форматирование для следующих элементов:

* Текст;
* Раздел;
* Подраздел;
* Список;
* Изображение и его подпись;
* Подписи к таблицам (ТекстКТаблице);
* Таблица;
* Код.

Доступные параметры форматирования включают:

* Размер шрифта;
* Выравнивание;
* Полужирный текст;
* Отступ сверху абзаца;
* Отступ снизу абзаца;
* Межстрочный интервал;
* Отступ слева для абзаца;
* Отступ справа для абзаца;
* Отступ первой строки абзаца.

Также можно задать, с какой страницы начнется нумерация основного текста (после титульного листа/листа задания/содержания).

<img width="600" height="400" src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/022f6e20-60db-4321-8600-5e6c3d7f6f8b">

Есть возможность заменить титульную страницу. Для этого выполните следующие шаги:

1) Нажмите кнопку "Заменить титульник";
2) Выберите файл формата .docx, состоящий из одной страницы, где поля для изменений отмечены желтым.

Пример документа:

<img width="600" height="800" src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/ef28f5fc-7d0a-4783-8aa6-d7bcf64f239e">

После выбора документа программа проанализирует строки, выделенные желтым, и предложит выбрать, какие из них соответствуют пулу полей приложения.

<img width=600 height=800 src="https://github.com/GREBIAR-Git/WordKiller/assets/74742355/0bff717a-cd57-4d17-8ead-7087df5e2dd8">

# Будущие нововведения
* Переделать (улучшить) интерфейс;
* Возможность одновременной работы с документом с 2 или более устройств;
* Найти способ создавать PDF и HTML без установленного приложения WORD;
* Добавить возможность добавления новых типов документа;
* Добавить новые возможности взаимодействия с таблицей;
* Улучшить меню Ctrl+F;
* Добавить формулы;
* Добавить проверку орфографии одновременно на нескольких языках;
* Доработать создание производственной практики и ВКР;
* Добавить возможность удаления сразу нескольких элементов;
* Добавить картинки вместо символов в drag&drop treeview.

# Известные баги
1. Формирование содержания:
    * При первом открытии файла ненужный вопрос;
    * Внутри содержания не тот стиль.
2. Не обновляется язык в заголовках GridView;
3. Картинка не подстраиватеься под размер конца страницы;
4. Таблицане не правильно переноситься на следующую страницу (оформление);
5. Если подзаголовок начинается с новой странцы ставить \n.
