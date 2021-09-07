using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    [Header("Skin")]
    public int selectedSkinTextureId;

    [Header("Lobby State")]
    public bool isReady;  
    
    [Header("In-Game State")]
    public Roles role = Roles.None;  
    public bool isCaught;

    public bool IsWalk { get; private set; }
    private bool audioPlayed;
    private bool footprint;

    [Header("References")]
    public TextMeshPro usernameText;
    public GameObject prison;

    private Animator _animator;
    private AudioSource _audioSource;
    private SkinnedMeshRenderer _renderer;

    public GameObject prisonObj;

    private float footprintDur;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _animator = GetComponentInChildren<Animator>();
        _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        CheckFootprint();

        if(IsWalk && !audioPlayed)
        {
            _audioSource.Play();
            audioPlayed = true;
        }

        else if(!IsWalk && audioPlayed)
        {
            _audioSource.Stop();
            audioPlayed = false;
        }
    }

    private void OnDestroy() {
        Destroy(usernameText.gameObject);
    }

    public void Initialize(int _id, string _username, bool _isReady, int _textureId)
    {
        id = _id;
        isReady = _isReady;
        SetUsername(_username);

        audioPlayed = false;
        IsWalk = false;
        footprint = false;
        SetTexture(_textureId);
    }

    public void SetUsername(string _username)
    {
        username = _username;
        usernameText.text = _username;
    }

    public void SetIsReady(bool _value)
    {
        isReady = _value;
    }

    public void SetRole(Roles _role)
    {
        role = _role;
        tag = _role.ToString();
    }

    public void SetIsCaught(bool _value)
    {
        isCaught = _value;

        if (Client.instance.myId == PlayerManager.instance.seekerId && _renderer.enabled != _value)
        {
            SetInvisible(!_value);
        }

        if (isCaught)
        {
            Instantiate(PlayerManager.instance.caughtParticle, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            prisonObj = Instantiate(prison, this.transform) as GameObject;
        }
        else
        {
            Instantiate(PlayerManager.instance.destroyParticle, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            Destroy(prisonObj);
        }        
    }

    public void SetPosition(Vector3 _position)
    {
        if(transform.position == _position)
        {
            if(IsWalk) IsWalk = false;
        }
        else
        {
            if(!IsWalk) IsWalk = true;
            transform.position = _position;
        }
    }

    public void SetRotation(Quaternion _rotation)
    {
        transform.rotation = _rotation;
    }

    public void SetAnimation(string _animation, bool _value)
    {      
        _animator.SetBool(_animation, _value);
    }

    public void SetInvisible(bool _value) {
        Color _color = usernameText.color;

        if (_value) _color.a = 0;
        else _color.a = 1;

        Instantiate(PlayerManager.instance.visibleParticle, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);

        usernameText.color = _color;
        _renderer.enabled = !_value;
    }

    public void SetTexture(int _textureId)
    {
        if(GameManager.instance.isLobbySession)
        {
            _renderer.material.SetTexture("_BaseMap", WardrobeManager.instance.skinTextures[_textureId]);
            selectedSkinTextureId = _textureId;
        }
    }

    public void SetFootprint(bool _value)
    {
        footprint = _value;
    }

    private void CheckFootprint()
    {
        if(footprint)
        {
            if(footprintDur <= 0)
            {
                Instantiate(PlayerManager.instance.footprint, new Vector3(this.transform.position.x, 0.2f, this.transform.position.z), this.transform.rotation * Quaternion.Euler(new Vector3(90, this.transform.rotation.y, 0)));
                footprintDur = 0.2f;
            }

            else
            {
                footprintDur -= Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item") && id == PlayerManager.instance.seekerId)
        {
            if(Client.instance.myId == PlayerManager.instance.seekerId && GameManager.instance.isGameSession)
                StartCoroutine(GameSceneManager.instance.VisibleForSecond(6f));

            collision.gameObject.GetComponent<ItemScript>().DestryObj();
        }
    }
}
