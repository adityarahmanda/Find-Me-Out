using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FindMeOut.Lobby;

public class WardrobeManager : MonoBehaviour
{
    public static WardrobeManager instance;
    
    private int lastSelectedId = 0;

    [Header("Character")]
    public Renderer characterRenderer;
    public Texture[] skinTextures = new Texture[8];

    [Header("Selection Settings")]
    public SkinSelection[] skinSelections = new SkinSelection[8];
    public Sprite selectedBackground;
    public Sprite unselectedBackground;

    [Header("Buttons")]
    public Button useButton;
    public Button wardrobeButton;
    public Button lobbyButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start() {
        for(int i = 0; i < skinSelections.Length; i++)
        {
            int _id = i;
            skinSelections[i].button.onClick.AddListener(() => SetCharacterTexture(_id));
        }

        int _clientId = Client.instance.myId;
        if(PlayerManager.instance.players.ContainsKey(_clientId))
        {
            SetCharacterTexture(PlayerManager.instance.players[_clientId].selectedSkinTextureId);
        }

        wardrobeButton.onClick.AddListener(() => CanvasManager.instance.OpenCanvas(CanvasType.Wardrobe, true));
        lobbyButton.onClick.AddListener(() => CanvasManager.instance.OpenCanvas(CanvasType.Wardrobe, false));
        useButton.onClick.AddListener(UseSkinTexture);    
    }

    public void SetCharacterTexture(int _id)
    {
        skinSelections[lastSelectedId].image.sprite = unselectedBackground;
        skinSelections[_id].image.sprite = selectedBackground;
        lastSelectedId = _id;

        characterRenderer.material.SetTexture("_BaseMap", skinTextures[_id]);
    }

    public void UseSkinTexture()
    {
        ClientSend.SetSkin(lastSelectedId);
        CanvasManager.instance.OpenCanvas(CanvasType.Wardrobe, false);
    }
}
