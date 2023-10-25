using System.Collections.Frozen;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Gruda.HttpServer;

public static class ReasonPhrases
{
    // Status Codes listed at http://www.iana.org/assignments/http-status-codes/http-status-codes.xhtml
    private static readonly FrozenDictionary<int, string> Phrases = (new Dictionary<int, string>()
    {
        {100, "Continue"},
        {101, "Switching Protocols"},
        {102, "Processing"},

        {200, "OK"},
        {201, "Created"},
        {202, "Accepted"},
        {203, "Non-Authoritative Information"},
        {204, "No Content"},
        {205, "Reset Content"},
        {206, "Partial Content"},
        {207, "Multi-Status"},
        {208, "Already Reported"},
        {226, "IM Used"},

        {300, "Multiple Choices"},
        {301, "Moved Permanently"},
        {302, "Found"},
        {303, "See Other"},
        {304, "Not Modified"},
        {305, "Use Proxy"},
        {306, "Switch Proxy"},
        {307, "Temporary Redirect"},
        {308, "Permanent Redirect"},

        {400, "Bad Request"},
        {401, "Unauthorized"},
        {402, "Payment Required"},
        {403, "Forbidden"},
        {404, "Not Found"},
        {405, "Method Not Allowed"},
        {406, "Not Acceptable"},
        {407, "Proxy Authentication Required"},
        {408, "Request Timeout"},
        {409, "Conflict"},
        {410, "Gone"},
        {411, "Length Required"},
        {412, "Precondition Failed"},
        {413, "Payload Too Large"},
        {414, "URI Too Long"},
        {415, "Unsupported Media Type"},
        {416, "Range Not Satisfiable"},
        {417, "Expectation Failed"},
        {418, "I'm a teapot"},
        {419, "Authentication Timeout"},
        {421, "Misdirected Request"},
        {422, "Unprocessable Entity"},
        {423, "Locked"},
        {424, "Failed Dependency"},
        {426, "Upgrade Required"},
        {428, "Precondition Required"},
        {429, "Too Many Requests"},
        {431, "Request Header Fields Too Large"},
        {451, "Unavailable For Legal Reasons"},

        {500, "Internal Server Error"},
        {501, "Not Implemented"},
        {502, "Bad Gateway"},
        {503, "Service Unavailable"},
        {504, "Gateway Timeout"},
        {505, "HTTP Version Not Supported"},
        {506, "Variant Also Negotiates"},
        {507, "Insufficient Storage"},
        {508, "Loop Detected"},
        {510, "Not Extended"},
        {511, "Network Authentication Required"},
    }).ToFrozenDictionary();
    
    private static readonly byte[] BytesStatus100 = CreateStatusBytes(StatusCodes.Status100Continue);
    private static readonly byte[] BytesStatus101 = CreateStatusBytes(StatusCodes.Status101SwitchingProtocols);
    private static readonly byte[] BytesStatus102 = CreateStatusBytes(StatusCodes.Status102Processing);

    private static readonly byte[] BytesStatus200 = CreateStatusBytes(StatusCodes.Status200OK);
    private static readonly byte[] BytesStatus201 = CreateStatusBytes(StatusCodes.Status201Created);
    private static readonly byte[] BytesStatus202 = CreateStatusBytes(StatusCodes.Status202Accepted);
    private static readonly byte[] BytesStatus203 = CreateStatusBytes(StatusCodes.Status203NonAuthoritative);
    private static readonly byte[] BytesStatus204 = CreateStatusBytes(StatusCodes.Status204NoContent);
    private static readonly byte[] BytesStatus205 = CreateStatusBytes(StatusCodes.Status205ResetContent);
    private static readonly byte[] BytesStatus206 = CreateStatusBytes(StatusCodes.Status206PartialContent);
    private static readonly byte[] BytesStatus207 = CreateStatusBytes(StatusCodes.Status207MultiStatus);
    private static readonly byte[] BytesStatus208 = CreateStatusBytes(StatusCodes.Status208AlreadyReported);
    private static readonly byte[] BytesStatus226 = CreateStatusBytes(StatusCodes.Status226IMUsed);

