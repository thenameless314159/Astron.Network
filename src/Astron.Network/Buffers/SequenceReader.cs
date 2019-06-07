using System;
using System.Buffers;
using System.Buffers.Binary;

namespace Astron.Network.Buffers
{
    public class SequenceReader
    {
        private const int _maxStackalloc = 128;
        protected ReadOnlySequence<byte> _input;

        public SequenceReader(ReadOnlySequence<byte> input) => _input = input;

        public delegate T ReadDelegate<out T>(ReadOnlySpan<byte> src);

        /// <summary>
        /// Try to read a <see cref="T"/> with the <see cref="ReadDelegate{T}"/> specified as an arg.
        /// The <see cref="SequenceReader"/> then advance the current position according to the size of <see cref="T"/>.
        /// <see cref="T"/> must be a struct :
        /// <see cref="byte"/>, <see cref="sbyte"/>, <see cref="bool"/>, <see cref="short"/>, 
        /// <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, 
        /// <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>, 
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <param name="read">The delegate to read the <see cref="T"/>. Must be a method from <see cref="BinaryPrimitives"/></param>
        /// <param name="result">The result returned.</param>
        /// <returns>Returns true if the read was successful, else returns false.</returns>
        public unsafe bool TryRead<T>(ReadDelegate<T> read, out T result) where T : unmanaged
        {
            result = default;
            var size = sizeof(T);
            if (size > _maxStackalloc) return false;
            if (size > _input.Length) return false;

            if (_input.First.Length >= size)
                result = read(_input.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[size];
                _input.Slice(size).CopyTo(local);
                result = read(local);
            }

            _input = _input.Slice(size);
            return true;
        }

        /// <summary>
        /// Try to read a <see cref="T"/> with the <see cref="ReadDelegate{T}"/> specified as an arg.
        /// Then advance the current position according to the specified <see cref="size"/>.
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <param name="read">The delegate to read the <see cref="T"/>. Must be a method from <see cref="BinaryPrimitives"/></param>
        /// <param name="size">The size of the current value to read.</param>
        /// <param name="result">The result returned.</param>
        /// <returns>Returns true if the read was successful, else returns false.</returns>
        public bool TryRead<T>(ReadDelegate<T> read, int size, out T result)
        {
            result = default;
            if (size > _maxStackalloc) return false;
            if (size > _input.Length) return false;

            if (_input.First.Length >= size)
                result = read(_input.First.Span);
            else
            {
                Span<byte> local = stackalloc byte[size];
                _input.Slice(size).CopyTo(local);
                result = read(local);
            }

            _input = _input.Slice(size);
            return true;
        }
    }
}
