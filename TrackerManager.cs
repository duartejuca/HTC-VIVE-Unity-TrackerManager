// DESENVOLVIMENTO: JOAO MANOEL FRANÇA DUARTE BONGIOVANI (Juca Duarte)
//v1.0.1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;
using static Valve.VR.SteamVR_TrackedObject;
//TODO using System.Linq; 


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
    
    public enum TrackerPart { left_shoulder, right_shoulder, left_foot, right_foot, waist }

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

        private void Awake() => instance = this;

        private void Start()
        {
            FillDevices();
        }

        public List<string> FindAllRegisteredDevices()
        {
            UnityEngine.Debug.Log("[ErgoTraining]: Verificando dispositivos conectados.");
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
            //VirtualDevices.Clear();


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
            AttachVirtualToModule(TrackerPart.left_shoulder) ;
            AttachVirtualToModule(TrackerPart.left_foot);
            AttachVirtualToModule(TrackerPart.right_foot);
            AttachVirtualToModule(TrackerPart.waist);
        }

        

        private void AttachVirtualToModule(TrackerPart TrackerModule)
        {
            foreach (VirtualDevice vd in VirtualDevices)
            {
                if (_ = vd.Limb.Contains(TrackerModule.ToString()))
                {
                    foreach (TrackerLocal ltracker in LocalTrackers)
                    {
                        if(_ = ltracker.Limb.ToString().Contains(TrackerModule.ToString()))
                        {
                            ltracker.TrackerObject.index = vd.DeviceIndex;
                            break;
                        }
                    }
                    continue;
                }
            }
        }

        // RETORNA LISTA DE ENUMERAÇÃO - HANDLER
        /*private static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }*/
    }
}
