![Logo](https://sun9-14.userapi.com/impg/dSaWIDHHbQndWkUYYXShw_B4E7dfdNV5-Dp67g/WqCvkMoq5q0.jpg?size=800x200&quality=96&sign=1eac232a01a06a4d5e9b7656b4306dfd&type=album)


## Usage

### Object pool
`RStream` and `WStream` have their own object pools.
This library provides some classes for object pools: 
`LoadObjectPool` and `PooledObject`.
If you want to make object pool for your class, you should inherit
`PooledObject<T>`. 
Also, if you need to do something right before `Get()` method, you should
inherit `ILoadOnGet` interface.

### RStream
Object for reading binary serialized data from some buffer
![RStream](https://sun9-39.userapi.com/impg/MLVKKxjI7nGmrZ8syqx1RPXb2ILCAEGwLIawlw/afOuQqziNmY.jpg?size=800x200&quality=96&sign=050301fc0dc1aa04066a3d51721b2fef&type=album)

```c#
using var r = RStream.Get();

// ...reading data into internal buffer of stream
// you can get this internal buffer using r.Buffer

var someNumber = r.ReadSignedIntLittleEndian();
var str = r.ReadString();
```

As you can see, you should use `using` keyword.
It's because this is pooled object. Right after 
function call object will be returned to the pool.

Also you can use 
```c#
using(var r = RStream.Get()){
// ...
}
```
There RStream will be returned after block execution.

```c#
// BIG endian by default
r.ReadSignedIntLittleEndian() // Read signed int32 LITTLE endian
r.ReadUnsignedLongLittleEndian() // Read unsigned long LITTLE endian 
r.ReadSignedShort() // Read signed short BIG endian
r.ReadTriad() // Read triad (3 bytes) BIG endian
r.ReadUnsignedByte() // Read unsigned byte

r.ReadByteSizedString() // 1 byte used for length (string can contain max 255 bytes)
r.ReadString() // VarInt used for length (string can contain more than 255 bytes)
```

#### Reading byte arrays with no allocations
We recommend to use [`RecyclableMemoryStreamManager`](https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream) for this purpose.
Example:
```c#
// of course you shouldn't allocate manager in local variable
var manager = new RecyclableMemoryStreamManager();
...
using var r = RStream.Get();
// ...reading to r.Buffer

using var stream = manager.GetStream();
r.ReadByteArrayInto(stream);
// now stream has bytes
```

### WStream
Object for writing binary serialized data
![WStream](https://sun9-10.userapi.com/impg/uLQXH6jIkgEU97200JRk4w4dRcCJ8UQ0QjA1ug/CymP8bJ1FNE.jpg?size=800x200&quality=96&sign=6cce4d8ef5359fdce532d02ad0e3dfd5&type=album)

```c#
using var w = WStream.Get();

w.WriteSignedInt(426);

var buffer = w.Buffer;
// ...sending buffer to client or saving to file
```

```c#
// BIG endian by default
r.WriteSignedIntLittleEndian(-6) // Write signed int32 LITTLE endian
r.WriteUnsignedLongLittleEndian(0xAABBCCDDEE) // Write unsigned long LITTLE endian 
r.WriteSignedShort(-0x7FAA) // Write signed short BIG endian
r.WriteTriad(0x7FBBCC) // Write triad (3 bytes) BIG endian
r.WriteUnsignedByte(0xFF) // Write unsigned byte

r.WriteByteSizedString("aquaminer") // 1 byte used for length (string can contain max 255 bytes)
r.WriteString("NolikTop") // VarInt used for length (string can contain more than 255 bytes)
```