using System.IO;
using System.Runtime.Serialization;
using WolvenKit.RED3.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED3.CR2W.Types.Enums;


namespace WolvenKit.RED3.CR2W.Types
{
	[DataContract(Namespace = "")]
	[REDMeta]
	public class CR4LootContainerParam : CVariable
	{
		[Ordinal(1)] [RED("name")] 		public CName Name { get; set;}

		[Ordinal(2)] [RED("chance")] 		public CFloat Chance { get; set;}

		[Ordinal(3)] [RED("respawnTime")] 		public CUInt32 RespawnTime { get; set;}

		public CR4LootContainerParam(IRed3EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name){ }

		

		public override void Read(BinaryReader file, uint size) => base.Read(file, size);

		public override void Write(BinaryWriter file) => base.Write(file);

	}
}