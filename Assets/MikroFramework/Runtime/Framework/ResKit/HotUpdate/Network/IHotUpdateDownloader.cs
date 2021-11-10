using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.ResKit { 
    public interface IHotUpdateDownloader {
        public IEnumerator GetLocalAssetResVersion(Action<ResVersion> getResVersion, Action<HotUpdateError>
            onGetFailed);

        public IEnumerator RequestGetRemoteResVersion(Action<ResVersion> onResDownloaded,
            Action<HotUpdateError> onError);

        public void DoDownloadResVersion(ResVersion remoteResVersion);

        public IEnumerator DoDownloadRemoteABs(List<ABMD5Base> downloadList,
            Action<List<ABMD5Base>> onDownloadDone, Action<HotUpdateError> onDownloadFailed);

        /// <summary>
        /// Get the download progress (between 0-1) of all the current downloading files
        /// </summary>
        /// <returns></returns>
        public float GetDownloadProgress();

        /// <summary>
        /// Return the total size (kb) of all the files need to be downloaded currently
        /// </summary>
        /// <returns></returns>
        public float GetTotalDownloadFileSize();

        /// <summary>
        /// Return the total size (kb) of all the files that have already downloaded in the current downloading progress
        /// </summary>
        /// <returns></returns>
        public float GetAlreadyDownloadedFileSize();

        /// <summary>
        /// Get the file size  that has already been downloaded of the currently downloading single file
        /// return 0 if nothing is downloading
        /// </summary>
        /// <returns></returns>
        public float GetDownloadingFileSize();

        /// <summary>
        /// Get the current download speed (kb/s). You must also call EnableUpdateDownloadSpeed() method before
        /// to update real-time download speed. It will return -1 if EnableUpdateDownloadSpeed hasn't been called
        /// </summary>
        /// <returns></returns>
        public float GetDownloadSpeed();

        /// <summary>
        /// Enable the hot-update manager to update real-time download speed (this may cost extra memory). 
        /// </summary>
        /// <param name="updateInterval">Update time interval (seconds)</param>
        public void EnableUpdateDownloadSpeed(float updateInterval = 1f);

        /// <summary>
        /// Disable the hot-update manager to update real-time download speed
        /// </summary>
        public void DisableUpdateDownloadSpeed();
    }
}
