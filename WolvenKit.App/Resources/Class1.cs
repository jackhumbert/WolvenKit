using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Shaders;
using System.IO;
using System.Reflection;

namespace WolvenKit.Resources.Shaders
{

    public static class CustomShaderNames
    {
        public static readonly string DataSampling = "DataSampling";
        public static readonly string NoiseMesh = "NoiseMesh";
        public static readonly string TexData = "texData";
        public static readonly string TexDataSampler = "texDataSampler";
    }
    public static class ShaderHelper
    {
        public static byte[] LoadShaderCode(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                throw new ArgumentException($"Shader File not found: {path}");
            }
        }
    }

    /// <summary>
    /// Build using Nuget Micorsoft.HLSL.Microsoft.HLSL.CSharpVB automatically during project build
    /// </summary>
    public static class CustomVSShaderDescription
    {
        public static byte[] VSMeshDataSamplerByteCode
        {
            get
            {
                return ShaderHelper.LoadShaderCode(@"Shaders\vsMeshDataSampling.cso");
            }
        }
        public static ShaderDescription VSDataSampling = new ShaderDescription(nameof(VSDataSampling), ShaderStage.Vertex,
            new ShaderReflector(), VSMeshDataSamplerByteCode);
    }
    /// <summary>
    /// Build using Nuget Micorsoft.HLSL.Microsoft.HLSL.CSharpVB automatically during project build
    /// </summary>
    public static class CustomPSShaderDescription
    {
        public static ShaderDescription PSDataSampling = new ShaderDescription(nameof(PSDataSampling), ShaderStage.Pixel,
            new ShaderReflector(), ShaderHelper.LoadShaderCode(@"Shaders\psMeshDataSampling.cso"));

        public static ShaderDescription PSNoiseMesh = new ShaderDescription(nameof(PSNoiseMesh), ShaderStage.Pixel,
            new ShaderReflector(), ShaderHelper.LoadShaderCode(@"Shaders\psMeshNoiseBlinnPhong.cso"));


        public static ShaderDescription PSCustomPoint = new ShaderDescription(nameof(PSCustomPoint), ShaderStage.Pixel,
            new ShaderReflector(), ShaderHelper.LoadShaderCode(@"Shaders\psCustomPoint.cso"));
    }


    public class RedEffectsManager : DefaultEffectsManager
    {
        public RedEffectsManager()
        {
            LoadCustomTechniqueDescriptions();
        }

        private void LoadCustomTechniqueDescriptions()
        {
            var dataSampling = new TechniqueDescription(CustomShaderNames.DataSampling)
            {
                InputLayoutDescription = new InputLayoutDescription(CustomVSShaderDescription.VSMeshDataSamplerByteCode, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.ColorStripe1D)
                    {
                        ShaderList = new[]
                        {
                            CustomVSShaderDescription.VSDataSampling,
                            //DefaultVSShaderDescriptions.VSMeshDefault,
                            CustomPSShaderDescription.PSDataSampling
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            CustomVSShaderDescription.VSDataSampling,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };
            var noiseMesh = new TechniqueDescription(CustomShaderNames.NoiseMesh)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            CustomPSShaderDescription.PSNoiseMesh
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };

            AddTechnique(dataSampling);
            AddTechnique(noiseMesh);

            var points = GetTechnique(DefaultRenderTechniqueNames.Points);
            points.AddPass(new ShaderPassDescription("CustomPointPass")
            {
                ShaderList = new[]
                {
                    DefaultVSShaderDescriptions.VSPoint,
                    DefaultGSShaderDescriptions.GSPoint,
                    CustomPSShaderDescription.PSCustomPoint
                },
                BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual

            });
        }
    }

    //public static class RedByteCodes
    //{
    //    public static string vehicle_glass
    //    {
    //        get;
    //    } = "vehicle_glass";
    //}

    //public static class RedShader
    //{
    //    public static readonly ShaderDescription vehicle_glass =
    //        new ShaderDescription(nameof(vehicle_glass), ShaderStage.Pixel, new ShaderReflector(),
    //            RedByteCodes.vehicle_glass);
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public interface IShaderByteCodeReader
    //{
    //    byte[] Read(string name);
    //}

    ///// <summary>
    ///// Used to read HelixToolkit internal default shader byte codes
    ///// </summary>
    //public sealed class RedByteCodeReader : IShaderByteCodeReader
    //{
    //    public byte[] Read(string name)
    //    {
    //        var assembly = typeof(RedShaderBytePool).GetTypeInfo().Assembly;
    //        Stream shaderStream = assembly.GetManifestResourceStream($"HelixToolkit.SharpDX.Core.Resources.{name}.cso");
    //        if (shaderStream == null)
    //        {
    //            throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
    //        }
    //        using (var memory = new MemoryStream())
    //        {
    //            shaderStream.CopyTo(memory);
    //            return memory.ToArray();
    //        }
    //    }
    //}

    ///// <summary>
    ///// Used to read shader bytecode
    ///// </summary>
    //public static class RedShaderBytePool
    //{
    //    public static Dictionary<string, byte[]> Dict = new Dictionary<string, byte[]>();
    //    internal static readonly IShaderByteCodeReader InternalByteCodeReader = new RedByteCodeReader();
    //    public static byte[] Read(string name, IShaderByteCodeReader reader = null)
    //    {
    //        lock (Dict)
    //        {
    //            if (!Dict.TryGetValue(name, out var byteCode))
    //            {
    //                lock (Dict)
    //                {
    //                    if (!Dict.TryGetValue(name, out byteCode))
    //                    {
    //                        if (reader == null)
    //                        {
    //                            byteCode = InternalByteCodeReader.Read(name);
    //                        }
    //                        else
    //                        {
    //                            byteCode = reader.Read(name);
    //                        }
    //                        Dict.Add(name, byteCode);
    //                    }
    //                }
    //            }
    //            return byteCode;
    //        }
    //    }
    //}
}