    private static readonly byte[] BytesStatus300 = CreateStatusBytes(StatusCodes.Status300MultipleChoices);
    private static readonly byte[] BytesStatus301 = CreateStatusBytes(StatusCodes.Status301MovedPermanently);
    private static readonly byte[] BytesStatus302 = CreateStatusBytes(StatusCodes.Status302Found);
    private static readonly byte[] BytesStatus303 = CreateStatusBytes(StatusCodes.Status303SeeOther);
    private static readonly byte[] BytesStatus304 = CreateStatusBytes(StatusCodes.Status304NotModified);
    private static readonly byte[] BytesStatus305 = CreateStatusBytes(StatusCodes.Status305UseProxy);
    private static readonly byte[] BytesStatus306 = CreateStatusBytes(StatusCodes.Status306SwitchProxy);
    private static readonly byte[] BytesStatus307 = CreateStatusBytes(StatusCodes.Status307TemporaryRedirect);
    private static readonly byte[] BytesStatus308 = CreateStatusBytes(StatusCodes.Status308PermanentRedirect);

    private static readonly byte[] BytesStatus400 = CreateStatusBytes(StatusCodes.Status400BadRequest);
    private static readonly byte[] BytesStatus401 = CreateStatusBytes(StatusCodes.Status401Unauthorized);
    private static readonly byte[] BytesStatus402 = CreateStatusBytes(StatusCodes.Status402PaymentRequired);
    private static readonly byte[] BytesStatus403 = CreateStatusBytes(StatusCodes.Status403Forbidden);
    private static readonly byte[] BytesStatus404 = CreateStatusBytes(StatusCodes.Status404NotFound);
    private static readonly byte[] BytesStatus405 = CreateStatusBytes(StatusCodes.Status405MethodNotAllowed);
    private static readonly byte[] BytesStatus406 = CreateStatusBytes(StatusCodes.Status406NotAcceptable);

    private static readonly byte[]
        BytesStatus407 = CreateStatusBytes(StatusCodes.Status407ProxyAuthenticationRequired);

    private static readonly byte[] BytesStatus408 = CreateStatusBytes(StatusCodes.Status408RequestTimeout);
    private static readonly byte[] BytesStatus409 = CreateStatusBytes(StatusCodes.Status409Conflict);
    private static readonly byte[] BytesStatus410 = CreateStatusBytes(StatusCodes.Status410Gone);
    private static readonly byte[] BytesStatus411 = CreateStatusBytes(StatusCodes.Status411LengthRequired);
    private static readonly byte[] BytesStatus412 = CreateStatusBytes(StatusCodes.Status412PreconditionFailed);
    private static readonly byte[] BytesStatus413 = CreateStatusBytes(StatusCodes.Status413PayloadTooLarge);
    private static readonly byte[] BytesStatus414 = CreateStatusBytes(StatusCodes.Status414UriTooLong);
    private static readonly byte[] BytesStatus415 = CreateStatusBytes(StatusCodes.Status415UnsupportedMediaType);
    private static readonly byte[] BytesStatus416 = CreateStatusBytes(StatusCodes.Status416RangeNotSatisfiable);
    private static readonly byte[] BytesStatus417 = CreateStatusBytes(StatusCodes.Status417ExpectationFailed);
    private static readonly byte[] BytesStatus418 = CreateStatusBytes(StatusCodes.Status418ImATeapot);
    private static readonly byte[] BytesStatus419 = CreateStatusBytes(StatusCodes.Status419AuthenticationTimeout);
    private static readonly byte[] BytesStatus421 = CreateStatusBytes(StatusCodes.Status421MisdirectedRequest);
    private static readonly byte[] BytesStatus422 = CreateStatusBytes(StatusCodes.Status422UnprocessableEntity);
    private static readonly byte[] BytesStatus423 = CreateStatusBytes(StatusCodes.Status423Locked);
    private static readonly byte[] BytesStatus424 = CreateStatusBytes(StatusCodes.Status424FailedDependency);
    private static readonly byte[] BytesStatus426 = CreateStatusBytes(StatusCodes.Status426UpgradeRequired);
    private static readonly byte[] BytesStatus428 = CreateStatusBytes(StatusCodes.Status428PreconditionRequired);
    private static readonly byte[] BytesStatus429 = CreateStatusBytes(StatusCodes.Status429TooManyRequests);

