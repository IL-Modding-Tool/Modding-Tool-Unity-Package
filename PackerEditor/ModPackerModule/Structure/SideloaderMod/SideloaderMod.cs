using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using hooh_ModdingTool.asm_Packer.Utility;
using JetBrains.Annotations;
using ModPackerModule.Structure.BundleData;
using ModPackerModule.Structure.Classes.ManifestData;
using ModPackerModule.Structure.ListData;
using ModPackerModule.Structure.SideloaderMod.Data;
using ModPackerModule.Utility;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace ModPackerModule.Structure.SideloaderMod
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TargetGame
    {
        HS = 0, PH = 1, KK = 2, AI = 3, HS2 = 4,
        KKS = 5
    }

    public enum AssetType { Map, ScriptableObject, Animation, Prefab, Other }

    [Serializable]
    public struct Issue
    {
        public enum IssueLevel { Fatal, Error, Warning, Info }

        public enum IssueType
        {
            XMLFormat, System, MainInfo, BuildTarget, AssetBundleReference,
            Policy
        }

        public string Message;
        public string Suggest;
        public IssueLevel Level;
        public IssueType Type;

        public Issue(IssueLevel level, IssueType type, string message, string suggest = "")
        {
            Message = message;
            Suggest = suggest;
            Level = level;
            Type = type;
        }
    }

    // Use this data to make new archive.
    public partial class SideloaderMod : ScriptableObject
    {
        // should i separate this?
        private const string BuildTargetName = "build";
        private const string BundleTargetName = "bundles";
        private Dictionary<string, int> _autoPathIndex;
        private List<BundleBase> _bundleTargets;
        private List<IManifestData> _manifestData;
        private string _failedReason = "";
        private string _outputFileName;
        public AssetInfo Assets;
        public Data.CharacterInfo CharacterInfo;
        public DependencyLoaderData DependencyData;
        public Dictionary<string, GameInfo> GameItems;
        public GameMapInfo GameMapInfo;
        public int indexOffset = 0; // for sideloader local slots.
        public List<Issue> Issues;
        public MainData MainData;
        public StudioInfo StudioInfo;
        public string lastBuildVersion;
        public string lastBuildTime;

        // Generate root document when constructor is there.
        public SideloaderMod([NotNull] TextAsset file)
        {
            LoadFromTextAsset(file);
        }

        public void LoadFromContext(string assetPath)
        {
            SetContextFolders(assetPath);
            Initialize(Path.GetFileName(assetPath), File.ReadAllText(
                Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, assetPath)
                    .ToUnixPath()
            ));
        }

        public void LoadFromTextAsset(TextAsset file)
        {
            // check parse error
            var assetPath = AssetDatabase.GetAssetPath(file);
            SetContextFolders(assetPath);
            Initialize(file.name, file.text);
        }

        public void SetContextFolders(string path)
        {
            AssetFolder = Path.GetDirectoryName(path) ?? "";
            AssetDirectory = Path.Combine(Directory.GetCurrentDirectory(), AssetFolder).Replace("\\", "/");
        }

        public void Initialize(string filename, string content = "")
        {
            indexOffset = 0;
            _autoPathIndex = new Dictionary<string, int>();
            _bundleTargets = new List<BundleBase>();
            OutputDocumentObject = XmlUtils.GetManifestTemplate();
            Assets = new AssetInfo();
            CharacterInfo = new Data.CharacterInfo();
            DependencyData = new DependencyLoaderData();
            FileName = filename;
            GameItems = new Dictionary<string, GameInfo>();
            GameMapInfo = new GameMapInfo();
            Issues = new List<Issue>();
            MainData = new MainData();
            StudioInfo = new StudioInfo();
            try
            {
                InputDocumentObject = XDocument.Parse(content);
                // Strange dependency sturcture... must unfuck this mess.
                MainData.ParseData(this, InputDocumentObject.Root);
                DependencyData.ParseData(this, InputDocumentObject.Root);

                _manifestData = new List<IManifestData>
                {
                    // it should be in order, lmk if there is good way to do. I'm always learning.
                    MainData,
                    DependencyData,
                    new HeelsData(),
                    new MaterialEditorData(),
                    new AIMapData()
                };


                ParseDocument(InputDocumentObject.Root);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
            }
        }

        private string FileName { get; set; }

        public void RememberAsset(string path, string bundle, string target)
        {
            var assetType = AssetInfo.GetAssetType(path);

            Assets.RememberAsset(assetType, path, bundle, target);

            switch (assetType)
            {
                case AssetType.ScriptableObject:
                    GameMapInfo.ParseMapAsset(path);
                    break;
                case AssetType.Map:
                    break;
                case AssetType.Animation:
                    break;
                case AssetType.Prefab:
                    break;
                case AssetType.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int GetAutoPathIndex(string key)
        {
            if (!_autoPathIndex.ContainsKey(key)) _autoPathIndex[key] = 0;
            return _autoPathIndex[key]++;
        }

        public bool IsValid()
        {
            var root = InputDocumentObject.Root;
            if (ReferenceEquals(null, root))
            {
                Issues.Add(new Issue(Issue.IssueLevel.Fatal, Issue.IssueType.XMLFormat,
                    "Cannot find document root from the file", "Make sure that the file contains corrent XML data."));
                return false;
            }

            if (ReferenceEquals(null, root.Element(BundleTargetName)))
            {
                Issues.Add(new Issue(Issue.IssueLevel.Fatal, Issue.IssueType.XMLFormat,
                    "Cannot find asset bundle info in the file",
                    "Make sure that the file contains bundle data inside of <bundles> XML tag."));
                return false;
            }

            if (ReferenceEquals(null, root.Element(BuildTargetName)))
            {
                Issues.Add(new Issue(Issue.IssueLevel.Fatal, Issue.IssueType.XMLFormat,
                    "Cannot find build target data from the file",
                    "Make sure that the file contains mod data inside of <target> XML tag."));
                return false;
            }

            return true;
        }

        private void ParseDocument(in XElement rootNode)
        {
            if (!IsValid()) throw new InvalidDataException(_failedReason);
            var lastBuild = rootNode.Element("last-build");
            lastBuildTime = lastBuild?.Attribute("time")?.Value;
            lastBuildVersion = lastBuild?.Attribute("version")?.Value;
            ParseBundleTargets(rootNode?.Element(BundleTargetName)); // parse bundle data is done
            foreach (var target in _bundleTargets.Where(x => x.AutoPath)) target.ResolveAutoPath();
            ParseListData(rootNode?.Element(BuildTargetName)); // parse list data
            ParseManifest(rootNode);
            ValidateBundleData();
            ValidateListData();
            if (_outputFileName.IsNullOrEmpty())
                _outputFileName = !MainData.uniqueId.IsNullOrEmpty()
                    ? MainData.uniqueId
                    : MainData.guid.SanitizeNonCharacters();
        }

        private void ParseManifest(in XElement document)
        {
            foreach (var manifestData in _manifestData)
            {
                if (!(manifestData is DependencyLoaderData) && !(manifestData is MainData))
                    manifestData.ParseData(this, document);

                var (valid, reason) = manifestData.IsValid();
                if (valid) manifestData.SaveData(ref OutputDocumentObject);
                else throw new InvalidDataException(reason);
            }
        }

        private List<AssetBundleBuild> GetAssetBundleBuilds() =>
            _bundleTargets.SelectMany(target => target.Bundles).ToList();

        private IEnumerable<CopyFiles> GetCopyTargets(string target = AssetInfo.BundleTargetDefault) =>
            _bundleTargets.OfType<CopyFiles>().Where(x => x.Target == target).ToArray();

        private void ParseBundleTargets(in XElement document)
        {
            foreach (var element in document.Elements())
            {
                var name = element.Name.LocalName;
                switch (name)
                {
                    case "bundle":
                        _bundleTargets.Add(new AssetList(this, element).InitializeBundles(element));
                        break;
                    case "each":
                        _bundleTargets.Add(new EachBundles(this, element).InitializeBundles(element));
                        break;
                    case "folder":
                        _bundleTargets.Add(new FolderBundle(this, element).InitializeBundles(element));
                        break;
                    case "move":
                    case "copy":
                        _bundleTargets.Add(new CopyFiles(this, element).InitializeBundles(element));
                        break;
                }
            }
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private void RegisterItems(in XElement element, string type)
        {
            var target = element.Attr("target", AssetInfo.BundleTargetDefault);
            Assets.RememberTarget(target);
            var dictItemList = GameItems.ContainsKey(target) ? GameItems[target] : new GameInfo(target);

            foreach (var item in element.Elements("item"))
                switch (type)
                {
                    // TODO: bruh
                    case "anime":
                    case "animation":
                        dictItemList.Insert(typeof(ListAnimation), new ListAnimation(this, item));
                        break;
                    case "studioitem":
                    case "props":
                        dictItemList.Insert(typeof(ListStudioItem), new ListStudioItem(this, item));
                        break;
                    case "bigcategory":
                        dictItemList.Insert(typeof(ListIdBigCategory), new ListIdBigCategory(this, item));
                        break;
                    case "midcategory":
                        dictItemList.Insert(typeof(ListIdMidCategory), new ListIdMidCategory(this, item));
                        break;
                    case "animebigcategory":
                        dictItemList.Insert(typeof(ListIdBigAnimeCategory), new ListIdBigAnimeCategory(this, item));
                        break;
                    case "animemidcategory":
                        dictItemList.Insert(typeof(ListIdMidAnimeCategory), new ListIdMidAnimeCategory(this, item));
                        break;
                    case "map":
                    case "scene":
                        dictItemList.Insert(typeof(ListMap), new ListMap(this, item));
                        break;
                    case "hairback":
                    case "hairfront":
                    case "hairside":
                    case "hairext":
                        dictItemList.Insert(typeof(ListHair), new ListHair(this, item, type));
                        break;
                    case "acchead":
                        dictItemList.Insert(typeof(ListAccessoryHead), new ListAccessoryHead(this, item, type));
                        break;
                    case "accnone":
                    case "accear":
                    case "accglasses":
                    case "accface":
                    case "accneck":
                    case "accshoulder":
                    case "accchest":
                    case "accwaist":
                    case "accback":
                    case "accarm":
                    case "acchand":
                    case "accleg":
                    case "acckokan":
                        dictItemList.Insert(typeof(ListAccessory), new ListAccessory(this, item, type));
                        break;
                    case "spattern":
                    case "sunderhair":
                    case "snip":
                    case "seye_hl":
                    case "seyeblack":
                    case "seye":
                    case "seyelash":
                    case "seyebrow":
                    case "fsunburn":
                    case "fdetailf":
                    case "msunburn":
                    case "mdetailf":
                    case "fskinb":
                    case "mskinb":
                    case "mbeard":
                        dictItemList.Insert(typeof(ListSkinDiffuse), new ListSkinDiffuse(this, item, type));
                        break;
                    case "smole":
                    case "slip":
                    case "scheek":
                    case "spaint":
                    case "seyeshadow":
                        dictItemList.Insert(typeof(ListSkinGloss), new ListSkinGloss(this, item, type));
                        break;
                    case "fsocks":
                    case "fshoes":
                    case "fgloves":
                    case "mgloves":
                    case "mshoes":
                        dictItemList.Insert(typeof(ListClothing), new ListClothing(this, item, type));
                        break;
                    case "fpanst":
                    case "finbottom":
                        dictItemList.Insert(typeof(ListClothingKokan), new ListClothingKokan(this, item, type));
                        break;
                    case "fintop":
                        dictItemList.Insert(typeof(ListClothingInnerTop), new ListClothingInnerTop(this, item, type));
                        break;
                    case "ftop":
                    case "mtop":
                        dictItemList.Insert(typeof(ListClothingTop), new ListClothingTop(this, item, type));
                        break;
                    case "fbottom":
                    case "mbottom":
                        dictItemList.Insert(typeof(ListClothingBottom), new ListClothingBottom(this, item, type));
                        break;
                    case "fhead":
                    case "mhead":
                        dictItemList.Insert(typeof(ListHead), new ListHead(this, item, type));
                        break;
                    case "fdetailb":
                    case "mdetailb":
                        dictItemList.Insert(typeof(ListSkinDetail), new ListSkinDetail(this, item, type));
                        break;
                    case "fskinf":
                    case "mskinf":
                        dictItemList.Insert(typeof(ListSkinFace), new ListSkinFace(this, item, type));
                        break;
                    default:
                        throw new Exception($"\"{type}\" is a not valid category.");
                }

            GameItems[target] = dictItemList;
        }

        private void ParseListData(in XElement document)
        {
            _outputFileName = document.Attr("name");

            foreach (var element in document.Elements())
                RegisterItems(element, element.Attr<string>("type"));
        }
    }
}