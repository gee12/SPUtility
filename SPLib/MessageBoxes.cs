using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace SPLib
{
    public class MessageBoxes
    {
        public const string TitleError = "Ошибка";
        public const string TitleWarning = "Предупреждение!";
        public const string TitleExclamation = "Внимание!";
        public const string TitleNotify = "Информация";
        public const string TitleSuccess = "Операция выполнена успешно";
        public const string TitleNotSuccess = "Операция завершилась неудачей";
        public const string TitleServerConn = "Подключение к серверу ...";
        public const string TitleServerConnClose = "Отключение от сервера ...";
        public const string TitleSPOpenTest = "Тестирование соединения с COM-портом ...";

        public const string MsgCloseRequest = "Прекратить работу с программой?";
        public const string MsgSPOpenNotSuccess = "Порт не доступен";
        public const string MsgSPOpenSuccess = "Порт доступен!";
        public const string MsgEmptyField = "Значение <{0}> не заполнено!";
        public const string MsgCatDeleteRequestSimple = "Удалить выбранный элемент из базы данных?";
        public const string MsgErrorOperation = "Ошибка при выполнении операции:\n{0}";

        // OK dialogs
        public static short Success(Window owner, string message)
        {
            MessageBox.Show(owner, message, TitleSuccess,
                MessageBoxButton.OK, MessageBoxImage.Information);
            return ResultCodes.Success;
        }

        public static short Error(Window owner, string message)
        {
            MessageBox.Show(owner, String.Format(MsgErrorOperation, message), TitleError,
                MessageBoxButton.OK, MessageBoxImage.Error);
            return ResultCodes.Error;
        }

        public static short Error(Exception ex)
        {
            MessageBox.Show(String.Format(MsgErrorOperation, ex.Message), TitleError,
                MessageBoxButton.OK, MessageBoxImage.Error);
            return ResultCodes.Error;
        }

        public static short Warning(Window owner, string message)
        {
            MessageBox.Show(owner, message, TitleExclamation,
                MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return ResultCodes.Exclamation;
        }

        public static short Warning(string message)
        {
            MessageBox.Show(message, TitleExclamation,
                MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return ResultCodes.Exclamation;
        }

        public static short EmptyField(Window owner, object field)
        {
            MessageBox.Show(owner, String.Format(MsgEmptyField, field), TitleNotify,
                MessageBoxButton.OK, MessageBoxImage.Question);
            return ResultCodes.EmptyField;
        }

        public static void SerialPortTest(bool isConnected)
        {
            MessageBox.Show((isConnected) ? MsgSPOpenSuccess : MsgSPOpenNotSuccess, TitleSPOpenTest,
                MessageBoxButton.OK, (isConnected) ? MessageBoxImage.Information : MessageBoxImage.Warning);
        }

        // YesNo dialogs
        public static bool DeleteRequest(Window owner)
        {
            return (MessageBox.Show(owner, MsgCatDeleteRequestSimple, TitleWarning, 
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes);
        }

    }
}
