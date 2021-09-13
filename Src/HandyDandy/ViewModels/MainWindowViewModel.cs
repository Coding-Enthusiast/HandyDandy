// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using HandyDandy.MVVM;
using System;
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
        }



        public InputType[] InputTypeList { get; }

        private InputType _selIn;
        public InputType SelectedInputType
        {
            get => _selIn;
            set => SetField(ref _selIn, value);
        }

        public OutputType[] OutputTypeList { get; }

        private OutputType _selOut;
        public OutputType SelectedOutputType
        {
            get => _selOut;
            set => SetField(ref _selOut, value);
        }

        public DescriptiveEnum<MnemonicLength>[] MnemonicLengthList { get; }

        private DescriptiveEnum<MnemonicLength> _selMnLen;
        public DescriptiveEnum<MnemonicLength> SelectedMnemonicLength
        {
            get => _selMnLen;
            set => SetField(ref _selMnLen, value);
        }

        [DependsOnProperty(nameof(SelectedOutputType))]
        public bool IsMnemonicLengthVisible => SelectedOutputType == OutputType.Bip39Mnemonic;

        [DependsOnProperty(nameof(SelectedInputType), nameof(SelectedOutputType), nameof(SelectedMnemonicLength))]
        public ViewModelBase Generator
        {
            get
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
                    OutputType.ElectrumMnemonic => 128,
                    _ => throw new NotImplementedException(),
                };

                return SelectedInputType switch
                {
                    InputType.BinaryGrid => new BinaryGridViewModel(len, SelectedOutputType),
                    InputType.GroupedBinary => new GroupedBinaryViewModel(len, SelectedOutputType),
                    _ => throw new NotImplementedException(),
                };
            }
        }
    }
}
