// Dev by: JOAO MANOEL FRANÇA DUARTE BONGIOVANI (Juca Duarte)
// HTC Vive Tracker Manager

    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using Valve.VR;
    using static Valve.VR.SteamVR_TrackedObject;
    using System.Linq;
    using Logger;

namespace ErgoTraining.Inputs
{
    [Serializable]
    public struct VirtualDevice
    {
        public EIndex DeviceIndex;
        public string Limb;

        public VirtualDevice(EIndex deviceIndex, string limb)
        {
            DeviceIndex = deviceIndex;
            Limb = limb;
        }
    }

    public enum TrackerPart { left_shoulder, right_shoulder, left_foot, right_foot, waist, /*add more*/ }

    [Serializable]
    public struct TrackerLocal
    {
        public SteamVR_TrackedObject TrackerObject;
        public TrackerPart Limb;

        public TrackerLocal(SteamVR_TrackedObject deviceIndex, TrackerPart Limb)
        {
            TrackerObject = deviceIndex;
            this.Limb = Limb;
        }
    }

    public sealed class TrackerManager : MonoBehaviour
    {
        private static TrackerManager instance;
        public static TrackerManager Instance { get { return instance; } private set { } }
        private const ETrackedDeviceProperty _config = ETrackedDeviceProperty.Prop_ControllerType_String;
        [SerializeField] private int MaxHadwareDevicesConnected = 20;
        [SerializeField] private List<VirtualDevice> VirtualDevices;
        [SerializeField] private TrackerLocal[] LocalTrackers;
        List<string> harddevices = new List<string>();

        private void Awake() => instance = this;

        private StringBuilder GetDeviceTracked(int index)
        {
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            StringBuilder id = new StringBuilder(64);
            OpenVR.System.GetStringTrackedDeviceProperty((uint)index, _config, id, 64, ref error);
            return id;
        }

        public List<string> FindAllRegisteredDevices()
        {
            try
            {
                LogTool.RegisterLog($"-----------------", false);
                LogTool.RegisterLog($"[TrackerManager]: Start Scaning Trackers.");

                string device = "";
                StringBuilder id = new StringBuilder(64);
                for (int hard = 0; hard < MaxHadwareDevicesConnected; hard++)
                {
                    try
                    {
                        LogTool.RegisterLog($"Tentando ler OPEN VR.");
                        id = GetDeviceTracked(hard);
                    }
                    catch (UnityException e) { LogTool.RegisterLog($"[ERRO] Tentativa de leitura do dispositivo mal sucessedida {e}."); }

                    device = id.ToString();
                    if (!string.IsNullOrEmpty(device) && device.Contains("tracker"))
                    {
                        harddevices.Add(device);
                    }
                }

                return harddevices;
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar FindAllRegisteredDevices. Exception: {e}");
                UFeedback.GetInstance().ErrorMsgBox.text = e.Message;
                return new List<string>();
            }
        }

        public void FillTrackersDevices()
        {
            try
            {
                LogTool.RegisterLog($"-----------------");
                harddevices = FindAllRegisteredDevices();

                foreach (var device in harddevices)
                {
                    var vd = new VirtualDevice((EIndex)harddevices.IndexOf(device), device);
                    VirtualDevices.Add(vd);
                }

                LogTool.RegisterLog($"[OK]: FillTrackersDevices OK.");
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar FillTrackersDevices. Exception: {e}");
            }

            AttachVirtualToModule(TrackerPart.right_shoulder);
            AttachVirtualToModule(TrackerPart.left_shoulder);
            AttachVirtualToModule(TrackerPart.left_foot);
            AttachVirtualToModule(TrackerPart.right_foot);
            AttachVirtualToModule(TrackerPart.waist);
        }

        private void AttachVirtualToModule(TrackerPart TrackerModule)
        {
            try
            {
                LogTool.RegisterLog($"-----------------");
                var vd = VirtualDevices.FirstOrDefault(v => v.Limb.Contains(TrackerModule.ToString()));
                var ltracker = LocalTrackers.FirstOrDefault(t => t.Limb == TrackerModule);
                if (ltracker.TrackerObject != null)
                {
                    ltracker.TrackerObject.index = vd.DeviceIndex;
                }
                LogTool.RegisterLog($"[OK]: AttachVirtualToModule OK ");
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar FillTrackersDevices. Exception: {e}");
            }
        }
    }
}
