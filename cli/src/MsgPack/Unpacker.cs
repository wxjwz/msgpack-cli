﻿#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;

using MsgPack.Linq;

// TODO: Remove magic numbers related to MsgPack spec.

namespace MsgPack
{
	/// <summary>
	///		Implements deserializing feature of MsgPack.
	/// </summary>
	public partial class Unpacker : IEnumerable<MessagePackObject>, IDisposable
	{
		/// <summary>
		///		Default buffer size.
		/// </summary>
		/// <remarks>
		///		This value is subject to change.
		/// </remarks>
		public static readonly int DefaultBufferSize = 1024 * 64;

		/// <summary>
		///		Actual unpackaging strategy.
		/// </summary>
		private readonly StreamingUnpacker _unpacker = new StreamingUnpacker();

		/// <summary>
		///		If current position MAY be in tail of source then true, otherwise false.
		/// </summary>
		/// <remarks>
		///		This value should be refered via <see cref="IsInTailUltimately"/>.
		/// </remarks>
		private bool _mayInTail;

		/// <summary>
		///		Queue of successors of data source.
		/// </summary>
		private readonly Queue<DataSource> _successorSources = new Queue<DataSource>();

		/// <summary>
		///		Current data source.
		/// </summary>
		private DataSource _currentSource;

		/// <summary>
		///		Last unpacked data or null.
		/// </summary>
		private MessagePackObject? _data;

		/// <summary>
		///		Get last unpacked data.
		/// </summary>
		/// <value>Last unpacked data or null.</value>
		/// <remarks>
		///		If you use any of directory APIs (methods which return non-<see cref="MessagePackObject"/>), 
		///		then this property to be invalidated.
		/// </remarks>
		public MessagePackObject? Data
		{
			get { return this._data; }
		}

		/// <summary>
		///		Initialize new instance with default sized on memory buffer.
		/// </summary>
		public Unpacker() : this( new MemoryStream( DefaultBufferSize ), true ) { }

		/// <summary>
		///		Initialize new instance using specified <see cref="Stream"/> as source.
		///		This instance will have <see cref="Stream"/> ownership.
		/// </summary>
		/// <param name="source">Source <see cref="Stream"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
		public Unpacker( Stream source ) : this( source, true ) { }

		/// <summary>
		///		Initialize new instance using specified <see cref="Stream"/> as source.
		/// </summary>
		/// <param name="source">Source <see cref="Stream"/>.</param>
		/// <param name="ownsStream">If you want to dispose stream when this instance is disposed, then true.</param>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
		public Unpacker( Stream source, bool ownsStream )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( "source" );
			}

			Contract.EndContractBlock();

