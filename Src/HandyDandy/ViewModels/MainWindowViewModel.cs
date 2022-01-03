// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Asymmetric.KeyPairs;
using Autarkysoft.Bitcoin.Encoders;
using Autarkysoft.Bitcoin.ImprovementProposals;
using Avalonia;
using HandyDandy.Models;
using HandyDandy.MVVM;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace HandyDandy.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            InputTypeList = EnumHelper.GetAllEnumValues<InputType>().ToArray();
            _selIn = InputTypeList[0];

            OutputTypeList = EnumHelper.GetAllEnumValues<OutputType>().ToArray();
            _selOut = OutputTypeList[0];

            MnemonicLengthList = EnumHelper.GetDescriptiveEnums<MnemonicLength>().ToArray();
            _selMnLen = MnemonicLengthList.First();

            // This has to be called last
            BuildNewGenerator();
        }


        public static bool IsDebug =>
#if DEBUG
            true;
#else
            false;
#endif

        public void Fill(bool b)
        {
            for (int i = 0; i < Generator.Stream.DataBitSize; i++)
            {
                Generator.Stream.Items[i].SetState(b);
            }
        }

        public static string Title
        {
            get
            {
                Version? ver = Assembly.GetExecutingAssembly().GetName().Version;
                return $"Handy Dandy {ver?.ToString(4)} {(ver?.Major == 0 ? "(beta)" : "")}";
            }
        }

        public InputType[] InputTypeList { get; }

        private InputType _selIn;
        public InputType SelectedInputType
        {
            get => _selIn;
            set
            {
                if (SetField(ref _selIn, value))
                {
                    BuildNewGenerator();
                }
            }
        }

        public OutputType[] OutputTypeList { get; }

        private OutputType _selOut;
        public OutputType SelectedOutputType
        {
            get => _selOut;
            set
            {
                if (SetField(ref _selOut, value))
                {
                    BuildNewGenerator();
                }
            }
        }

        public DescriptiveEnum<MnemonicLength>[] MnemonicLengthList { get; }

        private DescriptiveEnum<MnemonicLength> _selMnLen;
        public DescriptiveEnum<MnemonicLength> SelectedMnemonicLength
        {
            get => _selMnLen;
            set
            {
                if (SetField(ref _selMnLen, value))
                {
                    BuildNewGenerator();
                }
            }
        }

        [DependsOnProperty(nameof(SelectedOutputType))]
        public bool IsMnemonicLengthVisible => SelectedOutputType == OutputType.Bip39Mnemonic;


        private void BuildNewGenerator()
        {
            Generator = SelectedInputType switch
            {
                InputType.BinaryGrid => new BinaryGridViewModel(SelectedOutputType, SelectedMnemonicLength.Value),
                InputType.GroupedBinary => new GroupedBinaryViewModel(SelectedOutputType, SelectedMnemonicLength.Value),
                InputType.Keyboard => new WithKeyboardViewModel(SelectedOutputType, SelectedMnemonicLength.Value),
                _ => throw new NotImplementedException(),
            };
        }

        private GeneratorVM _generator;
        public GeneratorVM Generator
        {
            get => _generator;
            set => SetField(ref _generator, value);
        }

        private bool _isCopy;
        public bool IsCopyReady
        {
            get => _isCopy;
            set => SetField(ref _isCopy, value);
        }

        private string _res = string.Empty;
        public string Result
        {
            get => _res;
            set => SetField(ref _res, value);
        }


        [DependsOnProperty(nameof(SelectedOutputType))]
        public string CopyButtonName
        {
            get
            {
                return SelectedOutputType switch
                {
                    OutputType.PrivateKey => "Copy WIF",
                    OutputType.Bip39Mnemonic => "Copy mnemonic",
                    OutputType.ElectrumMnemonic => "Copy mnemonic",
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public void CopyHex()
        {
            Debug.Assert(Generator is not null);
            Debug.Assert(Generator.Stream is not null);
            Debug.Assert(Generator.Stream.IsAllSet);

            byte[] ba = Generator.Stream.ToBytes();
            string hex = Base16.Encode(ba);
            Application.Current?.Clipboard?.SetTextAsync(hex);
        }

        public void CopyOutput()
        {
            try
            {
                byte[] ba = Generator.Stream.ToBytes();
                string res = SelectedOutputType switch
                {
                    OutputType.PrivateKey => new PrivateKey(ba).ToWif(true),
                    OutputType.Bip39Mnemonic => new BIP0039(ba).ToMnemonic(),
                    OutputType.ElectrumMnemonic => new ElectrumMnemonic(ba, ElectrumMnemonic.MnemonicType.SegWit).ToMnemonic(),
                    _ => throw new NotImplementedException()
                };

                Application.Current?.Clipboard?.SetTextAsync(res);
            }
            catch
            {
                Application.Current?.Clipboard?.ClearAsync();
            }
        }

        public void Finilize()
        {
            if (!Generator.Stream.TrySetChecksum())
            {
                Result = "Could not set checksum.";
            }
            else if (Generator.Stream.TryGetResult(out string res, out string hex, out string error))
            {
                Result = $"{res}{Environment.NewLine}{hex}";
                IsCopyReady = true;
            }
            else
            {
                Result = error;
                IsCopyReady = false;
            }
        }
    }
}
