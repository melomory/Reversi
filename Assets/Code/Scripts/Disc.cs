using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField]
    private Player _up;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Flip()
    {
        if (_up == Player.Black)
        {
            _animator.Play("BlackToWhite");
            _up = Player.White;
        }
        else
        {
            _animator.Play("WhiteToBlack");
            _up = Player.Black;
        }
    }

    public void Twitch()
    {
        _animator.Play("TwitchDisc");
    }
}
