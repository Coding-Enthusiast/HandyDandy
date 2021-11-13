// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.MVVM;
using HandyDandy.Services;
using System.ComponentModel;
using System.Diagnostics;

namespace HandyDandy.Models
{
    public class LinkedValues : InpcBase
    {
        public LinkedValues() : this(new TernaryStream(3, 1, null, false, OutputType.PrivateKey),
                                     new string[] { "Foo0", "Foo1", "Foo2", "Foo3" }, 1)
        {
        }

        public LinkedValues(TernaryStream stream, string[]? words, int len)
        {
            format = len == 8 ? "x2" : "x4";
            _hex = 0.ToString(format);
            needWords = words is not null;
            allWords = words;
            Buttons = stream.Next(len);
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].PropertyChanged += Button_PropertyChanged;
            }
        }


        private void Button_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            int result = 0;
            for (int i = 0; i < Buttons.Length; i++)
            {
                int bit = Buttons[i].ToBit();
                result |= bit << (Buttons.Length - i - 1);
            }

            Value = result;
            Hex = $"0x{Value.ToString(format)}";
            if (needWords)
            {
                Debug.Assert(allWords is not null);
                Debug.Assert(Value < allWords.Length);

                Word = allWords[Value];
            }
        }

        private readonly string format;
        private readonly bool needWords;
        private readonly string[]? allWords;

        public Ternary[] Buttons { get; private set; }

        private string _hex;
        public string Hex
        {
            get => _hex;
            private set => SetField(ref _hex, value);
        }

        private string _wrd;
        public string Word
        {
            get => _wrd;
            private set => SetField(ref _wrd, value);
        }

        private int _val;
        public int Value
        {
            get => _val;
            private set => SetField(ref _val, value);
        }
    }
}
