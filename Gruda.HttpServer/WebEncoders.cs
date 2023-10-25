using System.Buffers;
using System.Diagnostics;
using System.Globalization;

namespace Gruda.HttpServer;

/// <summary>
/// https://source.dot.net/#Microsoft.AspNetCore.WebUtilities/src/Shared/WebEncoders/WebEncoders.cs,cbfd4784152e14a4
/// </summary>
public static class WebEncoders
{
    public static byte[] Base64UrlDecode(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return Base64UrlDecode(input, offset: 0, count: input.Length);
    }

    public static byte[] Base64UrlDecode(string input, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(input);

        ValidateParameters(input.Length, nameof(input), offset, count);

        // Special-case empty input
        if (count == 0)
        {
            return Array.Empty<byte>();
        }

        // Create array large enough for the Base64 characters, not just shorter Base64-URL-encoded form.
        var buffer = new char[GetArraySizeRequiredToDecode(count)];

        return Base64UrlDecode(input, offset, buffer, bufferOffset: 0, count: count);
    }

    public static byte[] Base64UrlDecode(string input, int offset, char[] buffer, int bufferOffset, int count)
    {
        if (count == 0)
        {
            return Array.Empty<byte>();
        }

        // Assumption: input is base64url encoded without padding and contains no whitespace.

        var paddingCharsToAdd = GetNumBase64PaddingCharsToAddForDecode(count);
        var arraySizeRequired = checked(count + paddingCharsToAdd);
        Debug.Assert(arraySizeRequired % 4 == 0, "Invariant: Array length must be a multiple of 4.");

        if (buffer.Length - bufferOffset < arraySizeRequired)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Invalid {0}, {1} or {2} length.",
                    nameof(count),
                    nameof(bufferOffset),
                    nameof(input)),
                nameof(count));
        }

        // Copy input into buffer, fixing up '-' -> '+' and '_' -> '/'.
        var i = bufferOffset;
        for (var j = offset; i - bufferOffset < count; i++, j++)
        {
            var ch = input[j];
            if (ch == '-')
            {
                buffer[i] = '+';
            }
            else if (ch == '_')
            {
                buffer[i] = '/';
            }
            else
            {
                buffer[i] = ch;
            }
        }

        // Add the padding characters back.
        for (; paddingCharsToAdd > 0; i++, paddingCharsToAdd--)
        {
            buffer[i] = '=';
        }

        // Decode.
        // If the caller provided invalid base64 chars, they'll be caught here.
        return Convert.FromBase64CharArray(buffer, bufferOffset, arraySizeRequired);
    }

    public static string Base64UrlEncode(ReadOnlySpan<byte> input)
    {
        const int stackAllocThreshold = 128;

        if (input.IsEmpty)
        {
            return string.Empty;
        }

        int bufferSize = GetArraySizeRequiredToEncode(input.Length);

        char[]? bufferToReturnToPool = null;
        Span<char> buffer = bufferSize <= stackAllocThreshold
            ? stackalloc char[stackAllocThreshold]
            : bufferToReturnToPool = ArrayPool<char>.Shared.Rent(bufferSize);

        var numBase64Chars = Base64UrlEncode(input, buffer);
        var base64Url = new string(buffer.Slice(0, numBase64Chars));

        if (bufferToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(bufferToReturnToPool);
        }

        return base64Url;
    }

    private static int Base64UrlEncode(ReadOnlySpan<byte> input, Span<char> output)
    {
        Debug.Assert(output.Length >= GetArraySizeRequiredToEncode(input.Length));

        if (input.IsEmpty)
        {
            return 0;
        }

        // Use base64url encoding with no padding characters. See RFC 4648, Sec. 5.

        Convert.TryToBase64Chars(input, output, out int charsWritten);

        // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
        for (var i = 0; i < charsWritten; i++)
        {
            var ch = output[i];
            if (ch == '+')
            {
                output[i] = '-';
            }
            else if (ch == '/')
            {
                output[i] = '_';
            }
            else if (ch == '=')
            {
                // We've reached a padding character; truncate the remainder.
                return i;
            }
        }

        return charsWritten;
    }

    private static int GetNumBase64PaddingCharsToAddForDecode(int inputLength)
    {
        switch (inputLength % 4)
        {
            case 0:
                return 0;
            case 2:
                return 2;
            case 3:
                return 1;
            default:
                throw new FormatException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Malformed input: {0} is an invalid input length.",
                        inputLength));
        }
    }

    private static int GetArraySizeRequiredToDecode(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count == 0)
        {
            return 0;
        }

        var numPaddingCharsToAdd = GetNumBase64PaddingCharsToAddForDecode(count);

        return checked(count + numPaddingCharsToAdd);
    }

    private static int GetArraySizeRequiredToEncode(int count)
    {
        var numWholeOrPartialInputBlocks = checked(count + 2) / 3;
        return checked(numWholeOrPartialInputBlocks * 4);
    }

    private static void ValidateParameters(int bufferLength, string inputName, int offset, int count)
    {
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (bufferLength - offset < count)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Invalid {0}, {1} or {2} length.",
                    nameof(count),
                    nameof(offset),
                    inputName),
                nameof(count));
        }
    }
}