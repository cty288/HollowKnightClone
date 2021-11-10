using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.ResKit;
using MikroFramework.SceneEntranceKit;
using UnityEngine;

namespace MikroFramework.Managers
{
    /// <summary>
    /// A convenient Entrance that integrates the complete process of hot-update (initialize Hot-update manager-
    /// check new version-download updated files-validate completeness-delete redundant files).
    /// The complete cycle will automatically start at the beginning of the released mode, with callback functions for each phrase
    /// </summary>
    public abstract class HotUpdateEntranceManager : EntranceManager {

        protected ResLoader resLoader;
        /// <summary>
        /// The complete cycle of hot-updating will always run in the released mode
        /// </summary>
        protected override void LaunchInReleasedMode() {
            StartHotUpdate();
        }

        /// <summary>
        /// Start the complete hot update process, called by LaunchInReleasedMode() by default
        /// </summary>
        protected void StartHotUpdate() {
            InitializeHotUpdateManager();
            EnableUpdateDownloadSpeed();
        }
        private void InitializeHotUpdateManager() {
            HotUpdateManager.Singleton.Init(() => {
                OnHotUpdateManagerInitialized();
                CheckNewVersionRes();
            }, OnHotUpdateError);
        }

        private void CheckNewVersionRes() {
            HotUpdateManager.Singleton.HasNewVersionRes((needUpdate, resVersion) => {
                OnHotUpdateVersionChecked(needUpdate,resVersion);

                if (needUpdate) {
                    UpdateResources();
                }
                else {
                    ValidateResourceCompleteness();
                }
                
            }, OnHotUpdateError);
        }

        private bool downloading = false;

        private void UpdateResources() {
            downloading = true;
            HotUpdateManager.Singleton.UpdateRes(() => {
                downloading = false;
                OnHotUpdateResourceDownloadedAndUpdated();
                ValidateResourceCompleteness();
            },OnHotUpdateError);
        }

        private void ValidateResourceCompleteness() {
            downloading = true;
            HotUpdateManager.Singleton.ValidateHotUpdateCompleteness(results => {
                downloading = false;
                OnResourceCompletenessValidated(results);
                DeleteRedundantFiles();
            },OnHotUpdateError);
        }

        private void DeleteRedundantFiles() {
            HotUpdateManager.Singleton.DeleteRedundantFiles(() => {
                resLoader = new ResLoader();
                OnRedundantFilesDeleted();
                OnHotUpdateComplete();
                
            },OnHotUpdateError);
        }

        /// <summary>
        /// Extend this update method
        /// </summary>
        public virtual void Update() {
            if (downloading) {
                OnHotUpdateResourcesDownloading(HotUpdateManager.Singleton.Downloader.GetDownloadProgress(),
                    HotUpdateManager.Singleton.Downloader.GetTotalDownloadFileSize(), HotUpdateManager.Singleton.Downloader.GetAlreadyDownloadedFileSize(),
                    HotUpdateManager.Singleton.Downloader.GetDownloadingFileSize(), HotUpdateManager.Singleton.Downloader.GetDownloadSpeed());
            }
        }

        /// <summary>
        /// To disable updating download speed, override this function and call HotUpdateManager.DisableUpdateDownloadSpeed()
        /// </summary>
        protected virtual void EnableUpdateDownloadSpeed() {
            HotUpdateManager.Singleton.EnableUpdateDownloadSpeed(1f);
        }


        /// <summary>
        /// Callback function invoked when any error occurs during the hot-updating phrase
        /// </summary>
        /// <param name="error"></param>
        protected virtual void OnHotUpdateError(HotUpdateError error) {

        }

        /// <summary>
        /// Callback function invoked when the hot-update manager is initialized
        /// </summary>
        protected virtual void OnHotUpdateManagerInitialized() {

        }

        /// <summary>
        /// Callback invoked when the Hot-update version is checked
        /// </summary>
        /// <param name="needUpdate">Does the current game need to be updated?</param>
        /// <param name="localResVersion">The local ResVersion (contains local AB file infos) file</param>

        protected virtual void OnHotUpdateVersionChecked(bool needUpdate, ResVersion localResVersion) {

        }

        /// <summary>
        /// Callback invoked every frame when resources are downloading and updating
        /// </summary>
        /// <param name="downloadProgress">The overall progress of the current download queue</param>
        /// <param name="totalDownloadSize">The total size of all files combined of the current download queue</param>
        /// <param name="alreadyDownloadedFileSize">The combined size of all files that have completely downloaded (not including files that are downloading) of the current download queue</param>
        /// <param name="downloadingFileDownloadedSize">The downloaded size of the current downloading file (not complete)</param>
        /// <param name="downloadSpeed">Current download speed</param>
        protected virtual void OnHotUpdateResourcesDownloading(float downloadProgress, float totalDownloadSize,
            float alreadyDownloadedFileSize, float downloadingFileDownloadedSize, float downloadSpeed) {

        }

        /// <summary>
        /// Invoked when resources are successfully downloaded and updated on the local device
        /// </summary>
        protected virtual void OnHotUpdateResourceDownloadedAndUpdated() {

        }

        /// <summary>
        /// Invoked when the completeness of all local resources has been validated
        /// </summary>
        /// <param name="updatedResourceInfos">Infos of files that have been updated (null if all original files are complete)</param>
        protected virtual void OnResourceCompletenessValidated(List<ABMD5Base> updatedResourceInfos) {

        }

        /// <summary>
        /// Invoked when redundant files are deleted from the local device
        /// </summary>
        protected virtual void OnRedundantFilesDeleted() {

        }

        /// <summary>
        /// Invoked after the complete hot-update cycle has completed
        /// </summary>
        protected abstract void OnHotUpdateComplete();
    }
}
