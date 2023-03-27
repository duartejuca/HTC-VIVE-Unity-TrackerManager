// DESENVOLVIMENTO: JOAO MANOEL FRANÇA DUARTE BONGIOVANI (João Duarte)
// V1.0 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Valve.VR;
using static Valve.VR.SteamVR_TrackedObject;

namespace Tracker.Inputs
{

public enum TrackerPart { LeftShoulder, RightShoulder, LeftFoot, RightFoot, Waist }
    
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
        private List<VirtualDevice> VirtualDevices;

        [SerializeField] private TrackerLocal[] LocalTrackers;
        

        private void Awake() => instance = this;

        private void Start()
        {
            FillDevices();
        }

        public List<string> FindAllRegisteredDevices()
        {
            UnityEngine.Debug.Log("Verificying connected devices.");
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            StringBuilder id = new(64);
            List<string> harddevices = new List<string>();

            for (int hard = 0; hard < MaxHadwareDevicesConnected; hard++)
            {
                OpenVR.System.GetStringTrackedDeviceProperty((uint)hard, _config, id, 64, ref error);
                string device = id.ToString();
                if (device != string.Empty) 
                {
                    harddevices.Add(new string(device)); 
                }
            }
            return harddevices;
        }

        public void FillDevices()
        {
            List<string> harddevices = FindAllRegisteredDevices();
            VirtualDevices.Clear();
            for (int hard = 0; hard < harddevices.Count; hard++)
            {
                if (harddevices[hard].Contains("tracker"))
                {
                    VirtualDevice vd = new VirtualDevice((EIndex)hard, harddevices[hard]);
                    VirtualDevices.Add(vd);
                    continue;
                }
            }

            AttachVirtualToModule(TrackerPart.RightShoulder);
            AttachVirtualToModule(TrackerPart.LeftShoulder) ;
            AttachVirtualToModule(TrackerPart.LeftFoot);
            AttachVirtualToModule(TrackerPart.RightFoot);
            AttachVirtualToModule(TrackerPart.Waist);
        }

        private string TrackerPartToString(TrackerPart TrackerModule)
        {
            string part = "";
            switch (TrackerModule)
            {
                case TrackerPart.RightShoulder: part = "right_shoulder"; break;
                case TrackerPart.LeftShoulder: part = "left_shoulder"; break;
                case TrackerPart.LeftFoot: part = "left_foot"; break;
                case TrackerPart.RightFoot: part = "right_foot"; break;
                case TrackerPart.Waist: part = "waist"; break;
            }
            return part;
        }

        private void AttachVirtualToModule(TrackerPart TrackerModule)
        {
            foreach(VirtualDevice vd in VirtualDevices)
            {
                if (vd.Limb.Contains(TrackerPartToString(TrackerModule)))
                {
                    foreach (TrackerLocal lt in LocalTrackers)
                    {
                        if(lt.Limb == TrackerModule)
                        {
                            lt.TrackerObject.index = vd.DeviceIndex;
                        }
                    }
                }
                continue;
            }
        }
    }
}
