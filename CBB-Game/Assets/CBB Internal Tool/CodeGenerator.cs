using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CodeGenerator
{
    private static string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string lowers = "abcdefghijklmnopqrstuvwxyz";
    private static string numbers = "0123456789";

    public enum LettersType
    {
        Uppercase,
        Lowercase,
        Mixed
    }

    public enum CodeType
    {
        OnlyNumbers,
        OnlyLetters,
        Mixed
    }

    private static string GetCharacter(CodeType codeType, LettersType lettersType)
    {
        var result = "";
        switch (codeType)
        {
            case CodeType.OnlyNumbers:
                result += numbers;
                break;

            case CodeType.OnlyLetters:
                switch (lettersType)
                {
                    case LettersType.Uppercase:
                        result += uppers;
                        break;
                    case LettersType.Lowercase:
                        result += lowers;
                        break;
                    case LettersType.Mixed:
                        result += lowers + uppers;
                        break;
                    default:
                        break;
                }
                break;

            case CodeType.Mixed:
                result += numbers;
                switch (lettersType)
                {
                    case LettersType.Uppercase:
                        result += uppers;
                        break;
                    case LettersType.Lowercase:
                        result += lowers;
                        break;
                    case LettersType.Mixed:
                        result += lowers + uppers;
                        break;
                    default:
                        break;
                }
                break;

            default:
                break;
        }
        return result;
    }

    public static string Generate(int groupLength, int groupCount, CodeType codeType = CodeType.OnlyLetters, LettersType lettersType = LettersType.Lowercase)
    {
        string result = "";

        for (int i = 0; i < groupCount; i++)
        {
            for (int j = 0; j < groupLength; j++)
            {
                var chars = GetCharacter(codeType, lettersType);
                char randomCharacter = chars[Random.Range(0, chars.Length)];
                result += randomCharacter;
            }

            if (i < groupCount - 1)
            {
                result += "-";
            }
        }
        return result;
    }
}