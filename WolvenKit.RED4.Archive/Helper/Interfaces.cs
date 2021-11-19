using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using WolvenKit.RED4.Archive.CR2W;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.Types;

namespace WolvenKit.RED4.Archive
{
    public interface ICR2WProperty
    {
    }

    public interface ICR2WImport : IRedImport
    {
    }

    public interface ICR2WName
    {
        public string Str { get; }
        public uint hash { get; }
    }

    public interface ICR2WBuffer : IRedBuffer
    {
        public uint Flags { get; set; }

        public bool IsCompressed { get; set; }
    }
    
    public interface ICR2WExport
    {

        public string REDType { get; }

        public int ParentChunkIndex { get; }

        public IRedType Data { get; }

        [JsonIgnore] public string REDName { get; }
        [JsonIgnore] public int ChunkIndex { get; }

        [JsonIgnore] public IRedType UnknownBytes { get; }

        [JsonIgnore] public ICR2WExport ParentChunk { get; set; }
        [JsonIgnore] public ICR2WExport VirtualParentChunk { get; set; }
        [JsonIgnore] public List<ICR2WExport> ChildrenChunks { get; }
        [JsonIgnore] public List<ICR2WExport> VirtualChildrenChunks { get; }
        [JsonIgnore] public List<IREDChunkPtr> AdReferences { get; }
        [JsonIgnore] public List<IREDChunkPtr> AbReferences { get; }
        [JsonIgnore] public List<string> UnknownTypes { get; }



        public void CreateDefaultData(IRedType cvar = null);
        public string GetFullChunkTypeDependencyString();
        public void MountChunkVirtually(int virtualparentchunkindex, bool force = false);
        public void MountChunkVirtually(ICR2WExport virtualparentchunk, bool force = false);

        public void ReadData(BinaryReader file);
        public void WriteData(BinaryWriter file);

        public uint GetOffset();
    }

    public interface ICR2WEmbeddedFile
    {
        public string FileName { get; set; }
        public IRedClass Content { get; set; }
    }

    //public interface IEditableVariable
    //{
    //    bool IsSerialized { get; set; }

    //    [JsonIgnore] int VarChunkIndex { get; set; }

    //    void Read(CR2WReader file, uint size);
    //}
}
