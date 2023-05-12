// Dev by: JOAO MANOEL FRANÇA DUARTE BONGIOVANI (Juca Duarte)
// HTC Vive Tracker Manager

using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;
using static ErgoTraining.Inputs.TrackerManager;
using static Valve.VR.SteamVR_TrackedObject;

namespace ErgoTraining.Inputs
{
    public sealed class TrackerManager : MonoBehaviour
    {
        [System.Serializable]
        public class TrackerInfo
        {
            public string id;
            public string model;

            public TrackerInfo(string id, string model)
            {
                this.id = id;
                this.model = model;
            }
        }

        public SteamVR_TrackedObject waist, leftShoulder, rightShoulder, leftFoot, rightFoot;
        public List<TrackerInfo> trackersInfo = new List<TrackerInfo>();
        [SerializeField] private int MaxHadwareDevicesConnected = 20;

        private string GetDeviceTracked(int index)
        {
            try
            {
                ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;

                StringBuilder deviceRegistry = new StringBuilder(64);
                ETrackedDeviceProperty prop = ETrackedDeviceProperty.Prop_Firmware_ProgrammingTarget_String;
                _ = OpenVR.System.GetStringTrackedDeviceProperty((uint)index, prop, deviceRegistry, 64, ref error);

                prop = ETrackedDeviceProperty.Prop_ControllerType_String;
                StringBuilder id = new StringBuilder(64);
                _ = OpenVR.System.GetStringTrackedDeviceProperty((uint)index, prop, id, 64, ref error);

                if (id.ToString().Contains("tracker"))
                {
                    LogTool.RegisterLog($"[Device]: {id} : {deviceRegistry}");
                    Debug.Log($"[Device]: {id} : {deviceRegistry}");
                }

                TrackerInfo tf = new TrackerInfo(id.ToString(), deviceRegistry.ToString());
                trackersInfo.Add(tf);

                return id.ToString();
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar GetDeviceTracked. Exception: {e}");
                Debug.Log($"[ERRO]: Não foi possível executar GetDeviceTracked. Exception: {e}");
                return null;
            }
        }
       
        public void FillTrackersDevices()
        {
            try
            {
                if (trackersInfo.Count > 0)
                {
                    trackersInfo.Clear();
                }

                LogTool.RegisterLog($"[TrackerManager]: Iniciando rastreio dos sensores.");
                for (int i = 0; i < MaxHadwareDevicesConnected; i++)
                {
                    GetDeviceTracked(i);

                    if (trackersInfo[i].id.Contains("left_foot"))
                    {
                        leftFoot.index = (EIndex)i;
                    }

                    if (trackersInfo[i].id.Contains("right_foot"))
                    {
                        rightFoot.index = (EIndex)i;
                    }

                    if (trackersInfo[i].id.Contains("right_shoulder"))
                    {
                        rightShoulder.index = (EIndex)i;
                    }

                    if (trackersInfo[i].id.Contains("left_shoulder"))
                    {
                        leftShoulder.index = (EIndex)i;
                    }

                    if (trackersInfo[i].id.Contains("waist"))
                    {
                        waist.index = (EIndex)i;
                    }
                }
                LogTool.RegisterLog($"[TrackerManager]: Finalizando rastreio dos sensores.");
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar FillTrackersDevices. Exception: {e}");
                Debug.Log($"[ERRO]: Não foi possível executar FillTrackersDevices. Exception: {e}");
            }
        }
    }
}
