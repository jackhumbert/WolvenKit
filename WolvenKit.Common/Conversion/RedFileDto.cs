using System.Collections.Generic;
using System.IO;
using System.Linq;
using WolvenKit.Common.Model;
using WolvenKit.Common.Tools;
using WolvenKit.RED4.CR2W;
using WolvenKit.Common.Oodle;
using Splat;

namespace WolvenKit.Common.Conversion
{
    public class RedFileDto
    {
        public const string Magic = "w2rc";

        public Dictionary<int,RedExportDto> Chunks { get; set; } = new();

        public Dictionary<int, RedExportDto> Buffers { get; set; } = new();

        public bool ShouldSerializeBuffers() => Buffers is { Count: > 0 };

        public RedFileDto()
        {

        }

        public RedFileDto(IWolvenkitFile cr2w)
        {
            Chunks = cr2w.Chunks.ToDictionary(_ => _.ChunkIndex, _ => new RedExportDto(_));
            foreach (var buffer in cr2w.Buffers)
            {
                var data = ((CR2WBufferWrapper)buffer).GetData();

                using var uncompressedMS = new MemoryStream();
                using (var compressedMs = new MemoryStream(data))
                {
                    OodleHelper.DecompressAndCopySegment(compressedMs, uncompressedMS, buffer.DiskSize, buffer.MemSize);
                }

                // try reading as normal cr2w file
                var _parser = Locator.Current.GetService<Red4ParserService>();
                var cr2wbuffer = _parser.TryReadCr2WFile(uncompressedMS);
                if (cr2wbuffer != null)
                {
                    foreach (var _ in cr2wbuffer.Chunks)
                    {
                        Buffers.Add(_.ChunkIndex, new RedExportDto(_));
                    }
                }
                // try reading as compiled package
                else
                {
                    uncompressedMS.Seek(0, SeekOrigin.Begin);
                    var compiledPackage = _parser.TryReadCompiledPackage(uncompressedMS);
                    if (compiledPackage != null)
                    {
                        foreach (var _ in compiledPackage.Chunks)
                        {
                            Buffers.Add(_.ChunkIndex, new RedExportDto(_));
                        }
                    }
                }
            }
        }

        public CR2WFile ToW2rc()
        {
            var cr2w = new CR2WFile();
            // chunks
            // order so that parent chunks get created first
            var groupedChunks = Chunks.GroupBy(_ => _.Value.ParentIndex);
            foreach (IGrouping<int, KeyValuePair<int, RedExportDto>> groupedChunk in groupedChunks)
            {
                foreach (var (chunkIndex, chunk) in groupedChunk.OrderBy(_ => _.Key))
                {
                    chunk.CreateChunkInFile(cr2w, chunkIndex);
                }
            }
            var last_key = cr2w.Chunks.Count();
            // buffers
            // might be naughty. maybe it works?
            // we might need remove the compiledData field in the previous set of chunks
            var groupedBuffers = Buffers.GroupBy(_ => _.Value.ParentIndex);
            foreach (IGrouping<int, KeyValuePair<int, RedExportDto>> groupedBuffer in groupedBuffers)
            {
                foreach (var (bufferIndex, buffer) in groupedBuffer.OrderBy(_ => _.Key))
                {
                    buffer.CreateChunkInFile(cr2w, bufferIndex + last_key);
                }
            }

            return cr2w;
        }
    }
}
