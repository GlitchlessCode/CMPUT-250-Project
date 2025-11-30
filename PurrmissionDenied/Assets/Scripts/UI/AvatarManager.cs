using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : Subscriber
{
    public Image AvatarElement;

    public List<Sprite> Sprites;

    [Header("Event Listeners")]
    public UserEntryGameEvent UserLoaded;

    public override void Subscribe()
    {
        UserLoaded?.Subscribe(OnUserLoaded);
    }

    void OnUserLoaded(UserEntry user)
    {
        if (Sprites.Count > user.image_index)
        {
            AvatarElement.sprite = Sprites[user.image_index];
        }
        else if (Sprites.Count > 0)
        {
            AvatarElement.sprite = Sprites[0];
        }
    }
}
