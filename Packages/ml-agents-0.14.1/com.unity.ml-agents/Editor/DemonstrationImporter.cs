using System;
using System.IO;
using MLAgents.CommunicatorObjects;
using UnityEditor;
using UnityEngine;


namespace MLAgents
{
    /// <summary>
    /// Asset Importer used to parse demonstration files.
    /// </summary>
    [UnityEditor.AssetImporters.ScriptedImporter(1, new[] {"demo"})]
    public class DemonstrationImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        const string k_IconPath = "Assets/ML-Agents/Resources/DemoIcon.png";

        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            var inputType = Path.GetExtension(ctx.assetPath);
            if (inputType == null)
            {
                throw new Exception("Demonstration import error.");
            }

            try
            {
                // Read first two proto objects containing metadata and brain parameters.
                Stream reader = File.OpenRead(ctx.assetPath);

                var metaDataProto = DemonstrationMetaProto.Parser.ParseDelimitedFrom(reader);
                var metaData = metaDataProto.ToDemonstrationMetaData();

                reader.Seek(DemonstrationStore.MetaDataBytes + 1, 0);
                var brainParamsProto = BrainParametersProto.Parser.ParseDelimitedFrom(reader);
                var brainParameters = brainParamsProto.ToBrainParameters();

                reader.Close();

                var demonstration = ScriptableObject.CreateInstance<Demonstration>();
                demonstration.Initialize(brainParameters, metaData);
                userData = demonstration.ToString();

                var texture = (Texture2D)
                    AssetDatabase.LoadAssetAtPath(k_IconPath, typeof(Texture2D));

                ctx.AddObjectToAsset(ctx.assetPath, demonstration, texture);
                ctx.SetMainObject(demonstration);
            }
            catch
            {
                // ignored
            }
        }
    }
}
