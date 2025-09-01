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

using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using UnityEngine.ResourceManagement.Util;

namespace CriWare.Assets
{
    public class CriDummyAssetBundleResource : IAssetBundleResource
    {
        public AssetBundle GetAssetBundle()
        {
            return null;
        }
    }

	/**
	 * <summary>CRIアセットのキャッシュ向けの Addressables 向けリソースプロバイダークラス</summary>
	 */
    [System.ComponentModel.DisplayName("Cri Resource Provider")]
    public class CriResourceProvider : ResourceProviderBase
    {
        public override string ProviderId => GetType().FullName;

		public override void Provide(ProvideHandle providerInterface)
        {
			var location = providerInterface.Location;

			// This should be false if ModifyLocators called.
			if (!(location.Data is CriLocationSizeData))
			{
				location = new CriResourceLocation(location);
				Debug.LogWarning("[CRIWARE] CriAddressables.ModifyLocators not called yet.");
			}

			var pathset = CriAddressables.ResourceLocation2Path(location);
			var data = location.Data as CriLocationSizeData;

            if (File.Exists(pathset.local) || !ResourceManagerConfig.IsPathRemote(pathset.remote))
            {
				CriAddressables.AddCachePath(Path.GetFileName(pathset.local), pathset.local, data.GetResourceSize(location));
				CriAddressables.AddCachePath(data.BundleName, pathset.local, data.GetResourceSize(location));
				providerInterface.Complete(new CriDummyAssetBundleResource(), true, null);
                return;
            }

			var dir = Path.GetDirectoryName(pathset.local);
			if(Directory.Exists(dir))
				Directory.Delete(dir, true);
			var filename = Path.GetFileName(pathset.remote).Split('?')[0];
            var request = new UnityWebRequest(pathset.remote.Replace(filename, Uri.EscapeDataString(filename)), UnityWebRequest.kHttpVerbGET);
			var tmpPath = Path.Combine(dir, Guid.NewGuid().ToString().Replace("-", ""));
			var handler = new DownloadHandlerFile(tmpPath);
			handler.removeFileOnAbort = true;
			request.downloadHandler = handler;
			request.timeout = data.Timeout;
#if ADDRESSABLES_1_20_0_OR_NEWER
			var queop = UnityEngine.ResourceManagement.WebRequestQueue.QueueRequest(request);
			if (queop.IsDone)
				queop.Result.completed += (op) => CompleteWebRequest();
			else
				queop.OnComplete += (webop) => webop.completed += (op) => CompleteWebRequest();
#else
			request.SendWebRequest().completed += (op) => CompleteWebRequest();
#endif

            providerInterface.SetProgressCallback(() => request.downloadProgress);

#if ADDRESSABLES_1_14_2_OR_NEWER
			providerInterface.SetDownloadProgressCallbacks(() => new DownloadStatus() {
                IsDone = request.isDone,
                DownloadedBytes = (long)request.downloadedBytes,
                TotalBytes = data.GetResourceSize(location),
            });
#endif

			void CompleteWebRequest()
			{
#if UNITY_2020_2_OR_NEWER
				if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError || request.result == UnityWebRequest.Result.ProtocolError)
#else
				if (request.isHttpError || request.isNetworkError)
#endif
				{
					if (File.Exists(tmpPath))
						File.Delete(tmpPath);
					var exception = new Exception(string.Format(
							"CriResourceProvider unable to load from url {0}, result='{1}'.", request.url,
							request.error));
					providerInterface.Complete<CriDummyAssetBundleResource>(null, false, exception);
					return;
				}

				File.Move(tmpPath, pathset.local);
				CriAddressables.AddCachePath(Path.GetFileName(pathset.local), pathset.local, data.GetResourceSize(location));
				CriAddressables.AddCachePath(data.BundleName, pathset.local, data.GetResourceSize(location));
				providerInterface.Complete(new CriDummyAssetBundleResource(), true, null);
			}
		}

        public override Type GetDefaultType(IResourceLocation location) => typeof(IAssetBundleResource);

        public override void Release(IResourceLocation location, object asset) { }
	}

	[System.ComponentModel.DisplayName("Cri Local Resource Provider")]
	public class CriLocalResourceProvider : ResourceProviderBase
	{
		public override string ProviderId => GetType().FullName;

		public override void Provide(ProvideHandle providerInterface)
		{
			// GetDirectoryName does not work fine for URL.
			CriAddressables.LocalLoadPath = Uri.IsWellFormedUriString(providerInterface.Location.InternalId, System.UriKind.Absolute) ?
				providerInterface.Location.InternalId.Substring(0, providerInterface.Location.InternalId.LastIndexOf('/')) :
				Path.GetDirectoryName(providerInterface.Location.InternalId);
			providerInterface.Complete(new CriDummyAssetBundleResource(), true, null);
		}

		public override Type GetDefaultType(IResourceLocation location) => typeof(IAssetBundleResource);

		public override void Release(IResourceLocation location, object asset) { }
	}
}

#endif

/** @} */
