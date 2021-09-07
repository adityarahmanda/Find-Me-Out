using UnityEngine;
using TMPro;

public class PlayerReadyInfo : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI isReadyText;

    public Color isReadyColor = Color.green;
    public Color isNotReadyColor = Color.red;

    public void SetUsernameText(string _username)
    {
        usernameText.text = _username;
    }

    public void SetIsReadyText(bool _isReady)
    {
        if(_isReady)
        {
            isReadyText.text = "Ready";
            isReadyText.color = isReadyColor;
        }
        else
        {
            isReadyText.text = "Not Ready";
            isReadyText.color = isNotReadyColor;
        }
    }
}