			this._currentSource = new DataSource( source, ownsStream );
		}

		/// <summary>
		///		Initialize new instance using specified <see cref="Byte"/>[] as source.
		/// </summary>
		/// <param name="initialData">Source <see cref="Byte"/>[].</param>
		/// <exception cref="ArgumentNullException"><paramref name="initialData"/> is null.</exception>
		public Unpacker( byte[] initialData ) : this( initialData, 0, initialData == null ? 0 : initialData.Length ) { }

		/// <summary>
		///		Initialize new instance using specified <see cref="Byte"/>[] as source.
		/// </summary>
		/// <param name="initialData">Source <see cref="Byte"/>[].</param>
		/// <param name="offset">Offset of <paramref name="initialData"/> to copy.</param>
		/// <param name="count">Count of <paramref name="initialData"/> to copy.</param>
		/// <exception cref="ArgumentNullException"><paramref name="initialData"/> is null.</exception>
		public Unpacker( byte[] initialData, int offset, int count )
		{
			Validation.ValidateBuffer( initialData, offset, count, "initialData", "count", true );

			Contract.EndContractBlock();

			this._currentSource = new DataSource( initialData );
		}

		/// <summary>
		///		Initialize new instance using specified <see cref="IEnumerable&lt;T&gt;">IEnumerable</see>&lt;<see cref="Byte"/>&gt; as source.
		/// </summary>
		/// <param name="source">Source <see cref="IEnumerable&lt;T&gt;">IEnumerable</see>&lt;<see cref="Byte"/>&gt;.</param>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
		public Unpacker( IEnumerable<byte> source )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( "source" );
			}

			Contract.EndContractBlock();

			this._currentSource = new DataSource( source );
		}

		private void VerifyNotDisposed()
		{
			if ( this._currentSource.Source == null )
			{
				throw new ObjectDisposedException( this.GetType().FullName );
			}
		}

		/// <summary>
		///		Clean up internal resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		///		Clean up internal resources.
		/// </summary>
		protected virtual void Dispose( bool disposing )
		{
			var source = this._currentSource;
			if ( source.Stream != null && source.OwnsStream )
			{
				source.Stream.Dispose();
				this._currentSource = default( DataSource );
			}

			foreach ( var successor in this._successorSources.ToArray() )
			{
				if ( successor.Stream != null && successor.OwnsStream )
				{
					successor.Stream.Dispose();
				}
			}

			this._successorSources.Clear();
		}

		/// <summary>
		///		Invalidate internal cache.
		/// </summary>
		private void InvalidateCache()
		{
			this._data = null;
		}

		/// <summary>
		///		Get <see cref="IEnumerator&lt;T&gt;"/> to enumerate <see cref="MessagePackObject"/> from source stream.
		/// </summary>
		/// <returns><see cref="IEnumerator&lt;T&gt;"/> to enumerate <see cref="MessagePackObject"/> from source stream.</returns>
		public IEnumerator<MessagePackObject> GetEnumerator()
		{
			while ( !this.IsInTailUltimately() )
			{
				this._data = this._unpacker.Unpack( this._currentSource.Source );
				if ( this._data != null )
				{
					yield return this._data.Value;
				}
				else
				{
					this._mayInTail = true;
				}
			}
		}

		/// <summary>
		///		Determins this instance is in tail of all data sources.
		///		This method deque successors when needed.
		/// </summary>
		/// <returns>If this instance is in tail of all data sources then true, otherwise false.</returns>
		private bool IsInTailUltimately()
		{
			if ( !this._mayInTail )
			{
				return false;
			}

			if ( this._successorSources.Count == 0 )
			{
				return true;
			}

			this._currentSource = this._successorSources.Dequeue();
			return false;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		/// <summary>
		///		Feed new data source.
		/// </summary>
		/// <param name="newData">New data source to feed.</param>
		/// <exception cref="ArgumentNullException"><paramref name="newData"/> is null.</exception>
		public void Feed( IEnumerable<byte> newData )
		{
			if ( newData == null )
			{
				throw new ArgumentNullException( "newData" );
			}

			Contract.EndContractBlock();

			this._successorSources.Enqueue( new DataSource( newData ) );
		}

		/// <summary>
		///		Feed new data source.
		/// </summary>
		/// <param name="stream">New data source to feed.</param>
		/// <param name="ownsStream">If <paramref name="stream"/> should be disposed in this instance then true.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public void Feed( Stream stream, bool ownsStream )
		{
			if ( stream == null )
			{
				throw new ArgumentNullException( "stream" );
			}

			Contract.EndContractBlock();

			this._successorSources.Enqueue( new DataSource( stream, ownsStream ) );
		}

		/// <summary>
		///		Unpack length of array from current buffer.
		/// </summary>
		/// <returns>Length of array. This is up to <see cref="UInt32.MaxValue"/>.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public long UnpackArrayLength()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackArrayLength( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack count of list from current buffer.
		/// </summary>
		/// <returns>Count of list. This is up to <see cref="UInt32.MaxValue"/>.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public long UnpackListCount()
		{
			return this.UnpackArrayLength();
		}

		/// <summary>
		///		Unpack count of map pairs from current buffer.
		/// </summary>
		/// <returns>Count of map pairs. This is up to <see cref="UInt32.MaxValue"/>.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public long UnpackMapCount()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackDictionaryCount( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack count of map pairs from current buffer.
		/// </summary>
		/// <returns>Count of map pairs. This is up to <see cref="UInt32.MaxValue"/>.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public long UnpackDictionaryCount()
		{
			return this.UnpackMapCount();
		}

		/// <summary>
		///		Unpack length of raw binary from current buffer.
		/// </summary>
		/// <returns>Length of raw binary. This is up to <see cref="UInt32.MaxValue"/>.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public long UnpackRawLength()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackRawLength( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack null from current buffer.
		/// </summary>
		/// <returns>null.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public Object UnpackNull()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackNull( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack null from current buffer.
		/// </summary>
		/// <returns>If value is null then true.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public Boolean TryUnpackNull()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.TryUnpackNull( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack <see cref="Boolean"/> from current buffer.
		/// </summary>
		/// <returns><see cref="Boolean"/> value.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public Boolean UnpackBoolean()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackBoolean( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack raw byte stream.
		/// </summary>
		/// <returns>Raw byte stream.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public IEnumerable<byte> UnpackRaw()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackRaw( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack raw byte stream as string using UTF-8 <see cref="Encoding"/>.
		/// </summary>
		/// <returns>String.</returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public String UnpackString()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackString( this._currentSource.Stream );
		}

		/// <summary>
		///		Unpack raw byte stream as string using specified <see cref="Encoding"/>.
		/// </summary>
		/// <param name="encoding"><see cref="Encoding"/> to be used.</param>
		/// <returns>String.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="encoding"/> is null.</exception>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <exception cref="MessageTypeException">Current value is not <see cref="Byte"/>.</exception>
		/// <remarks>
		///		This method is direct API, so <see cref="Data"/> will be invalidated.
		/// </remarks>
		public String UnpackString( Encoding encoding )
		{
			if ( encoding == null )
			{
				throw new ArgumentNullException( "encoding" );
			}

			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackString( this._currentSource.Stream, encoding );
		}

		/// <summary>
		///		Unpack <see cref="MessagePackObject"/> from current stream.
		/// </summary>
		/// <returns>
		///		<see cref="MessagePackObject"/>.
		///		If current stream does not contain enough bytes, so this value may be null.
		/// </returns>
		/// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
		/// <exception cref="UnpackException">Failed to unpack due to malformed or collapsed source.</exception>
		/// <remarks>
		///		This method is NOT direct API, so <see cref="Data"/> will NOT be invalidated.
		/// </remarks>
		public MessagePackObject? UnpackObject()
		{
			this.VerifyNotDisposed();
			Contract.EndContractBlock();

			this.InvalidateCache();
			return Unpacking.UnpackObject( this._currentSource.Source );
		}
		
		/// <summary>
		///		Encapselates Stream and ownership information.
		/// </summary>
		private struct DataSource
		{
			private IEnumerable<byte> _source;

			/// <summary>
			///		Get data source bytes as <see cref="IEnumerable&lt;T&gt;"/>.
			/// </summary>
			/// <value>Data source bytes as <see cref="IEnumerable&lt;T&gt;"/>.</value>
			public IEnumerable<byte> Source
			{
				get { return this._source ?? this.Stream.AsEnumerable(); }
			}

			/// <summary>
			///		Indicates whether this unpacker should <see cref="Dispose()"/> <see cref="Stream"/>.
			/// </summary>
			public readonly bool OwnsStream;

			/// <summary>
			///		Underlying stream of this source. This value could be null.
			/// </summary>
			public readonly Stream Stream;

			public DataSource( IEnumerable<byte> source )
			{
				this._source = source;
				this.Stream = null;
				this.OwnsStream = false;
			}

			public DataSource( Stream stream, bool ownsStream )
			{
				this.Stream = stream;
				this.OwnsStream = ownsStream;
				this._source = null;
			}
		}
	}
}