﻿// HandyDandy
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

namespace HandyDandy.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            InputTypeList = EnumHelper.GetAllEnumValues<InputType>().ToArray();
            SelectedInputType = InputTypeList[0];

            OutputTypeList = EnumHelper.GetAllEnumValues<OutputType>().ToArray();
            SelectedOutputType = OutputTypeList[0];

            MnemonicLengthList = EnumHelper.GetDescriptiveEnums<MnemonicLength>().ToArray();
            _selMnLen = MnemonicLengthList.First();

            // This has to be called last
            BuildNewGenerator();
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
            int len = SelectedOutputType switch
            {
                OutputType.PrivateKey => 256,
                OutputType.Bip39Mnemonic => SelectedMnemonicLength.Value switch
                {
                    MnemonicLength.Twelve => 128,
                    MnemonicLength.Fifteen => 160,
                    MnemonicLength.Eighteen => 192,
                    MnemonicLength.TwentyOne => 224,
                    MnemonicLength.TwentyFour => 256,
                    _ => throw new NotImplementedException(),
                },
                // We only support the default 12-word Electrum mnemonics
                OutputType.ElectrumMnemonic => 136,
                _ => throw new NotImplementedException(),
            };

            Generator = SelectedInputType switch
            {
                InputType.BinaryGrid => new BinaryGridViewModel(len, SelectedOutputType),
                InputType.GroupedBinary => new GroupedBinaryViewModel(SelectedOutputType, SelectedMnemonicLength.Value),
                _ => throw new NotImplementedException(),
            };
        }

        private GeneratorVM _generator;
        public GeneratorVM Generator
        {
            get => _generator;
            set => SetField(ref _generator, value);
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
            Application.Current.Clipboard.SetTextAsync(hex);
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

                Application.Current.Clipboard.SetTextAsync(res);
            }
            catch
            {
                Application.Current.Clipboard.ClearAsync();
            }
        }
    }
}
