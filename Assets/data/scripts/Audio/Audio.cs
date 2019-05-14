using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class contains any information concerning audio. If you're looking for static methos, use the Environment class.
/// </summary>

[System.Serializable]
public class Audio {


    /// <summary> Enumeration of all sound effects. </summary>
    public enum sfx
    {
        ok, cancel, select, pause, powerUp, extend, itemPickup, masterSpark, plDeath, bossDeath, enemyDeath, plShoot, damage0, damage1, enmShoot0, enmShoot1, enmShoot2
    }

    /// <summary> Parses audio enumerations into AudioClips. </summary>
    public static AudioClip Parse(System.Enum AudioEnum)
    {
        System.Type enumType = AudioEnum.GetType();
        string audioName;

        // Audio SFX
        if (enumType == typeof(sfx))
        {
            switch ((sfx)AudioEnum)
            {
                case sfx.ok:
                    audioName = "se_ok00";
                    break;
                case sfx.cancel:
                    audioName = "se_cancel00";
                    break;
                case sfx.select:
                    audioName = "se_select00";
                    break;
                case sfx.pause:
                    audioName = "se_pause";
                    break;
                case sfx.powerUp:
                    audioName = "se_powerup";
                    break;
                case sfx.extend:
                    audioName = "se_extend";
                    break;
                case sfx.itemPickup:
                    audioName = "se_item00";
                    break;
                case sfx.masterSpark:
                    audioName = "se_nep00";
                    break;
                case sfx.plDeath:
                    audioName = "se_pldead00";
                    break;
                case sfx.bossDeath:
                    audioName = "se_enep01";
                    break;
                case sfx.enemyDeath:
                    audioName = "se_enep00";
                    break;
                case sfx.plShoot:
                    audioName = "se_plst00";
                    break;
                case sfx.damage0:
                    audioName = "se_damage00";
                    break;
                case sfx.damage1:
                    audioName = "se_damage01";
                    break;
                case sfx.enmShoot0:
                    audioName = "se_tan00";
                    break;
                case sfx.enmShoot1:
                    audioName = "se_tan01";
                    break;
                case sfx.enmShoot2:
                    audioName = "se_tan02";
                    break;
                default:
                    Debug.LogWarning("Invalid SFX Enumerator! Are you sure you're trying to parse a valid SFX enum?");
                    return null;
            }

            // Finds the desired SFX in the list and returns it.
            return Environment.sfx.Find(x => x.name == audioName);
        }
        else
        {
            Debug.LogWarning("Invalid Audio Enumerator! Are you sure you're trying to parse a valid audio enumerator?");
            return null;
        }
    }
}
