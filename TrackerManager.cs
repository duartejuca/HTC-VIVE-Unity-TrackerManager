/*
* DEVELOPED BY: JOAO MANOEL FRANÇA DUARTE BONGIOVANI (JOAO DUARTE)
* ESTA FERRAMENTA DEVE SER UTILIZADA PARA DESCOBERTA DE DADOS RELACIONADOS AO TRACKER DA HTC
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;
using static Valve.VR.SteamVR_TrackedObject;

namespace ErgoTraining.Inputs
{
    [System.Serializable]
    public struct HardwareDeviceProp
    {
        public string deviceModel;
        public bool isUsed;

        public HardwareDeviceProp(string deviceModel, bool isUsed)
        {
            this.deviceModel = deviceModel;
            this.isUsed = isUsed;
        }
    }

    public class TrackerManager : MonoBehaviour
    {

        private static TrackerManager instance;
        public static TrackerManager Instance { get { return instance; } private set { } }

        private void Awake()
        {
            instance = this;
        }

        /// <summary>Número de trackers a serem listados.</summary>
        public int VirtualDevicesToUse = 5;


        [SerializeField] private int MaxHadwareDevices = 20;
        [SerializeField] private List<EIndex> VirtualDevices;
        private List<HardwareDeviceProp> HardwareDevices = new List<HardwareDeviceProp>();

        /// <summary>Retorna a quantidade de dispositivos conectados e ativos.</summary>
        public int GetDevicesConected() => VirtualDevices.Count;
        
        
        private static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }

        private void Start()
        {
            
            StartCoroutine(DeviceCheck());
        }

        public ETrackedDeviceProperty RoleToReturn;
        
        // nome do tracker na configuração do STEAMVR [retorna ex: LHR-E54F8FFB]
        const ETrackedDeviceProperty firmwareName = ETrackedDeviceProperty.Prop_Firmware_ProgrammingTarget_String;
        const ETrackedDeviceProperty limbConfig = ETrackedDeviceProperty.Prop_ControllerType_String;

        private void FindTrackerHardwareDevices()
        {
            Debug.Log("[ERGOTRAINING]: Verificando dispositivos conectados.");
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            
            StringBuilder id = new(64);
            StringBuilder id2 = new(64);

            for (int hard = 0; hard < MaxHadwareDevices; hard++)
            {
                try {
                    // retornando todos os hardwares conectados da VIVE.
                    //OpenVR.System.GetStringTrackedDeviceProperty((uint)hard, ETrackedDeviceProperty.Prop_RenderModelName_String, id, 64, ref error);
                    OpenVR.System.GetStringTrackedDeviceProperty((uint)hard, limbConfig, id, 64, ref error);
                    OpenVR.System.GetStringTrackedDeviceProperty((uint)hard, RoleToReturn, id2, 64, ref error);

                   
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ERGOTRAINING]: Não foi encontrado nenhum dispositivo de realidade virtual: {e.Message} | {e.Source}");
                    return;
                }
                
                if(id2.ToString() != string.Empty)
                    Debug.Log($"Dispositivo Conectado ID2: {id2}");

                string device = id.ToString();
                if (device != string.Empty) { HardwareDevices.Add(new HardwareDeviceProp(device, false)); }
            }
        }

        // RETORNA TODAS AS INFORMAÇÕES DO PRODUTO
        private void AllDevicesConnectedInfo()
        {
            List<ETrackedDeviceProperty> ETrackDevice = new List<ETrackedDeviceProperty>();
            ETrackDevice = GetEnumList<ETrackedDeviceProperty>();
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            StringBuilder id = new(64);

            for (int hard = 0; hard < MaxHadwareDevices; hard++)
            {
                foreach (var device in ETrackDevice)
                {
                    OpenVR.System.GetStringTrackedDeviceProperty((uint)hard, device, id, 64, ref error);
                    Debug.Log($"{ETrackDevice}: {id}");
                }
            }
        }

        private bool HadwareToVirtualDevices()
        {
            for (int hard = 0; hard < HardwareDevices.Count; hard++)
            {
                if (HardwareDevices[hard].deviceModel.Contains("tracker"))
                {
                    if (!HardwareDevices[hard].isUsed)
                    {
                        VirtualDevices.Add((EIndex)hard);
                        HardwareDevices[hard] = new HardwareDeviceProp(HardwareDevices[hard].deviceModel, true);
                        continue;
                    }
                }
            }

            if (RegistryTrackersToUse()) return true;
            else return false;
        }

        /// <summary>Troca a configuração ativa do Tracker indicado no parametro Index.</summary>
        public void VirtualTrackersConfig(SteamVR_TrackedObject VirtualTracker, int Index) => VirtualTracker.index = VirtualDevices[Index];

        private bool RegistryTrackersToUse()
        {
            if (VirtualDevices.Count < VirtualDevicesToUse)
            {
                Debug.LogError($"[ERGOTRAINING]: Número de dispostitivos irregular, necessário {VirtualDevicesToUse} dispositivos conectados.");
                return false;
            }

            return true;
        }

        

        

        private IEnumerator DeviceCheck()
        {
            

            FindTrackerHardwareDevices();
            yield return new WaitForSeconds(2);

            yield return new WaitUntil(() => HadwareToVirtualDevices());

            //FindTrackerHardwareDevices()
            Debug.Log("Registrado");


            //DefaultVRTrackersSetup();

            yield return null;
            yield break;
        }

        /// <summary>Aplica o tracker por meio da tela de configuração.</summary>
       /* public void SetTrackerToGameObject(int TrackerIndex)
        {
            if (TrackerIndex > GameManager.Instance.PlayerTrackers.Count && TrackerIndex > VirtualDevices.Count)
            { Debug.LogError("[ERGOTRAINING]: Indice selecionado inexistente."); return; }

            VirtualTrackersConfig(GameManager.Instance.PlayerTrackers[TrackerIndex].GetComponent<SteamVR_TrackedObject>(), TrackerIndex);
        }


        /// <summary>Aplica o tracker com configuração default.</summary>
        public void DefaultVRTrackersSetup()
        {
            for (int i = 0; i < VirtualDevicesToUse; i++)
            {
                VirtualTrackersConfig(GameManager.Instance.PlayerTrackers[i].GetComponent<SteamVR_TrackedObject>(), i);
                Debug.Log("[ERGOTRAINING]: Dispositivos encontrados e configurados.");
            }
        }*/
    }
}
