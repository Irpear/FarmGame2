using UnityEngine.UI;

// ButtonUtils.SetInteractable(button, false);

public static class ButtonUtils
{
    // Knop (on)klikbaar maken zonder Unity's korte kleur-fade (standaard 0.1s).
    // Anders is een vergrendelde knop bij het openen van een scherm nog even in zijn normale kleur te zien.
    public static void SetInteractable(Selectable selectable, bool interactable)
    {
        if (selectable == null) return;

        ColorBlock colors = selectable.colors;
        if (colors.fadeDuration != 0f)
        {
            colors.fadeDuration = 0f;
            selectable.colors = colors;
        }

        selectable.interactable = interactable;
    }
}
