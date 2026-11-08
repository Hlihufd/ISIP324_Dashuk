using System.Windows;

namespace ISIP324_Dashuk.Helpers
{
    public static class MessageBoxHelper
    {
        public static bool ConfirmLoss()
        {
            var result = MessageBox.Show("Введённые данные будут потеряны. Уйти с этого шага?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}