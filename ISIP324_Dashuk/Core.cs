using System;

namespace ISIP324_Dashuk
{
    internal static class Core
    {
        private static MarketplaceDatabaseEntities1 context = new MarketplaceDatabaseEntities1();

        // Контекст создаётся при первом обращении. Во всех файлах по-прежнему Core.Context.
        public static MarketplaceDatabaseEntities1 Context
        {
            get
            {
                if (context == null) context = new MarketplaceDatabaseEntities1();
                return context;
            }
        }

        // Проверка подключения к базе. Вызывается один раз при запуске программы.
        public static bool CheckConnection()
        {
            try
            {
                if (Context.Database.Exists()) return true;
                Console.WriteLine("База данных MarketplaceDatabase не найдена. Запустите SQL-скрипт.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось подключиться к базе: " + (ex.InnerException ?? ex).Message);
            }
            return false;
        }

        // Сохраняет изменения. При ошибке выводит сообщение и сбрасывает контекст,
        // чтобы неудавшиеся изменения не остались в нём.
        public static bool TrySave()
        {
            try
            {
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при сохранении: " + (ex.InnerException ?? ex).Message);
                context = null; // при следующем обращении создастся новый контекст
                return false;
            }
        }
    }
}