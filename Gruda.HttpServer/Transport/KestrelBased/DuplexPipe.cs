﻿using System.IO.Pipelines;

namespace Gruda.HttpServer.Transport.KestrelBased;

// From: https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ServerInfrastructure/DuplexPipe.cs
internal sealed class DuplexPipe : IDuplexPipe
{
    public DuplexPipe(PipeReader reader, PipeWriter writer)
    {
        Input = reader;
        Output = writer;
    }

    public PipeReader Input { get; }

    public PipeWriter Output { get; }

    public static DuplexPipePair CreateConnectionPair(PipeOptions inputOptions, PipeOptions outputOptions)
    {
        var input = new Pipe(inputOptions);
        var output = new Pipe(outputOptions);

        // tta input = output.reader | output = input.writer
        // att input = input.reader | output = output.writer
        
        var transportToApplication = new DuplexPipe(output.Reader, input.Writer);
        var applicationToTransport = new DuplexPipe(input.Reader, output.Writer);

        return new DuplexPipePair(applicationToTransport, transportToApplication);
    }

    // This class exists to work around issues with value tuple on .NET Framework
    public readonly struct DuplexPipePair
    {
        public IDuplexPipe Transport { get; }
        public IDuplexPipe Application { get; }

        public DuplexPipePair(IDuplexPipe transport, IDuplexPipe application)
        {
            Transport = transport;
            Application = application;
        }
    }
}