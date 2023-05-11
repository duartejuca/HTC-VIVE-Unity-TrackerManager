// Dev by: JOAO MANOEL FRANÇA DUARTE BONGIOVANI (Juca Duarte)
// HTC Vive Tracker Manager

using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;
using static Valve.VR.SteamVR_TrackedObject;
using System.Linq; 

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
        
        [SerializeField] private int MaxHadwareDevicesConnected = 20;
        [SerializeField] private List<VirtualDevice> VirtualDevices;
        [SerializeField] private TrackerLocal[] LocalTrackers;
        private void Awake() => instance = this;


        private string GetDeviceTracked(int index)
        {
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;

            StringBuilder deviceRegistry = new StringBuilder(64);
            ETrackedDeviceProperty prop = ETrackedDeviceProperty.Prop_Firmware_ProgrammingTarget_String;
            _ = OpenVR.System.GetStringTrackedDeviceProperty((uint)index, prop, deviceRegistry, 64, ref error);
            
            prop = ETrackedDeviceProperty.Prop_ControllerType_String;
            StringBuilder id = new StringBuilder(64);
            _ = OpenVR.System.GetStringTrackedDeviceProperty((uint)index, prop, id, 64, ref error);

            LogTool.RegisterLog($"[Device]: {id} : {deviceRegistry}");
            Debug.Log($"[Device]: {id} : {deviceRegistry}");
            
            return id.ToString();
        }
        public List<string> FindAllRegisteredDevices()
        {
            try
            {
                LogTool.RegisterLog($"[TrackerManager]: Scaning Trackers.");
                UnityEngine.Debug.Log("[TrackerManager]: Scaning Trackers.");

                var harddevices = Enumerable.Range(0, MaxHadwareDevicesConnected)
                                            .Select(hard => GetDeviceTracked(hard))
                                            .Where(device => !string.IsNullOrEmpty(device))
                                            .ToList();
                return harddevices;
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar FindAllRegisteredDevices. Exception: {e}");
                Debug.Log($"[ERRO]: Não foi possível executar FindAllRegisteredDevices. Exception: {e}");
                return new List<string>();
            }
        }

    public void FillTrackersDevices()
        {
            //AllDevicesConnectedInfo();
            try
            {
                LogTool.RegisterLog($"[TrackerManager]: Começando a registrar trackers.");
                List<string> harddevices = FindAllRegisteredDevices();

                for (int hard = 0; hard < harddevices.Count; hard++)
                {
                    if (_ = harddevices[hard].Contains("tracker"))
                    {
                        VirtualDevice vd = new VirtualDevice((EIndex)hard, harddevices[hard]);
                        VirtualDevices.Add(vd);
                        continue;
                    }
                }

                AttachVirtualToModule(TrackerPart.right_shoulder);
                AttachVirtualToModule(TrackerPart.left_shoulder);
                AttachVirtualToModule(TrackerPart.left_foot);
                AttachVirtualToModule(TrackerPart.right_foot);
                AttachVirtualToModule(TrackerPart.waist);
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar FillTrackersDevices. Exception: {e}");
                Debug.Log($"[ERRO]: Não foi possível executar FillTrackersDevices. Exception: {e}");
            }
        }

        private void AttachVirtualToModule(TrackerPart TrackerModule)
        {
            // refatorado usando linq
            try
            {
                var matchingDevices = VirtualDevices.Where(vd => vd.Limb.Contains(TrackerModule.ToString()));
                var matchingTrackers = LocalTrackers.Where(ltracker => ltracker.Limb.ToString().Contains(TrackerModule.ToString()));

                foreach (var vd in matchingDevices)
                {
                    foreach (var ltracker in matchingTrackers)
                    {
                        LogTool.RegisterLog($"[TrackerManager]: Registrando Tracker: {ltracker.TrackerObject} >> Virtual Device: {vd.DeviceIndex} ");
                        ltracker.TrackerObject.index = vd.DeviceIndex;
                    }
                }

                LogTool.RegisterLog($"[OK]: AttachVirtualToModule OK.");
            }
            catch (UnityException e)
            {
                LogTool.RegisterLog($"[ERRO]: Não foi possível executar AttachVirtualToModule. Exception: {e}");
                Debug.Log($"[ERRO]: Não foi possível executar AttachVirtualToModule. Exception: {e}");
            }
        }
    }
}
