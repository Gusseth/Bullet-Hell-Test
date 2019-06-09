using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class contains any information concerning audio. If you're looking for static methos, use the Environment class.
/// </summary>

[System.Serializable]
public class Audio {

    // SFX Volume 'priority', makes specific sound effects louder to distinguish the sound from other sound effects

    /// <summary> Volume coefficient priority for less important sound effects. </summary>
    public readonly static float sfxLowPriority = 0.2F;

    /// <summary> Volume coefficient priority for most sound effects. </summary>
    public readonly static float sfxNormalPriority = 0.4F;

    /// <summary> Volume coefficient priority for more important effects. </summary>
    public readonly static float sfxHighPriority = 0.6F;

    /// <summary> Volume coefficient priority for highly important effects. </summary>
    public readonly static float sfxTopPriority = 0.8F;

    /// <summary> Enumeration of all sound effects. </summary>
    public enum sfx
    {
        ok, cancel, select, pause, powerUp, extend, itemPickup, masterSpark, spellcard, plDeath, bossDeath, enemyDeath, graze, plShoot, damage0, damage1, enmShoot0, enmShoot1, enmShoot2, enmPowerUp0, enmPowerUp1, chargeUp0, chargeUp1, cardClear
    }

    /// <summary> Enumeration of all stage and boss music. </summary>
    public enum bgm
    {
        menu, score, stg01, stg01b, stg02, stg02b, stg03, stg03b, stg04, stg04b, stg05, stg05b, stg06, stg06b, stg07, stg07b
    }

    /// <summary>
    /// Parses audio enumerations into appropriate AudioClips.
    /// </summary>
    /// <param name="AudioEnum">An enumerator from the Audio class.</param>
    /// <returns></returns>
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
                case sfx.spellcard:
                    audioName = "se_cat00";
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
                case sfx.graze:
                    audioName = "se_graze";
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
                case sfx.enmPowerUp0:
                    audioName = "se_power0";
                    break;
                case sfx.enmPowerUp1:
                    audioName = "se_power1";
                    break;
                case sfx.chargeUp0:
                    audioName = "se_ch00";
                    break;
                case sfx.chargeUp1:
                    audioName = "se_ch01";
                    break;
                case sfx.cardClear:
                    audioName = "se_cardget";
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

    /// <summary>
    /// Fades the AudioSource out.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeInSeconds"></param>
    /// <returns></returns>
    public static IEnumerator FadeOut(AudioSource source, float timeInSeconds)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / timeInSeconds;

            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}
