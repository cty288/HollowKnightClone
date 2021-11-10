using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Singletons;

namespace MikroFramework.ResKit
{
    public interface IHotUpdateManager: ISingleton {
        public void Init(Action onInitFinished, Action<HotUpdateError> onInitFailed);

        public void ValidateHotUpdateState(Action onCheckFinished, Action<HotUpdateError> onError);

        public void GetNativeResVersion(Action<ResVersion> getNativeResVersion, Action<HotUpdateError> onError);

        public void HasNewVersionRes(Action<bool, ResVersion> onResult, Action<HotUpdateError> onError);

        public void GetRemoteResVersion(Action<ResVersion> onRemoteResVersionGet, Action<HotUpdateError> onError);

        public void UpdateRes(Action onUpdateDone, Action<HotUpdateError> onUpdateFailed);

        public void ValidateHotUpdateCompleteness(Action<List<ABMD5Base>> onFinished, Action<HotUpdateError> onError);

        public void DeleteRedundantFiles(Action onFinished, Action<HotUpdateError> onError);

    }
}
