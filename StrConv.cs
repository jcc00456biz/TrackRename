using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin
{
    static class StrConv
    {
        // 全角数字を半角にする
        public static string ZenBlank2Han( string org_str )
        {
            string ret_str = org_str;
            ret_str = ret_str.Replace( "　", " " );
            return ret_str;
        }
        // 全角数字を半角にする
        public static string ZenNum2Han( string org_str )
        {
            string ret_str = org_str;
            ret_str = ret_str.Replace( "０", "0" );
            ret_str = ret_str.Replace( "１", "1" );
            ret_str = ret_str.Replace( "２", "2" );
            ret_str = ret_str.Replace( "３", "3" );
            ret_str = ret_str.Replace( "４", "4" );
            ret_str = ret_str.Replace( "５", "5" );
            ret_str = ret_str.Replace( "６", "6" );
            ret_str = ret_str.Replace( "７", "7" );
            ret_str = ret_str.Replace( "８", "8" );
            ret_str = ret_str.Replace( "９", "9" );
            return ret_str;
        }
        // 大文字アルファベットを半角にする
        public static string ZenUpperCase2Han( string org_str )
        {
            string ret_str = org_str;
            ret_str = ret_str.Replace( "Ａ", "A" ).Replace( "Ｎ", "N" );
            ret_str = ret_str.Replace( "Ｂ", "B" ).Replace( "Ｏ", "O" );
            ret_str = ret_str.Replace( "Ｃ", "C" ).Replace( "Ｐ", "P" );
            ret_str = ret_str.Replace( "Ｄ", "D" ).Replace( "Ｑ", "Q" );
            ret_str = ret_str.Replace( "Ｅ", "E" ).Replace( "Ｒ", "R" );
            ret_str = ret_str.Replace( "Ｆ", "F" ).Replace( "Ｓ", "S" );
            ret_str = ret_str.Replace( "Ｇ", "G" ).Replace( "Ｔ", "T" );
            ret_str = ret_str.Replace( "Ｈ", "H" ).Replace( "Ｕ", "U" );
            ret_str = ret_str.Replace( "Ｉ", "I" ).Replace( "Ｖ", "V" );
            ret_str = ret_str.Replace( "Ｊ", "J" ).Replace( "Ｗ", "W" );
            ret_str = ret_str.Replace( "Ｋ", "K" ).Replace( "Ｘ", "X" );
            ret_str = ret_str.Replace( "Ｌ", "L" ).Replace( "Ｙ", "Y" );
            ret_str = ret_str.Replace( "Ｍ", "M" ).Replace( "Ｚ", "Z" );
            return ret_str;
        }
        // 小文字アルファベットを半角にする
        public static string ZenLowerCase2Han( string org_str )
        {
            string ret_str = org_str;
            ret_str = ret_str.Replace( "ａ", "a" ).Replace( "ｎ", "n" );
            ret_str = ret_str.Replace( "ｂ", "b" ).Replace( "ｏ", "o" );
            ret_str = ret_str.Replace( "ｃ", "c" ).Replace( "ｐ", "p" );
            ret_str = ret_str.Replace( "ｄ", "d" ).Replace( "ｑ", "q" );
            ret_str = ret_str.Replace( "ｅ", "e" ).Replace( "ｒ", "r" );
            ret_str = ret_str.Replace( "ｆ", "f" ).Replace( "ｓ", "s" );
            ret_str = ret_str.Replace( "ｇ", "g" ).Replace( "ｔ", "t" );
            ret_str = ret_str.Replace( "ｈ", "h" ).Replace( "ｕ", "u" );
            ret_str = ret_str.Replace( "ｉ", "i" ).Replace( "ｖ", "v" );
            ret_str = ret_str.Replace( "ｊ", "j" ).Replace( "ｗ", "w" );
            ret_str = ret_str.Replace( "ｋ", "k" ).Replace( "ｘ", "x" );
            ret_str = ret_str.Replace( "ｌ", "l" ).Replace( "ｙ", "y" );
            ret_str = ret_str.Replace( "ｍ", "m" ).Replace( "ｚ", "z" );
            return ret_str;
        }
        public static string ZenSymbol2Han( string org_str )
        {
            string ret_str = org_str;
            ret_str = ret_str.Replace( "！", "!" ).Replace( "．", "." );
            ret_str = ret_str.Replace( "”", "\"" ).Replace( "／", " / " );
            ret_str = ret_str.Replace( "＃", "#" ).Replace( "：", ":" );
            ret_str = ret_str.Replace( "＄", "$" ).Replace( "；", ";" );
            ret_str = ret_str.Replace( "％", "%" ).Replace( "＜", "<" );
            ret_str = ret_str.Replace( "＆", "&" ).Replace( "＝", "=" );
            ret_str = ret_str.Replace( "’", "'" ).Replace( "＞", ">" );
            ret_str = ret_str.Replace( "（", "(" ).Replace( "？", "?" );
            ret_str = ret_str.Replace( "）", ")" ).Replace( "＠", "@" );
            ret_str = ret_str.Replace( "＊", "*" ).Replace( "｛", "{" );
            ret_str = ret_str.Replace( "＋", "+" ).Replace( "｜", "|" );
            ret_str = ret_str.Replace( "，", "," ).Replace( "｝", "}" );
            ret_str = ret_str.Replace( "－", "-" ).Replace( "￣", "~" );
            return ret_str;
        }
        public static string Zen2Han( string org_str )
        {
            string ret_str = org_str;
            ret_str = ZenBlank2Han( ret_str );
            ret_str = ZenNum2Han( ret_str );
            ret_str = ZenUpperCase2Han( ret_str );
            ret_str = ZenLowerCase2Han( ret_str );
            ret_str = ZenSymbol2Han( ret_str );
            return ret_str;
        }

    }
}
