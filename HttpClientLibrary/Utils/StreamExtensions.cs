using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientLibrary
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Provides extension methods for the <see cref="Stream"/> class.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  static class StreamExtensions
  {
    /// <summary>
    /// Asynchronously reads the bytes from a source stream and writes them to a destination stream.
    /// </summary>
    /// <remarks>
    /// <para>Copying begins at the current position in <paramref name="stream"/>.</para>
    /// </remarks>
    /// <param name="stream">The source stream.</param>
    /// <param name="destination">The stream to which the contents of the source stream will be copied.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <para>If <paramref name="stream"/> is disposed.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> is disposed.</para>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <para>If <paramref name="stream"/> does not support reading.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> does not support writing.</para>
    /// </exception>
    public static Task CopyToAsync(this Stream stream, Stream destination)
    {
#if NET45PLUS
            if (stream == null)
                throw new ArgumentNullException("stream");

            // This code requires the `Stream` class provide an implementation of `CopyToAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.CopyToAsync(destination);
#else
      return CopyToAsync(stream, destination, 16 * 1024, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Asynchronously reads the bytes from a source stream and writes them to a destination stream,
    /// using a specified buffer size.
    /// </summary>
    /// <remarks>
    /// <para>Copying begins at the current position in <paramref name="stream"/>.</para>
    /// </remarks>
    /// <param name="stream">The source stream.</param>
    /// <param name="destination">The stream to which the contents of the source stream will be copied.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>If <paramref name="bufferSize"/> is negative or zero.</para>
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <para>If <paramref name="stream"/> is disposed.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> is disposed.</para>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <para>If <paramref name="stream"/> does not support reading.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> does not support writing.</para>
    /// </exception>
    public static Task CopyToAsync(this Stream stream, Stream destination, int bufferSize)
    {
#if NET45PLUS
            if (stream == null)
                throw new ArgumentNullException("stream");

            // This code requires the `Stream` class provide an implementation of `CopyToAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.CopyToAsync(destination, bufferSize);
#else
      return CopyToAsync(stream, destination, bufferSize, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Asynchronously reads the bytes from a source stream and writes them to a destination stream,
    /// using a specified buffer size and cancellation token.
    /// </summary>
    /// <remarks>
    /// <para>If the operation is canceled before it completes, the returned task contains the <see cref="TaskStatus.Canceled"/>
    /// value for the <see cref="Task.Status"/> property.</para>
    /// <para>
    /// Copying begins at the current position in <paramref name="stream"/>.
    /// </para>
    /// </remarks>
    /// <param name="stream">The source stream.</param>
    /// <param name="destination">The stream to which the contents of the source stream will be copied.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>If <paramref name="bufferSize"/> is negative or zero.</para>
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <para>If <paramref name="stream"/> is disposed.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> is disposed.</para>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <para>If <paramref name="stream"/> does not support reading.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="destination"/> does not support writing.</para>
    /// </exception>
    public static Task CopyToAsync(this Stream stream, Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (destination == null)
        throw new ArgumentNullException("destination");
      if (!stream.CanRead)
        throw new NotSupportedException("The stream does not support reading");
      if (!destination.CanWrite)
        throw new NotSupportedException("The destination does not support writing");
      if (bufferSize <= 0)
        throw new ArgumentOutOfRangeException("bufferSize");

      if (cancellationToken.IsCancellationRequested)
        return CompletedTask.Canceled();

#if NET45PLUS
            // This code requires the `Stream` class provide an implementation of `CopyToAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.CopyToAsync(destination, bufferSize, cancellationToken);
#else
      return CopyToAsync(stream, destination, new byte[bufferSize], cancellationToken);
#endif
    }

#if !NET45PLUS
    private static Task CopyToAsync(Stream stream, Stream destination, byte[] buffer, CancellationToken cancellationToken)
    {
      int nread = 0;
      Func<Task<bool>> condition =
          () => stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)
              .Select(task => (nread = task.Result) != 0);
      Func<Task> body = () => destination.WriteAsync(buffer, 0, nread, cancellationToken);

      return TaskBlocks.While(condition, body);
    }
#endif

    /// <summary>
    /// Asynchronously clears all buffers for a stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    /// <remarks>
    /// <para>If a derived class does not flush the buffer in its implementation of the <see cref="Stream.Flush()"/>
    /// method, the <see cref="FlushAsync(Stream)"/> method will not flush the buffer.</para>
    /// </remarks>
    /// <param name="stream">The stream to flush.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">If <paramref name="stream"/> has been disposed.</exception>
    public static Task FlushAsync(this Stream stream)
    {
#if NET45PLUS
            if (stream == null)
                throw new ArgumentNullException("stream");

            // This code requires the `Stream` class provide an implementation of `FlushAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.FlushAsync();
#else
      return FlushAsync(stream, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Asynchronously clears all buffers for a stream and causes any buffered data to be written to the underlying device,
    /// and monitors cancellation requests.
    /// </summary>
    /// <remarks>
    /// <para>If the operation is canceled before it completes, the returned task contains the <see cref="TaskStatus.Canceled"/>
    /// value for the <see cref="Task.Status"/> property.</para>
    /// <para>
    /// If a derived class does not flush the buffer in its implementation of the <see cref="Stream.Flush()"/> method,
    /// the <see cref="FlushAsync(Stream)"/> method will not flush the buffer.
    /// </para>
    /// </remarks>
    /// <param name="stream">The stream to flush.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">If <paramref name="stream"/> has been disposed.</exception>
    public static Task FlushAsync(this Stream stream, CancellationToken cancellationToken)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      if (cancellationToken.IsCancellationRequested)
        return CompletedTask.Canceled();

#if NET45PLUS
            // This code requires the `Stream` class provide an implementation of `FlushAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.FlushAsync(cancellationToken);
#else
      return Task.Factory.StartNew(state => ((Stream)state).Flush(), stream, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
#endif
    }

    /// <summary>
    /// Asynchronously reads a sequence of bytes from a stream and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <remarks>
    /// <para>Use the <see cref="Stream.CanRead"/> property to determine whether the stream instance supports
    /// reading.</para>
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    /// <param name="buffer">The buffer to write the data into.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data from the stream.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation. When the task completes successfully, the <see cref="Task{TResult}.Result"/>
    /// property contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if
    /// the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="buffer"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>If <paramref name="offset"/> is negative.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="count"/> is negative.</para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para>If the sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</para>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <para>If <paramref name="stream"/> does not support reading.</para>
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <para>If <paramref name="stream"/> has been disposed.</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>If <paramref name="stream"/> is currently in use by a previous read operation.</para>
    /// </exception>
    public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
#if NET45PLUS
            if (stream == null)
                throw new ArgumentNullException("stream");

            // This code requires the `Stream` class provide an implementation of `FlushAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.ReadAsync(buffer, offset, count);
#else
      return ReadAsync(stream, buffer, offset, count, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Asynchronously reads a sequence of bytes from a stream, advances the position within the stream by the number of bytes read,
    /// and monitors cancellation requests.
    /// </summary>
    /// <remarks>
    /// <para>Use the <see cref="Stream.CanRead"/> property to determine whether the stream instance supports
    /// reading.</para>
    /// <para>
    /// If the operation is canceled before it completes, the returned task contains the <see cref="TaskStatus.Canceled"/>
    /// value for the <see cref="Task.Status"/> property.
    /// </para>
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    /// <param name="buffer">The buffer to write the data into.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data from the stream.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation. When the task completes successfully, the <see cref="Task{TResult}.Result"/>
    /// property contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if
    /// the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="buffer"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>If <paramref name="offset"/> is negative.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="count"/> is negative.</para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para>If the sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</para>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <para>If <paramref name="stream"/> does not support reading.</para>
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <para>If <paramref name="stream"/> has been disposed.</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>If <paramref name="stream"/> is currently in use by a previous read operation.</para>
    /// </exception>
    public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      if (cancellationToken.IsCancellationRequested)
        return CompletedTask.Canceled<int>();

#if NET45PLUS
            // This code requires the `Stream` class provide an implementation of `ReadAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.ReadAsync(buffer, offset, count, cancellationToken);
#else
      return Task<int>.Factory.FromAsync(stream.BeginRead, stream.EndRead, buffer, offset, count, null);
#endif
    }

    /// <summary>
    /// Asynchronously writes a sequence of bytes to a stream and advances the position within the stream by the number of bytes written.
    /// </summary>
    /// <remarks>
    /// <para>Use the <see cref="Stream.CanWrite"/> property to determine whether the stream instance supports writing.</para>
    /// </remarks>
    /// <param name="stream">The stream to write data to.</param>
    /// <param name="buffer">The buffer to read the data from.</param>
    /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    /// <returns>
    /// A task that represents the asynchronous write operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="buffer"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>If <paramref name="offset"/> is negative.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="count"/> is negative.</para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para>If the sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</para>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <para>If <paramref name="stream"/> does not support writing.</para>
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <para>If <paramref name="stream"/> has been disposed.</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>If <paramref name="stream"/> is currently in use by a previous write operation.</para>
    /// </exception>
    public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
#if NET45PLUS
            if (stream == null)
                throw new ArgumentNullException("stream");

            // This code requires the `Stream` class provide an implementation of `WriteAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.WriteAsync(buffer, offset, count);
#else
      return WriteAsync(stream, buffer, offset, count, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Asynchronously writes a sequence of bytes to a stream, advances the position within the stream by the number of bytes written,
    /// and monitors cancellation requests.
    /// </summary>
    /// <remarks>
    /// <para>Use the <see cref="Stream.CanWrite"/> property to determine whether the stream instance supports writing.</para>
    /// <para>
    /// If the operation is canceled before it completes, the returned task contains the <see cref="TaskStatus.Canceled"/>
    /// value for the <see cref="Task.Status"/> property.
    /// </para>
    /// </remarks>
    /// <param name="stream">The stream to write data to.</param>
    /// <param name="buffer">The buffer to read the data from.</param>
    /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous write operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="buffer"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>If <paramref name="offset"/> is negative.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="count"/> is negative.</para>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <para>If the sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.</para>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <para>If <paramref name="stream"/> does not support writing.</para>
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <para>If <paramref name="stream"/> has been disposed.</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>If <paramref name="stream"/> is currently in use by a previous write operation.</para>
    /// </exception>
    public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      if (cancellationToken.IsCancellationRequested)
        return CompletedTask.Canceled();

#if NET45PLUS
            // This code requires the `Stream` class provide an implementation of `WriteAsync`. The unit tests will
            // detect any case where this results in a stack overflow.
            return stream.WriteAsync(buffer, offset, count, cancellationToken);
#else
      return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, buffer, offset, count, null);
#endif
    }
  }
}
