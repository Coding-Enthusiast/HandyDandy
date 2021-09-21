// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.MVVM;
using System.ComponentModel;
using System.Diagnostics;

namespace HandyDandy.Models
{
    public class Quad : InpcBase
    {
        public Quad() : this(2, new string[] { "Foo0", "Foo1", "Foo2", "Foo3" }, 1) { }

        public Quad(int len)
        {
            Buttons = new Ternary[len];
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i] = new Ternary(true);
                Buttons[i].PropertyChanged += Button_PropertyChanged;
            }
            needWords = false;
        }

        public Quad(int len, string[]? words, int disableCount)
        {
            Buttons = new Ternary[len];
            int i = 0;
            for (; i < Buttons.Length - disableCount; i++)
            {
                Buttons[i] = new Ternary(true);
                Buttons[i].PropertyChanged += Button_PropertyChanged;
            }
            for (; i < Buttons.Length; i++)
            {
                Buttons[i] = new Ternary(false);
                Buttons[i].PropertyChanged += Button_PropertyChanged;
            }

            needWords = words is not null;
            allWords = words;
            if (allWords is not null)
            {
                _wrd = allWords[0];
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
            Hex = Value.ToString("x4");
            if (needWords)
            {
                Debug.Assert(allWords is not null);
                Debug.Assert(Value < allWords.Length);

                Word = allWords[Value];
            }
        }

        private readonly bool needWords;
        private readonly string[]? allWords;

        public Ternary[] Buttons { get; private set; }

        private string _hex = "0000";
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
