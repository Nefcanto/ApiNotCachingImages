namespace Infra;

public class Alphabet
{
    public static List<string> EnglishLowercaseLetters
    {
        get
        {
            var result = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            return result;
        }
    }

    public static List<string> EnglishUppercaseLetters
    {
        get
        {
            var result = EnglishLowercaseLetters.Select(i => i.ToUpper()).ToList();
            return result;
        }
    }

    public static List<string> PersianLetters
    {
        get
        {
            var result = new List<string> { "ا", "آ", "ب", "پ", "ت", "ث", "ج", "چ", "ح", "خ", "د", "ذ", "ر", "ز", "ژ", "س", "ش", "ص", "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ک", "گ", "ل", "م", "ن", "و", "ه", "ی" };
            return result;
        }
    }
}
