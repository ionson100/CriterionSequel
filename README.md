CriterionSequel
===============
Продолжение Criterion, скорее финальная часть, если первый был написан ради шутки,<br/>
то в этом проекте уже заложена реализация для вставки в промышленный код.<br/>
Исходим из задач:<br/>
Возможность комбинировать фильтры поиска по шаблону как из HTML разметки, так и в  RAZOR<br/>
Возможность наследование разметки.<br/>
Минимизация серверных скриптов, минимизация настроек в атрибутах и настроек при запуске.<br/>
Легкое создание своих собственных фильтров.<br/>
Получение финального  выражения для поиска в перечислениях,  так и для поиска через<br/> 
sql через  орм, которые поддерживают ```Linq to Sql```.<br/>
Визуализация на откуп дизайнеру.<br/>
Все поля, которые участвуют в фильтре, помечаются атрибутами.<br/>
Все атрибуты имеют наследника ```CriterionBaseAttribute```,   поддерживается локализация,<br/>
стоит остановиться на свойстве id, в клиента уходит в нижнем регистре, во избежание<br/>
коллизий, свойство можно инициализировать  своей строкой, по умолчанию название<br/>
свойства  типа.<br/>
Шаблон HTML, все включения маркируется в виде строки #имя свойства или ID если оно переопределено#<br/>
если вы работаете с шаблонами html то вы должны указать путь к шаблону<br/>
в атрибуте типа ```CriterionTemplateAttribute```<br/>
если вы работаете с указанием шаблона в запросе – можно не указыватьразметку  можно вызыавть тремя способами.<br/>
1 ```@(Html.CriterionHtmlTemplate<T>())```<br/>
2 ```@(Html.CriterionHtmlTemplate<T>("~/TemplateCriterion/Body2.html"))```<br/>
  и вызов частичных фильтров ( шаблоны не участвуют)<br/>
3 ```@(Html.Criterion<Body>(body => body.MadeIn))```<br/>
Заполнение опций  форм, вы должны реализовать своего поставщика этих опций,<br/>
реализовать тип  ```IListItem```<br/>
если свойство поддерживает null и если надо учитывать эту позицию в поиске<br/>
 – в строке пишем “null”, если поставить пустую строку,  позиция выбора с пустой строкой <br/>
не участвует в фильтре поиска.<br/>
В слйдере участвуют два значения мин и мах, вы должны так же организовать<br/>
поставщика для них, и передать тип через атрибут, все поставщики должны иметь конструктор по умолчании.<br/>
Получение результирующего выражения для поиска на сервере:<br/>
для ajax или Get запросов<br/>
```CriterionActivator.GetExpression<Т>(FormCollection collection);```<br/>
для обычных Post через контроллер<br/>
``CriterionActivator.GetExpression<Т>();``<br/>
Вшитый показ справки, справка отображается, если вы укажете поставщика<br/>
для справки, тип реализующий ITypeHelp,<br/>
Указывать тип нужно при старте приложения<br/>
``CriterionActivator.SetTypeHelp(typeof(Helper));``<br/>
Иконка вшита по умолчанию, вы можете назначить свою иконку<br/>
при старте приложения.<br/>
``CriterionActivator.SetUrlImageHelp(string url)``<br/>
что вынесено наружу:<br/>
инициализация календаря для атрибута  выбора  по диапазону дат<br/>
  ``$(".betweendate").datepicker({ dateFormat: "yy-mm-dd" });``<br/>
инициализация окна справки<br/>
``$("#dialog").dialog({ autoOpen: false, modal: true, title: "Справка:", height: 400, width: 500 });``<br/>
все это есть в примере.<br/>
Из сторонних библиотек используется:<br/>
Yahoo.Yui.Compressor – для сжатия скрипта при шаблонном показе<br/>
Dynamic – для создания Expressions<br/>
<br/>
