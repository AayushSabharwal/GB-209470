using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Shoot { get; private set; }

    [SerializeField]
    private VariableJoystick moveStick;
    [SerializeField]
    private VariableJoystick shootStick;

    [SerializeField]
    private bool keyboard;

    private bool _isPaused;

    private void Start() {
        _isPaused = false;
        ReferenceManager.Inst.UIManager.OnPause += OnPause;
    }

    private void OnPause(bool isPaused) {
        _isPaused = isPaused;
    }

    private void Update() {
        if (_isPaused) return;

        if (keyboard) {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Shoot = new Vector2(Input.GetKey(KeyCode.J) ? -1f : Input.GetKey(KeyCode.L) ? 1f : 0f,
                                Input.GetKey(KeyCode.I) ? 1f : Input.GetKey(KeyCode.K) ? -1f : 0f);
        }
        else {
            Move = new Vector2(moveStick.Horizontal, moveStick.Vertical);
            Shoot = new Vector2(shootStick.Horizontal, shootStick.Vertical);
        }
    }
}