// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using HandyDandy.MVVM;
using System.ComponentModel;
using System.Linq;

namespace HandyDandy.ViewModels
{
    public enum InputType
    {
        BinaryGrid,
        GroupedBinary,
    }
    public enum OutputType
    {
        PrivateKey,
        Mnemonic
    }
    public enum MnemonicLength
    {
        [Description("12 words (128 bits)")]
        Twelve,
        [Description("15 words (160 bits)")]
        Fifteen,
        [Description("18 words (192 bits)")]
        Eighteen,
        [Description("21 words (224 bits)")]
        TwentyOne,
        [Description("24 words (256 bits)")]
        TwentyFour
    }
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            InputTypeList = EnumHelper.GetAllEnumValues<InputType>().ToArray();
            SelectedInputType = InputTypeList[0];

            OutputTypeList = EnumHelper.GetAllEnumValues<OutputType>().ToArray();
            SelectedOutputType = OutputTypeList[0];

            MnemonicLengthList = EnumHelper.GetDescriptiveEnums<MnemonicLength>().ToArray();
            SelectedMnemonicLength = MnemonicLengthList.First();
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
        public bool IsMnemonicLengthVisible => SelectedOutputType == OutputType.Mnemonic;

        [DependsOnProperty(nameof(SelectedInputType), nameof(SelectedOutputType), nameof(SelectedMnemonicLength))]
        public ViewModelBase Generator
        {
            get
            {
                int len = SelectedOutputType switch
                {
                    OutputType.PrivateKey => 256,
                    OutputType.Mnemonic => SelectedMnemonicLength.Value switch
                    {
                        MnemonicLength.Twelve => 128,
                        MnemonicLength.Fifteen => 160,
                        MnemonicLength.Eighteen => 192,
                        MnemonicLength.TwentyOne => 224,
                        MnemonicLength.TwentyFour => 256,
                        _ => throw new System.NotImplementedException(),
                    },
                    _ => throw new System.NotImplementedException(),
                };

                return SelectedInputType switch
                {
                    InputType.BinaryGrid => new BinaryGridViewModel(len, SelectedOutputType),
                    InputType.GroupedBinary => new GroupedBinaryViewModel(len, SelectedOutputType),
                };
            }
        }

    }
}