    private static readonly byte[]
        BytesStatus431 = CreateStatusBytes(StatusCodes.Status431RequestHeaderFieldsTooLarge);

    private static readonly byte[] BytesStatus451 = CreateStatusBytes(StatusCodes.Status451UnavailableForLegalReasons);

    private static readonly byte[] BytesStatus500 = CreateStatusBytes(StatusCodes.Status500InternalServerError);
    private static readonly byte[] BytesStatus501 = CreateStatusBytes(StatusCodes.Status501NotImplemented);
    private static readonly byte[] BytesStatus502 = CreateStatusBytes(StatusCodes.Status502BadGateway);
    private static readonly byte[] BytesStatus503 = CreateStatusBytes(StatusCodes.Status503ServiceUnavailable);
    private static readonly byte[] BytesStatus504 = CreateStatusBytes(StatusCodes.Status504GatewayTimeout);
    private static readonly byte[] BytesStatus505 = CreateStatusBytes(StatusCodes.Status505HttpVersionNotsupported);
    private static readonly byte[] BytesStatus506 = CreateStatusBytes(StatusCodes.Status506VariantAlsoNegotiates);
    private static readonly byte[] BytesStatus507 = CreateStatusBytes(StatusCodes.Status507InsufficientStorage);
    private static readonly byte[] BytesStatus508 = CreateStatusBytes(StatusCodes.Status508LoopDetected);
    private static readonly byte[] BytesStatus510 = CreateStatusBytes(StatusCodes.Status510NotExtended);

    private static readonly byte[] BytesStatus511 =
        CreateStatusBytes(StatusCodes.Status511NetworkAuthenticationRequired);

    private static byte[] CreateStatusBytes(int statusCode)
    {
        var reasonPhrase = GetReasonPhrase(statusCode);
        Debug.Assert(!string.IsNullOrEmpty(reasonPhrase));

        return CreateStatusBytes(statusCode, reasonPhrase);
    }

    private static byte[] CreateStatusBytes(int statusCode, string? reasonPhrase)
    {
        // https://tools.ietf.org/html/rfc7230#section-3.1.2 requires trailing whitespace regardless of reason phrase
        return Encoding.ASCII.GetBytes(statusCode.ToString(CultureInfo.InvariantCulture) + " " + reasonPhrase);
    }

