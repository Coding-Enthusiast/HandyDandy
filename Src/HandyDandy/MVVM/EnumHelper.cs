﻿// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandyDandy.MVVM
{
    public static class EnumHelper
    {
        public static int Add(this InputType a, OutputType b, MnemonicLength c) => (int)a | (int)b | (int)c;

        public static IEnumerable<T> GetAllEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> GetEnumValues<T>(params T[] exclude) where T : Enum
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (exclude != null && !exclude.Contains(item))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<DescriptiveEnum<T>> GetDescriptiveEnums<T>(params T[] exclude) where T : Enum
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (exclude != null && !exclude.Contains(item))
                {
                    yield return new DescriptiveEnum<T>(item);
                }
            }
        }
    }
}
