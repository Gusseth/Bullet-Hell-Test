using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class Player : Entity {

// Variable Declaration

    // General variables for the player
    /// <summary> Number of lives in possession by default from this character. </summary>
    public int lives;

    /// <summary> Number of bombs in possession by default from this character. </summary>
    public int bombs;

    /// <summary> The maximum number of power the player can have. </summary>
    public float maxPower;

    /// <summary> Base speed of the character. </summary>
    public float rawSpeed;

    /// <summary> Shot speed of the main shot, unfocused.  </summary>
    public float shotSpeed1 = 20F;

    /// <summary> Shot speed of the secondary shot, focused.  </summary>
    public float shotSpeed2 = 10F;

    /// <summary> Shot speed of the tertiary shot, depends on character.  </summary>
    public float shotSpeed3 = 5F;

    /// <summary> The size of the hitbox for this character. </summary>
    public float hitboxRadius;

    /// <summary> The caracter the player is playing as. </summary>
    public Character character;

    /// <summary> The shot variant of the player. </summary>
    public CharacterShot characterShot;



    /// <summary> Enumeration of all playable characters. </summary>
    public enum Character
    {
        Reimu, Marisa //, Mima
    }

    /// <summary> Enumeration for shot variants ex. MarisaA, ReimuB, characterC etc.</summary>
    public enum CharacterShot
    {
        /// <summary> Concentrate shot types. </summary>
        A,

        /// <summary> Spread shot types. </summary>
        B,

        /// <summary> Power shot types. </summary>
        C
    }


// Functions and other crap below

    /// <summary>
    /// Class struct #1, function below creates an empty player for debug purposes.
    /// </summary>
    public Player()
    {

        lives = 3;
        bombs = 3;
        rawSpeed = 3.0F;
        maxPower = 4.0F;
        hitboxRadius = 0.03F;
        character = Character.Reimu;
        characterShot = CharacterShot.B;
    }

    /// <summary>
    /// Class struct #2, overloaded function creates a player based on the given characters and variant with default lives, bombs, speed.
    /// </summary>
    public Player(Character selectCharacter = Character.Reimu, CharacterShot selectCharacterShot = CharacterShot.A)
    {
        if (selectCharacter == Character.Reimu)
        {
            lives = 3;
            bombs = 3;
            rawSpeed = 3.0F;
            maxPower = 4.0F;
            hitboxRadius = 0.03F;
        }
        else if (selectCharacter == Character.Marisa)
        {
            //  If the selected character is Marisa
            lives = 3;
            bombs = 2;
            rawSpeed = 5.0F;
            maxPower = 5.0F;
            hitboxRadius = 0.05F;
        }

        character = selectCharacter;
        characterShot = selectCharacterShot;
    }

    /// <summary>
    /// Class struct #3, overloaded function creates a player based on specifications defined by more args.
    /// </summary>
    public Player(Character selectCharacter = Character.Reimu, CharacterShot selectCharacterShot = CharacterShot.A, int givenLives = 3, int givenBombs = 2, float givenSpeed = 3.0F)
    {
        if (selectCharacter == Character.Reimu)
        {
            //  If the selected character is Reimu
            lives = 3;
            bombs = 3;
            rawSpeed = 3.0F;
            hitboxRadius = 0.03F;
        }
        else if (selectCharacter == Character.Marisa)
        {
            //  If the selected character is Marisa
            lives = 3;
            bombs = 2;
            rawSpeed = 5.0F;
            hitboxRadius = 0.05F;
        }

        character = selectCharacter;
        characterShot = selectCharacterShot;
    }
}
