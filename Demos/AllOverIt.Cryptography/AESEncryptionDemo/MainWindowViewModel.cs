using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.Extensions;
using AllOverIt.Reactive;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AESEncryptionDemo
{
    internal sealed class PropertyNotInitializedException : Exception
    {
        private const string MessageTemplate = "The property {0} has not been set.";

        public PropertyNotInitializedException([CallerMemberName] string propertyName = "")
            : base(string.Format(MessageTemplate, propertyName))
        {
        }
    }

    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly AesEncryptor _encryptor;

        private string? _key;
        private string? _iv;
        private string? _textInput;
        private string? _textEncrypted;
        private string? _textDecrypted;

        public string Key
        {
            get => _key ?? throw new PropertyNotInitializedException();
            private set => RaiseAndSetIfChanged(ref _key, value);
        }

        public string IV
        {
            get => _iv ?? throw new PropertyNotInitializedException();
            private set => RaiseAndSetIfChanged(ref _iv, value);
        }

        public string TextInput
        {
            get => _textInput ?? throw new PropertyNotInitializedException();
            set => RaiseAndSetIfChanged(ref _textInput, value, null, OnTextInputChanged);
        }

        public string TextEncrypted
        {
            get => _textEncrypted ?? throw new PropertyNotInitializedException();
            private set => RaiseAndSetIfChanged(ref _textEncrypted, value);
        }

        public string TextDecrypted
        {
            get => _textDecrypted ?? throw new PropertyNotInitializedException();
            private set => RaiseAndSetIfChanged(ref _textDecrypted, value);
        }

        [SetsRequiredMembers()]
        public MainWindowViewModel()
        {
            _encryptor = new AesEncryptor();

            Key = Convert.ToBase64String(_encryptor.Configuration.Key);
            IV = Convert.ToBase64String(_encryptor.Configuration.IV);
            TextInput = $"Enter some text here to see it encrypted";
        }

        [MemberNotNull(nameof(TextEncrypted), nameof(TextDecrypted))]
        private void OnTextInputChanged()
        {
            TextEncrypted = _encryptor.EncryptPlainTextToBase64(TextInput);
            TextDecrypted = _encryptor.DecryptBase64ToPlainText(TextEncrypted);
        }
    }
}
