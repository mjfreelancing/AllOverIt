using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Reactive;

namespace RSAEncryptionDemo
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly IRsaEncryptor _encryptor;

        private string _publicKey;
        public string PublicKey
        {
            get => _publicKey;
            private set => RaiseAndSetIfChanged(ref _publicKey, value);
        }

        private string _privateKey;
        public string PrivateKey
        {
            get => _privateKey;
            private set => RaiseAndSetIfChanged(ref _privateKey, value);
        }

        private string _textInput;
        public string TextInput
        {
            get => _textInput;
            set => RaiseAndSetIfChanged(ref _textInput, value, null, OnTextInputChanged);
        }

        private int _maxInputLength;
        public int MaxInputLength
        {
            get => _maxInputLength;
            private set => RaiseAndSetIfChanged(ref _maxInputLength, value);
        }

        private string _textEncrypted;
        public string TextEncrypted
        {
            get => _textEncrypted;
            private set => RaiseAndSetIfChanged(ref _textEncrypted, value);
        }

        private string _textDecrypted;
        public string TextDecrypted
        {
            get => _textDecrypted;
            private set => RaiseAndSetIfChanged(ref _textDecrypted, value);
        }

        public MainWindowViewModel()
        {
            // Creates a new public/private key pair with 128-bit security
            var rsaKeyPair = RsaKeyPair.Create();

            _encryptor = RsaEncryptor.Create(rsaKeyPair);

            PublicKey = rsaKeyPair.GetPublicKeyAsBase64();
            PrivateKey = rsaKeyPair.GetPrivateKeyAsBase64();
            MaxInputLength = _encryptor.GetMaxInputLength();
            TextInput = $"Enter some text here to see it encrypted (max length {MaxInputLength} bytes)";
        }

        private void OnTextInputChanged()
        {
            TextEncrypted = _encryptor.EncryptPlainTextToBase64(TextInput);
            TextDecrypted = _encryptor.DecryptBase64ToPlainText(TextEncrypted);
        }
    }
}
