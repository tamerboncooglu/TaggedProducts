﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaggedProducts.Utils
{
    public static class StringHelper
    {
        public static string ToUrlSlug(this string text)
        {
            return Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(
                                text.Trim().ToLower()
                                       .Replace("ö", "o")
                                       .Replace("ç", "c")
                                       .Replace("ş", "s")
                                       .Replace("ı", "i")
                                       .Replace("ğ", "g")
                                       .Replace("ü", "u"),
                            @"\s+", " "), // multiple spaces to one space
                            @"\s", "-"), // spaces to hypens
                            @"[^a-z0-9\s-]", string.Empty); // removing invalid chars
        }

        public static string ToLowerTR(this string text)
        {
            return text.Trim().ToLower(Consts.CultureTR);
        }

        public static bool IsEmail(this string text)
        {
            try
            {
                new MailAddress(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsContainingLink(this string text)
        {
            return text.Contains("http:\\");
        }
    }
}