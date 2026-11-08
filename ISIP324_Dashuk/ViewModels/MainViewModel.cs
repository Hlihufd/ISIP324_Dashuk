using System;
using System.Windows;
using ISIP324_Dashuk.Helpers;
using ISIP324_Dashuk.Models;
using ISIP324_Dashuk.ViewModels.Base;

namespace ISIP324_Dashuk.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public CarConfiguration Config { get; } = new CarConfiguration();

        public int Step { get; private set; } = 1;
        public ViewModelBase CurrentViewModel { get; private set; }
        public string StepText => $"Шаг {Step} из 5";
        public string NextText => Step == 5 ? "Оформить заявку" : "Далее";

        public RelayCommand NextCommand { get; }
        public RelayCommand BackCommand { get; }

        public MainViewModel()
        {
            NextCommand = new RelayCommand(Next, CanNext);
            BackCommand = new RelayCommand(Back, () => Step > 1);
            ShowStep();
        }

        private bool CanNext()
        {
            var step5 = CurrentViewModel as Step5ViewModel;
            return step5 == null || step5.IsValid;
        }

        private void Next()
        {
            if (Step == 5)
            {
                MessageBox.Show("Заявка отправлена! Мы свяжемся с вами по телефону.");
                Application.Current.Shutdown();
                return;
            }
            Step++;
            ShowStep();
        }

        private void Back()
        {
            if (Step == 5 && !((Step5ViewModel)CurrentViewModel).IsValid)
            {
                if (!MessageBoxHelper.ConfirmLoss()) return;
                Config.Contact = new ContactInfo();
            }
            Step--;
            ShowStep();
        }

        private void ShowStep()
        {
            switch (Step)
            {
                case 1: CurrentViewModel = new Step1ViewModel(Config); break;
                case 2: CurrentViewModel = new Step2ViewModel(Config); break;
                case 3: CurrentViewModel = new Step3ViewModel(Config); break;
                case 4: CurrentViewModel = new Step4ViewModel(Config); break;
                case 5: CurrentViewModel = new Step5ViewModel(Config); break;
            }
            OnPropertyChanged("");
        }
    }
}