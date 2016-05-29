*Итоговый отчет*
**Итоги работы КТ3**
- Сделан мультиплеер с использованием сокетов и UDP(issues #6, #7) 
- Улучшена графическая составляющая(issue #5)
- Смоделирована сила притяжения(issue #8)
- Реализована система достижений(issue #10)

План и задачи были описаны в "Milestones". Для КТ № 3 - milestone "Третья контрольная точка".
Все задачи, описанные там были выполнены

**Итоги всей работы**
Задачи и план работы был описан в 3 milestones:
- Первая контрольная точка (выполнен)
- Вторая контрольная точка (выполнен)
- Третья контрольная точка (выполнен)
(Закрытые milestones)[https://github.com/rustammm/FoCS-AMI-Project/milestones?state=closed]

Результатом работы стала игра, написанная с помощью XNA framework(.NET 4.0). Геймплей напоминает игры Osmos и Agar.io.
С помощью реактивной тяги игрок передвигается по карте(при клике мыши сфера перемещается в обратном относитьльно мыши направлении).
Для обработки кликов мыши и пересечения объектов был написан обработчик событй(OsmosEventHandler), находящийся в библиотеки OsmosLibrary.
При столкновении с другим объектов, больший объект поглащает меньший(до тех пор пока они не перестанут пересекаться).

В игре доступен многопользовательский режим. Для того, чтобы подключиться к другому надо:
- в файле server.info прописать в первой строчке локальный адрес сервера
чтобы стать сервером или играть в однопользовательскую:
- в первой строчке написать "server"
- во второй кол-во ботов

В игре также присутствует физика(а именно, сила притяжения) и система достижений, который сейчас 5
- стать больше 70 единиц размера
- стать больше 100 единиц размера
- поглотить 1 сферу
- поглотить 3 сферы
- поглотить 10 сфер

Решение состоит из 3 проктов
- главный проект
- библиотека для игры(без многопользовательского режима)
- дополнение библиотеки многопользовательским режимом

По плану и задачам описанным в Milestones выполнены все пункты
- Первая контрольная точка - создан репозиторй
- (Вторая контрольная точка)[https://github.com/rustammm/FoCS-AMI-Project/issues?q=milestone%3A%22%D0%92%D1%82%D0%BE%D1%80%D0%B0%D1%8F+%D0%BA%D0%BE%D0%BD%D1%82%D1%80%D0%BE%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F+%D1%82%D0%BE%D1%87%D0%BA%D0%B0%22]
- (Третья контрольная точка)[https://github.com/rustammm/FoCS-AMI-Project/issues?q=milestone%3A%22%D0%A2%D1%80%D0%B5%D1%82%D1%8C%D1%8F+%D0%BA%D0%BE%D0%BD%D1%82%D1%80%D0%BE%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F+%D1%82%D0%BE%D1%87%D0%BA%D0%B0%22]