    public static byte[] ToBytes(int statusCode, string? reasonPhrase = null)
    {
        var candidate = statusCode switch
        {
            StatusCodes.Status100Continue => BytesStatus100,
            StatusCodes.Status101SwitchingProtocols => BytesStatus101,
            StatusCodes.Status102Processing => BytesStatus102,

            StatusCodes.Status200OK => BytesStatus200,
            StatusCodes.Status201Created => BytesStatus201,
            StatusCodes.Status202Accepted => BytesStatus202,
            StatusCodes.Status203NonAuthoritative => BytesStatus203,
            StatusCodes.Status204NoContent => BytesStatus204,
            StatusCodes.Status205ResetContent => BytesStatus205,
            StatusCodes.Status206PartialContent => BytesStatus206,
            StatusCodes.Status207MultiStatus => BytesStatus207,
            StatusCodes.Status208AlreadyReported => BytesStatus208,
            StatusCodes.Status226IMUsed => BytesStatus226,

            StatusCodes.Status300MultipleChoices => BytesStatus300,
            StatusCodes.Status301MovedPermanently => BytesStatus301,
            StatusCodes.Status302Found => BytesStatus302,
            StatusCodes.Status303SeeOther => BytesStatus303,
            StatusCodes.Status304NotModified => BytesStatus304,
            StatusCodes.Status305UseProxy => BytesStatus305,
            StatusCodes.Status306SwitchProxy => BytesStatus306,
            StatusCodes.Status307TemporaryRedirect => BytesStatus307,
            StatusCodes.Status308PermanentRedirect => BytesStatus308,

            StatusCodes.Status400BadRequest => BytesStatus400,
            StatusCodes.Status401Unauthorized => BytesStatus401,
            StatusCodes.Status402PaymentRequired => BytesStatus402,
            StatusCodes.Status403Forbidden => BytesStatus403,
            StatusCodes.Status404NotFound => BytesStatus404,
            StatusCodes.Status405MethodNotAllowed => BytesStatus405,
            StatusCodes.Status406NotAcceptable => BytesStatus406,
            StatusCodes.Status407ProxyAuthenticationRequired => BytesStatus407,
            StatusCodes.Status408RequestTimeout => BytesStatus408,
            StatusCodes.Status409Conflict => BytesStatus409,
            StatusCodes.Status410Gone => BytesStatus410,
            StatusCodes.Status411LengthRequired => BytesStatus411,
            StatusCodes.Status412PreconditionFailed => BytesStatus412,
            StatusCodes.Status413PayloadTooLarge => BytesStatus413,
            StatusCodes.Status414UriTooLong => BytesStatus414,
            StatusCodes.Status415UnsupportedMediaType => BytesStatus415,
            StatusCodes.Status416RangeNotSatisfiable => BytesStatus416,
            StatusCodes.Status417ExpectationFailed => BytesStatus417,
            StatusCodes.Status418ImATeapot => BytesStatus418,
            StatusCodes.Status419AuthenticationTimeout => BytesStatus419,
            StatusCodes.Status421MisdirectedRequest => BytesStatus421,
            StatusCodes.Status422UnprocessableEntity => BytesStatus422,
            StatusCodes.Status423Locked => BytesStatus423,
            StatusCodes.Status424FailedDependency => BytesStatus424,
            StatusCodes.Status426UpgradeRequired => BytesStatus426,
            StatusCodes.Status428PreconditionRequired => BytesStatus428,
            StatusCodes.Status429TooManyRequests => BytesStatus429,
            StatusCodes.Status431RequestHeaderFieldsTooLarge => BytesStatus431,
            StatusCodes.Status451UnavailableForLegalReasons => BytesStatus451,

            StatusCodes.Status500InternalServerError => BytesStatus500,
            StatusCodes.Status501NotImplemented => BytesStatus501,
            StatusCodes.Status502BadGateway => BytesStatus502,
            StatusCodes.Status503ServiceUnavailable => BytesStatus503,
            StatusCodes.Status504GatewayTimeout => BytesStatus504,
            StatusCodes.Status505HttpVersionNotsupported => BytesStatus505,
            StatusCodes.Status506VariantAlsoNegotiates => BytesStatus506,
            StatusCodes.Status507InsufficientStorage => BytesStatus507,
            StatusCodes.Status508LoopDetected => BytesStatus508,
            StatusCodes.Status510NotExtended => BytesStatus510,
            StatusCodes.Status511NetworkAuthenticationRequired => BytesStatus511,

            _ => null
        };

        if (candidate is not null && (string.IsNullOrEmpty(reasonPhrase) ||
                                      GetReasonPhrase(statusCode) == reasonPhrase))
        {
            return candidate;
        }

        return CreateStatusBytes(statusCode, reasonPhrase);
    }


    /// <summary>
    /// Gets the reason phrase for the specified status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <returns>The reason phrase, or <see cref="string.Empty"/> if the status code is unknown.</returns>
    public static string GetReasonPhrase(int statusCode)
    {
        return Phrases.TryGetValue(statusCode, out var phrase) ? phrase : string.Empty;
    }
}