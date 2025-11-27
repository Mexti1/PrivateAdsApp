Проект: Djabarov_Mekhty_4ISIP-422 (минимальная работа-скелет WPF)
Visual Studio 2022

Что внутри архива:
- Папка Djabarov_Mekhty_4ISIP_422/ - WPF-проекта (исходники .xaml и .cs)
- SQL/Create_Djabarov_DB.sql - SQL-скрипт для создания базы данных и таблиц
- Properties/Settings.settings - файл с ConnectionString (по умолчанию использует LocalDB)
- Djabarov_Mekhty_4ISIP_422.csproj - проектный файл

ВАЖНО (инструкция для запуска):
1) Откройте SQL Server Management Studio (SSMS) и выполните файл SQL/Create_Djabarov_DB.sql
   - Скрипт создаст базу данных Djabarov_DB_Payment и таблицы.
   - Рекомендуется регистрировать пользователей через приложение (чтобы пароли хранились в SHA1-хеше).

2) Откройте Visual Studio 2022:
   - Создайте новую пустую WPF (.NET Framework) проект с TargetFramework .NET Framework 4.7.2
   - Закройте VS.
   - Скопируйте содержимое папки Djabarov_Mekhty_4ISIP_422 в каталог проекта, перезаписав файлы.
   - Или создайте проект под именем Djabarov_Mekhty_4ISIP_422 и добавьте файлы вручную.

3) В файле Properties/Settings.settings (или через Project -> Properties -> Settings) проверьте ConnectionString:
   - По умолчанию установлен: Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Djabarov_DB_Payment;Integrated Security=True
   - Если у вас другой SQL Server, замените Data Source и/или используйте SQL авторизацию.

4) Соберите проект и запустите.
   - На стартовой странице нажмите "Регистрация" и создайте нового пользователя (роль Admin или User).
   - Затем войдите с новым пользователем.

Если хочешь, я могу дополнить проект:
- Сделать полноценные CRUD-страницы (Users/Category/Payment),
- Добавить диаграммы и экспорт в Word/Excel,
- Подготовить .sln чтобы открывалось прямо.

Напиши, какие дополнительные части надо включить сейчас.
