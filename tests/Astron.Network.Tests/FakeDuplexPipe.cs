using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Astron.Network.Tests
{
    public class FakeDuplexPipe : IDuplexPipe
    {
        private Pipe _readPipe;
        private Pipe _writePipe;

        public PipeReader Input  => _readPipe.Reader;
        public PipeWriter Output => _writePipe.Writer;

        public FakeDuplexPipe()
        {
            _readPipe = new Pipe();
            _writePipe = new Pipe();
        }

        public ValueTask<FlushResult> FakeRead(byte[] buffer)
            => _readPipe.Writer.WriteAsync(buffer);

        public ValueTask<ReadResult> ReadPipeWriter()
            => _writePipe.Reader.ReadAsync();
    }
}
