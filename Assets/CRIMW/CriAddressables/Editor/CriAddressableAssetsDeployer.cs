/****************************************************************************
 *
 * Copyright (c) 2022 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

/**
 * \addtogroup CRIADDON_ADDRESSABLES_INTEGRATION
 * @{
 */

#if CRI_USE_ADDRESSABLES

using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;

namespace CriWare.Assets
{
	/**
	 * <summary>CRI Addressables のデータデプロイを行うエディタクラス</summary>
	 */
	public static class CriAddressableAssetsDeployer
	{
		public static event System.Action OnComplete;

		[InitializeOnLoadMethod]
		static void RegisterHook()
		{
#pragma warning disable 0612
			BuildScript.buildCompleted += (AddressableAssetBuildResult result) => {
				if (EditorApplication.isPlayingOrWillChangePlaymode) return;
				Deploy();
			};
#pragma warning restore 0612

			ContentPipeline.BuildCallbacks.PostWritingCallback += (param, deps, data, result) => {
				// only addressable build.
				if (!(param is UnityEditor.AddressableAssets.Build.DataBuilders.AddressableAssetsBundleBuildParameters))
					return ReturnCode.Success;
				var buildResult = result as IBundleBuildResults;
				if (buildResult == null) return ReturnCode.Success;
				return DeployAsWriteBundle(buildResult);
			};
		}

		/**
		 * <summary>CRI Addressables 向けのデータデプロイ (非推奨)</summary>
		 * <remarks>
		 * <para header='説明'>本APIは削除予定の非推奨APIです。<br/>
		 * Addressablesバンドルビルド内で独自処理を行うため、本APIを呼び出す必要はありません。</para>
		 * </remarks>
		 */
		[System.Obsolete]
		public static void Deploy()
		{
			if (CriAddressablesSetting.Mode != CriAddressablesSetting.DeployMode.UseAnchorAsset) return;
			DeployLocalData();
#if !CRI_ADDRESSABLES_DISABLE_LEGACY_DEPLOY
			DeployRemoteData();
#endif
			OnComplete?.Invoke();
		}

		[System.Obsolete]
		public static void DeployLocalData()
		{
			var addressablesBuildPath = CriAddressablesSetting.Instance.Local.buildPath.GetValue(AddressableAssetSettingsDefaultObject.Settings);

			try
			{
				EditorUtility.DisplayProgressBar("[CRIWARE][Addressables] Collectiong dependencies for local bundles", "", 0);

				var assets = CriAddressablesEditor.CollectDependentAssets<CriLocalAddressablesAssetImpl>().ToList();

				if (assets.Count <= 0) return;

				var count = 0;
				foreach (var asset in assets)
				{
					count++;
					EditorUtility.DisplayProgressBar("[CRIWARE][Addressables] Deploy data for local bundles", $"{count} / {assets.Count} assets", (float)count / assets.Count);
					var srcPath = AssetDatabase.GetAssetPath(asset);
					var dstPath = Path.Combine(addressablesBuildPath, (asset.Implementation as CriLocalAddressablesAssetImpl).InternalPath);
					CriAddressablesEditor.DeployData(srcPath, dstPath);
				}
				Debug.Log($"[CRIWARE] CriAddressableAssetsDeployer copied {assets.Count} files to {addressablesBuildPath}/{CriStreamingFolderAssetImplCreator.DirectoryName}.");
			}
			catch
			{
				throw;
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}

			CriLocalAddressableGroupGenerator.ValidateGroup();
		}

		static ReturnCode DeployAsWriteBundle(IBundleBuildResults buildResults)
		{
			if (CriAddressablesSetting.Mode == CriAddressablesSetting.DeployMode.UseCriBuildScript) return ReturnCode.Success;
			var addressableEntries = 
				CriAddressablesEditor.CollectDependentAssets<CriAddressableAssetImpl>()
				.Select(a => new {
					asset = a,
					entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath((a.Implementation as CriAddressableAssetImpl).anchor)))
				})
				.Where(pair => pair.entry.parentGroup != CriRemoteAddressableGroupGenerator.Group.AddressableGroup)
				.Where(pair => pair.entry.parentGroup.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>()?.IncludeInBuild ?? false)
				.Distinct().ToList();
			var groups = new List<UnityEditor.AddressableAssets.Settings.AddressableAssetGroup>();
			foreach(var pair in addressableEntries)
			{
				groups.Add(pair.entry.parentGroup);
				var bundleName = CriAddressablesEditor.CalclateBundleName(pair.entry);
				var path = buildResults.BundleInfos.FirstOrDefault(info => Path.ChangeExtension(info.Key, null) == bundleName).Value.FileName;
				CriAddressablesEditor.DeployData(AssetDatabase.GetAssetPath(pair.asset), path);
			}

			foreach (var g in groups.Distinct())
				CriAddressableGroupGenerator.ValidateGroup(g);

			Debug.Log($"[CRIWARE] CriAddressableAssetsDeployer copied {addressableEntries.Count} files.");
			return ReturnCode.Success;
		}

		[System.Obsolete]
		public static void DeployRemoteData()
		{
			try
			{
				EditorUtility.DisplayProgressBar("[CRIWARE]  Collectiong dependencies for remote bundles", "", 0);
				var addressableReferences = CriAddressablesEditor.CollectDependentAssets<CriAddressableAssetImpl>()
					.Select(asset => new
					{
						asset = asset,
						entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath((asset.Implementation as CriAddressableAssetImpl).anchor)))
					})
					.Where(pair => pair.entry != null)
					.Where(pair => pair.entry.parentGroup == CriRemoteAddressableGroupGenerator.Group.AddressableGroup).ToList();

				if (addressableReferences.Count <= 0) return;

				var count = 0;
				foreach (var pair in addressableReferences)
				{
					count++;
					EditorUtility.DisplayProgressBar("[CRIWARE] Deploy data for remote bundles", $"{count} / {addressableReferences.Count} assets", (float)count / addressableReferences.Count);
					CriRemoteAddressableGroupGenerator.DeployData(AssetDatabase.GetAssetPath(pair.asset), pair.entry);
				}

				Debug.Log($"[CRIWARE] Deploy Cri Addressable Assets : Complete, {addressableReferences.Count} Files.");
			}
			catch
			{
				throw;
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}

			CriRemoteAddressableGroupGenerator.ValidateGroup();
		}
	}
}

#endif

/** @} */
