using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.Extensions;
using AllOverIt.Reactive;

namespace AESEncryptionDemo
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly AesEncryptor _encryptor;

        private string? _key;
        public string? Key
        {
            get => _key;
            private set => RaiseAndSetIfChanged(ref _key, value);
        }

        private string? _iv;
        public string? IV
        {
            get => _iv;
            private set => RaiseAndSetIfChanged(ref _iv, value);
        }

        private string? _textInput;
        public string? TextInput
        {
            get => _textInput;
            set => RaiseAndSetIfChanged(ref _textInput, value, null, OnTextInputChanged);
        }

        private string? _textEncrypted;
        public string? TextEncrypted
        {
            get => _textEncrypted;
            private set => RaiseAndSetIfChanged(ref _textEncrypted, value);
        }

        private string? _textDecrypted;
        public string? TextDecrypted
        {
            get => _textDecrypted;
            private set => RaiseAndSetIfChanged(ref _textDecrypted, value);
        }

        public MainWindowViewModel()
        {
            _encryptor = new AesEncryptor();

            Key = Convert.ToBase64String(_encryptor.Configuration.Key);
            IV = Convert.ToBase64String(_encryptor.Configuration.IV);
            TextInput = $"Enter some text here to see it encrypted";
        }

        private void OnTextInputChanged()
        {
            TextEncrypted = _encryptor.EncryptPlainTextToBase64(TextInput);
            TextDecrypted = _encryptor.DecryptBase64ToPlainText(TextEncrypted);
        }
    }
}
